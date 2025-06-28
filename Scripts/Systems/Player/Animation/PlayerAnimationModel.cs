using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Systems.Player.Base;
using Systems.Player.Events;
using Core.Events;
using Systems.Player.State;
using Core.Utilities;

namespace Systems.Player.Animation
{
    /// <summary>
    /// アニメーションクリップのモッククラス
    /// </summary>
    public class MockAnimation
    {
        public string Name { get; set; } = "";
        public float Length { get; set; } = 1.0f;
        public bool IsPlaying { get; set; } = false;
    }

    /// <summary>
    /// プレイヤーアニメーションモデル
    /// </summary>
    public class PlayerAnimationModel : PlayerSystemBase
    {
        private readonly Dictionary<string, MockAnimation> _clips = new();
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
            // テスト環境ではモックアニメーションを使用
            _clips["Idle"] = new MockAnimation { Name = "Idle", Length = 1.0f };
            _clips["Walk"] = new MockAnimation { Name = "Walk", Length = 1.0f };
            _clips["Run"] = new MockAnimation { Name = "Run", Length = 1.0f };
            _clips["Jump"] = new MockAnimation { Name = "Jump", Length = 1.0f };
            _clips["Attack"] = new MockAnimation { Name = "Attack", Length = 1.0f };
            
            GodotMock.Print("Animation clips loaded successfully");
        }

        /// <summary>
        /// アニメーションクリップを取得
        /// </summary>
        public MockAnimation? GetClip(string name)
        {
            return _clips.TryGetValue(name, out var clip) ? clip : null;
        }

        /// <summary>
        /// 利用可能なアニメーション名を取得
        /// </summary>
        public IEnumerable<string> GetAvailableAnimations()
        {
            return _clips.Keys;
        }
    }
}
