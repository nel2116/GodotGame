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
        private readonly CompositeDisposable _disposables = new();
        private event EventHandler? _canExecuteChanged;
        private bool _disposed;

        /// <summary>
        /// 実行通知ストリーム
        /// </summary>
        public IObservable<Unit> ExecuteObservable => _executeSubject.AsObservable();

        /// <summary>
        /// 実行可否ストリーム
        /// </summary>
        public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

        public ReactiveCommand()
        {
            _canExecute.ValueChanged
                .Subscribe(_ => _canExecuteChanged?.Invoke(this, EventArgs.Empty))
                .AddTo(_disposables);
        }

        public bool CanExecute(object parameter) => _canExecute.Value;

        public event EventHandler? CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        public void SetCanExecute(bool value)
        {
            _canExecute.Value = value;
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
            _disposables.Dispose();
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
        private readonly CompositeDisposable _disposables = new();
        private event EventHandler? _canExecuteChanged;
        private bool _disposed;

        /// <summary>
        /// 実行通知ストリーム
        /// </summary>
        public IObservable<T> ExecuteObservable => _executeSubject.AsObservable();

        /// <summary>
        /// 実行可否ストリーム
        /// </summary>
        public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

        public ReactiveCommand()
        {
            _canExecute.ValueChanged
                .Subscribe(_ => _canExecuteChanged?.Invoke(this, EventArgs.Empty))
                .AddTo(_disposables);
        }

        public bool CanExecute(object parameter) => _canExecute.Value;

        public event EventHandler? CanExecuteChanged
        {
            add { _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        public void SetCanExecute(bool value)
        {
            _canExecute.Value = value;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            if (parameter is T t)
            {
                _executeSubject.OnNext(t);
            }
            else if (parameter is null)
            {
                _executeSubject.OnNext(default!);
            }
            else
            {
                throw new ArgumentException($"Invalid parameter type. Expected {typeof(T)}, but got {parameter.GetType()}", nameof(parameter));
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposables.Dispose();
            _executeSubject.Dispose();
            _canExecute.Dispose();
            _disposed = true;
        }
    }
}
