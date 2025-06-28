using System;

namespace Core.Reactive
{
    /// <summary>
    /// IDisposableをCompositeDisposableに追加する拡張メソッド
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// CompositeDisposableに登録
        /// </summary>
        public static T AddTo<T>(this T disposable, CompositeDisposable composite) where T : IDisposable
        {
            composite.Add(disposable);
            return disposable;
        }
    }
}
