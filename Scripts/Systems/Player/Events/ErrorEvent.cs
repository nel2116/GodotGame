using Core.Events;
using Systems.Player.Base;

namespace Systems.Player.Events
{
    /// <summary>
    /// プレイヤーシステムで発生したエラーを通知するイベント
    /// </summary>
    public class ErrorEvent : GameEvent
    {
        public PlayerSystemException Exception { get; }
        public ErrorEvent(PlayerSystemException exception)
        {
            Exception = exception;
        }
    }
}
