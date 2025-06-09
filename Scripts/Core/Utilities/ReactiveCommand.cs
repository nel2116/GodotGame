using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Windows.Input;
using Core.Reactive;

namespace Core.Utilities
{
    /// <summary>
    /// 実行可能状態を持つリアクティブコマンド
    /// </summary>
    public class ReactiveCommand : ICommand, IDisposable
    {
        private readonly Subject<Unit> _executeSubject = new();
        private readonly ReactiveProperty<bool> _canExecute = new(true);
        private bool _disposed;

        /// <summary>
        /// 実行通知ストリーム
        /// </summary>
        public IObservable<Unit> ExecuteObservable => _executeSubject.AsObservable();

        /// <summary>
        /// 実行可否ストリーム
        /// </summary>
        public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

        public bool CanExecute(object parameter) => _canExecute.Value;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _executeSubject.OnNext(Unit.Default);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _executeSubject.Dispose();
            _canExecute.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// パラメーターを受け取るリアクティブコマンド
    /// </summary>
    public class ReactiveCommand<T> : ICommand, IDisposable
    {
        private readonly Subject<T> _executeSubject = new();
        private readonly ReactiveProperty<bool> _canExecute = new(true);
        private bool _disposed;

        /// <summary>
        /// 実行通知ストリーム
        /// </summary>
        public IObservable<T> ExecuteObservable => _executeSubject.AsObservable();

        /// <summary>
        /// 実行可否ストリーム
        /// </summary>
        public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

        public bool CanExecute(object parameter) => _canExecute.Value;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var value = parameter is T t ? t : default!;
            _executeSubject.OnNext(value);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _executeSubject.Dispose();
            _canExecute.Dispose();
            _disposed = true;
        }
    }
}
