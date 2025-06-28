using Core.Events;

namespace Systems.Common.Events
{
    /// <summary>
    /// アニメーション変更イベント
    /// </summary>
    public class AnimationChangedEvent : GameEvent
    {
        public string AnimationName { get; }
        public AnimationChangedEvent(string name)
        {
            AnimationName = name;
        }
    }

    /// <summary>
    /// 再生状態変更イベント
    /// </summary>
    public class AnimationPlayingChangedEvent : GameEvent
    {
        public bool IsPlaying { get; }
        public AnimationPlayingChangedEvent(bool playing)
        {
            IsPlaying = playing;
        }
    }
}
