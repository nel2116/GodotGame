using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// アニメーション再生イベント
    /// </summary>
    public class AnimationPlayedEvent : GameEvent
    {
        public string AnimationName { get; }
        public AnimationPlayedEvent(string animationName)
        {
            AnimationName = animationName;
        }
    }

    /// <summary>
    /// アニメーションブレンド開始イベント
    /// </summary>
    public class AnimationBlendStartedEvent : GameEvent
    {
        public string FromAnimation { get; }
        public string ToAnimation { get; }
        public float BlendTime { get; }
        public AnimationBlendStartedEvent(string fromAnimation, string toAnimation, float blendTime)
        {
            FromAnimation = fromAnimation;
            ToAnimation = toAnimation;
            BlendTime = blendTime;
        }
    }

    /// <summary>
    /// アニメーションブレンド完了イベント
    /// </summary>
    public class AnimationBlendCompletedEvent : GameEvent
    {
        public string AnimationName { get; }
        public AnimationBlendCompletedEvent(string animationName)
        {
            AnimationName = animationName;
        }
    }

    /// <summary>
    /// アニメーション変更イベント
    /// </summary>
    public class AnimationChangedEvent : GameEvent
    {
        public string Animation { get; }
        public AnimationChangedEvent(string animation)
        {
            Animation = animation;
        }
    }

    /// <summary>
    /// アニメーション速度変更イベント
    /// </summary>
    public class AnimationSpeedChangedEvent : GameEvent
    {
        public float Speed { get; }
        public AnimationSpeedChangedEvent(float speed)
        {
            Speed = speed;
        }
    }

    /// <summary>
    /// アニメーション再生状態変更イベント
    /// </summary>
    public class AnimationPlayingChangedEvent : GameEvent
    {
        public bool IsPlaying { get; }
        public AnimationPlayingChangedEvent(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }
    }
}
