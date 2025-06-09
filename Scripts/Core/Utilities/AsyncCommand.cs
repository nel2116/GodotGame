using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Windows.Input;
using Core.Reactive;

namespace Core.Utilities
{
    /// <summary>
    /// 非同期処理向けのリアクティブコマンド
    /// </summary>
    public class AsyncCommand : ICommand, IDisposable
    {
        private readonly Func<Task> _execute;
        private readonly ReactiveProperty<bool> _isExecuting = new(false);
        private bool _disposed;

        /// <summary>
        /// 実行中状態を監視
        /// </summary>
        public IObservable<bool> IsExecuting => _isExecuting.ValueChanged;

        public AsyncCommand(Func<Task> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter) => !_isExecuting.Value;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            try
            {
                _isExecuting.Value = true;
                await _execute();
            }
            finally
            {
                _isExecuting.Value = false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _isExecuting.Dispose();
            _disposed = true;
        }
    }
}
