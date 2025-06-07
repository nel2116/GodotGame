using System;
using System.Reactive.Linq;

namespace Core.Events
{
    /// <summary>
    /// ゲームイベントバスのインターフェース
    /// </summary>
    public interface IGameEventBus
    {
        /// <summary>
        /// イベントを発行
        /// </summary>
        void Publish<T>(T evt) where T : IGameEvent;

        /// <summary>
        /// 指定型のイベントストリームを取得
        /// </summary>
        IObservable<T> GetEventStream<T>() where T : IGameEvent;
    }
}
