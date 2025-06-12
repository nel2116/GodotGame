using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Godot;

namespace Core.Events
{
	/// <summary>
	/// ゲームイベントを発行・購読するバス
	/// </summary>
	public class GameEventBus : IGameEventBus, IDisposable
	{
		private readonly ConcurrentDictionary<Type, ISubject<GameEvent>> _subjects = new();
		private readonly object _dispose_lock = new();
		// マルチスレッド環境での可視性を保証するため volatile を使用
		private volatile bool _disposed;
		private readonly int _maxEventQueueSize = 1000; // イベントキューサイズの上限

		/// <summary>
		/// イベントを発行
		/// </summary>
		public void Publish<T>(T evt) where T : GameEvent
		{
			if (_disposed)
			{
				GD.PrintErr("Attempted to publish event to disposed GameEventBus");
				return;
			}

			if (evt == null)
			{
				GD.PrintErr("Attempted to publish null event");
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
				GD.PrintErr($"Error publishing event of type {typeof(T).Name}: {ex.Message}");
			}
		}

		/// <summary>
		/// 指定型のイベントストリームを取得
		/// </summary>
		public IObservable<T> GetEventStream<T>() where T : GameEvent
		{
			if (_disposed)
			{
				GD.PrintErr("Attempted to get event stream from disposed GameEventBus");
				return Observable.Empty<T>();
			}

			try
			{
				return GetOrCreateSubject(typeof(T))
					.OfType<T>()
					.Buffer(TimeSpan.FromMilliseconds(16)) // フレームレートに合わせたバッファリング
					.SelectMany(events => events)
					.TakeUntil(Observable.Create<Unit>(observer =>
					{
						_disposed = true;
						observer.OnCompleted();
						return () => { };
					}));
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error getting event stream for type {typeof(T).Name}: {ex.Message}");
				return Observable.Empty<T>();
			}
		}

		private ISubject<GameEvent> GetOrCreateSubject(Type type)
		{
			return _subjects.GetOrAdd(type, _ =>
			{
				var subject = new Subject<GameEvent>();
				return Subject.Synchronize(subject);
			});
		}

		/// <summary>
		/// バスが保持するリソースを解放する
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
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
								GD.PrintErr($"Error disposing subject: {ex.Message}");
							}
						}
						_subjects.Clear();
					}
					catch (Exception ex)
					{
						GD.PrintErr($"Error during GameEventBus disposal: {ex.Message}");
					}
					finally
					{
						_disposed = true;
					}
				}
			}
		}
	}
}
