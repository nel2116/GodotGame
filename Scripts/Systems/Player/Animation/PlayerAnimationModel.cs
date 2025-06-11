using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Player.Base;
using Systems.Player.Events;
using Core.Events;
using Systems.Player.State;
using Godot;

namespace Systems.Player.Animation
{
    /// <summary>
    /// プレイヤーアニメーションモデル
    /// </summary>
    public class PlayerAnimationModel : PlayerSystemBase
    {
        private readonly Dictionary<string, Godot.Animation> _clips = new();
        private bool _is_playing;
        public string CurrentAnimation { get; private set; } = "Idle";
        public float Speed { get; private set; } = 1.0f;
        public bool IsPlaying => _is_playing;

        public PlayerAnimationModel(IGameEventBus bus) : base(bus) { }

        public override void Initialize()
        {
            try
            {
                LoadAnimationClips();
                _is_playing = true;
                StateManager.RegisterState("Animation", new IdleState());
                StateManager.RegisterState("Playing", new PlayingState());
                StateManager.RegisterState("Paused", new PausedState());
                StateManager.RegisterTransition("Animation", "Playing", () => _is_playing);
                StateManager.RegisterTransition("Animation", "Paused", () => !_is_playing);
            }
            catch (Exception ex)
            {
                HandleError("Initialize", ex);
            }
        }

        public override void Update()
        {
            if (_is_playing)
            {
                // アニメーション更新処理
            }
        }

        public void PlayAnimation(string animationName)
        {
            try
            {
                if (!_clips.ContainsKey(animationName))
                {
                    throw new ArgumentException($"Invalid animation: {animationName}");
                }
                CurrentAnimation = animationName;
                _is_playing = true;
                EventBus.Publish(new AnimationPlayedEvent(animationName));
            }
            catch (Exception ex)
            {
                HandleError("PlayAnimation", ex);
            }
        }

        public async Task BlendAnimationAsync(string from, string to, float time)
        {
            try
            {
                if (!_clips.ContainsKey(from) || !_clips.ContainsKey(to))
                {
                    throw new ArgumentException("Invalid animation blend");
                }
                EventBus.Publish(new AnimationBlendStartedEvent(from, to, time));
                await Task.Delay(TimeSpan.FromSeconds(time));
                CurrentAnimation = to;
                EventBus.Publish(new AnimationBlendCompletedEvent(to));
            }
            catch (Exception ex)
            {
                HandleError("BlendAnimationAsync", ex);
            }
        }

        private void LoadAnimationClips()
        {
            _clips["Idle"] = new Godot.Animation();
            _clips["Walk"] = new Godot.Animation();
            _clips["Run"] = new Godot.Animation();
            _clips["Jump"] = new Godot.Animation();
            _clips["Attack"] = new Godot.Animation();
        }
    }
}
