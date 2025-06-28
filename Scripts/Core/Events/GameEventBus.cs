using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using Core.Utilities;

namespace Core.Events
{
	/// <summary>
	/// ゲームイベントを発行・購読するバス
	/// </summary>
	public class GameEventBus : IGameEventBus, IDisposable
	{
		private static GameEventBus? _instance;
		private static readonly object _instanceLock = new();

		/// <summary>
		/// シングルトンインスタンスを取得
		/// </summary>
		public static GameEventBus Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_instanceLock)
					{
						_instance ??= new GameEventBus();
					}
				}
				return _instance;
			}
		}

		private readonly ConcurrentDictionary<Type, ISubject<GameEvent>> _subjects = new();
		private readonly object _dispose_lock = new();
		// マルチスレッド環境での可視性を保証するため volatile を使用
		private volatile bool _disposed;
		private readonly int _maxEventQueueSize = 1000; // イベントキューサイズの上限

		/// <summary>
		/// インスタンスを生成するコンストラクタ
		/// </summary>
		public GameEventBus() { _disposed = false; }

		/// <summary>
		/// イベントを発行
		/// </summary>
		public void Publish<T>(T evt) where T : GameEvent
		{
			if (_disposed)
			{
				// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
				// GodotMock.PrintErr("Attempted to publish event to disposed GameEventBus");
				return;
			}

			if (evt == null)
			{
				// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
				// GodotMock.PrintErr("Attempted to publish null event");
				return;
			}

			try
			{
				var subject = GetOrCreateSubject(typeof(T));
				if (subject is ISubject<GameEvent> typedSubject)
				{
					typedSubject.OnNext(evt);
				}
			}
			catch (Exception ex)
			{
				// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
				// GodotMock.PrintErr($"Error publishing event of type {typeof(T).Name}: {ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// 指定型のイベントストリームを取得
		/// </summary>
		public IObservable<T> GetEventStream<T>() where T : GameEvent
		{
			if (_disposed)
			{
				// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
				// GodotMock.PrintErr("Attempted to get event stream from disposed GameEventBus");
				return Observable.Empty<T>();
			}

			try
			{
				return GetOrCreateSubject(typeof(T))
					.OfType<T>()
					.Buffer(TimeSpan.FromMilliseconds(16)) // フレームレートに合わせたバッファリング
					.SelectMany(events => events);
			}
			catch (Exception ex)
			{
				// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
				// GodotMock.PrintErr($"Error getting event stream for type {typeof(T).Name}: {ex.Message}");
				return Observable.Empty<T>();
			}
		}

		private ISubject<GameEvent> GetOrCreateSubject(Type type)
		{
			return _subjects.GetOrAdd(type, _ =>
			{
				var subject = new ReplaySubject<GameEvent>(_maxEventQueueSize);
				return Subject.Synchronize(subject);
			});
		}

		/// <summary>
		/// バスが保持するリソースを解放する（テスト用）
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// IDisposableの明示的実装（アプリ本体からは何もしない）
		/// </summary>
		void IDisposable.Dispose()
		{
			// アプリ本体からはDisposeできないようにする
			// 必要なら警告ログを出してもよい
		}

		/// <summary>
		/// リソース解放処理本体
		/// </summary>
		/// <param name="disposing">マネージドリソースを解放する場合 true</param>
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				lock (_dispose_lock)
				{
					if (_disposed)
					{
						return;
					}

					try
					{
						foreach (var subject in _subjects.Values)
						{
							try
							{
								subject.OnCompleted();
								if (subject is IDisposable disposable)
								{
									disposable.Dispose();
								}
							}
							catch (Exception ex)
							{
								// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
								// GodotMock.PrintErr($"Error disposing subject: {ex.Message}");
							}
						}
						_subjects.Clear();
					}
					catch (Exception ex)
					{
						// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
						// GodotMock.PrintErr($"Error during GameEventBus disposal: {ex.Message}");
					}
					finally
					{
						_disposed = true;
						_instance = null;
					}
				}
			}
		}
	}
}
