using System;

namespace Core.Reactive
{
    /// <summary>
    /// 値変更通知を提供するリアクティブプロパティのインターフェース
    /// </summary>
    public interface IReactiveProperty<T> : IDisposable
    {
        /// <summary>
        /// 現在の値を取得または設定
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// 値変更を購読
        /// </summary>
        IDisposable Subscribe(Action<T> onNext);
    }
}
