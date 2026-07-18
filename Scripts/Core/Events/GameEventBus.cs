using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using Core.Utilities;

namespace Core.Events
{
	/// <summary>
	/// ゲームイベントを発行・購読するバス。
	/// シングルトンパターンで実装され、マルチスレッド環境でも安全に動作する。
	/// </summary>
	public class GameEventBus : IGameEventBus, IDisposable
	{
		private const int MaxEventQueueSize = 1000;

		private static GameEventBus? _instance;
		private static readonly object _instanceLock = new();

		/// <summary>
		/// シングルトンインスタンスを取得する。
		/// 初回アクセス時に遅延初期化される。
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
		private readonly object _disposeLock = new();
		// マルチスレッド環境での可視性を保証するため volatile を使用
		private volatile bool _disposed;

		/// <summary>
		/// インスタンスを生成するコンストラクタ。
		/// </summary>
		public GameEventBus()
		{
			_disposed = false;
		}

		/// <summary>
		/// イベントを発行する。
		/// 破棄済みのバスへの発行や null イベントは無視される。
		/// </summary>
		/// <typeparam name="T">イベントの型</typeparam>
		/// <param name="evt">発行するイベント</param>
		public void Publish<T>(T evt) where T : GameEvent
		{
			if (_disposed)
			{
				return;
			}

			if (evt == null)
			{
				return;
			}

			try
			{
				var subject = GetOrCreateSubject(typeof(T));
				subject.OnNext(evt);
			}
			catch (Exception ex)
			{
				if (!GodotMock.IsTestEnvironment())
				{
					Console.Error.WriteLine($"Error publishing event of type {typeof(T).Name}: {ex.Message}");
				}
				throw;
			}
		}

		/// <summary>
		/// 指定型のイベントストリームを取得する。
		/// ReplaySubject を使用しているため、購読前に発行されたイベントも取得できる。
		/// </summary>
		/// <typeparam name="T">イベントの型</typeparam>
		/// <returns>イベントストリーム。破棄済みの場合は空のストリームを返す。</returns>
		public IObservable<T> GetEventStream<T>() where T : GameEvent
		{
			if (_disposed)
			{
				return Observable.Empty<T>();
			}

			try
			{
				return GetOrCreateSubject(typeof(T))
					.OfType<T>();
			}
			catch (Exception ex)
			{
				if (!GodotMock.IsTestEnvironment())
				{
					Console.Error.WriteLine($"Error getting event stream for type {typeof(T).Name}: {ex.Message}");
				}
				return Observable.Empty<T>();
			}
		}

		/// <summary>
		/// 指定型の Subject を取得または作成する。
		/// スレッドセーフな Subject を返す。
		/// </summary>
		/// <param name="type">イベントの型</param>
		/// <returns>スレッドセーフな Subject</returns>
		private ISubject<GameEvent> GetOrCreateSubject(Type type)
		{
			return _subjects.GetOrAdd(type, _ =>
			{
				var replaySubject = new ReplaySubject<GameEvent>(MaxEventQueueSize);
				return Subject.Synchronize(replaySubject);
			});
		}

		/// <summary>
		/// バスが保持するリソースを解放する（テスト用）。
		/// アプリ本体からは呼び出さないこと。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// IDisposable の明示的実装。
		/// アプリ本体からは何もしない（テスト用の Dispose() メソッドを使用すること）。
		/// </summary>
		void IDisposable.Dispose()
		{
			// アプリ本体からは Dispose できないようにする
		}

		/// <summary>
		/// リソース解放処理本体。
		/// すべての Subject を完了させ、リソースを解放する。
		/// </summary>
		/// <param name="disposing">マネージドリソースを解放する場合 true</param>
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (!disposing)
			{
				return;
			}

			lock (_disposeLock)
			{
				if (_disposed)
				{
					return;
				}

				try
				{
					DisposeAllSubjects();
					_subjects.Clear();
				}
				catch (Exception ex)
				{
					if (!GodotMock.IsTestEnvironment())
					{
						Console.Error.WriteLine($"Error during GameEventBus disposal: {ex.Message}");
					}
				}
				finally
				{
					_disposed = true;
					_instance = null;
				}
			}
		}

		/// <summary>
		/// すべての Subject を完了させ、リソースを解放する。
		/// 個別の Subject の解放に失敗しても処理を継続する。
		/// </summary>
		private void DisposeAllSubjects()
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
					if (!GodotMock.IsTestEnvironment())
					{
						Console.Error.WriteLine($"Error disposing subject: {ex.Message}");
					}
				}
			}
		}
	}
}
