using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;

namespace Systems.Player.Animation
{
    /// <summary>
    /// プレイヤーアニメーションビューモデル
    /// </summary>
    public class PlayerAnimationViewModel : ViewModelBase
    {
        private readonly PlayerAnimationModel _model;
        private readonly ReactiveProperty<string> _current_animation;
        private readonly ReactiveProperty<float> _speed;
        private readonly ReactiveProperty<bool> _is_playing;

        public ReactiveProperty<string> CurrentAnimation => _current_animation;
        public ReactiveProperty<float> Speed => _speed;
        public ReactiveProperty<bool> IsPlaying => _is_playing;

        public PlayerAnimationViewModel(PlayerAnimationModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _current_animation = new ReactiveProperty<string>().AddTo(Disposables);
            _speed = new ReactiveProperty<float>().AddTo(Disposables);
            _is_playing = new ReactiveProperty<bool>().AddTo(Disposables);

            _current_animation.Subscribe(OnAnimationChanged).AddTo(Disposables);
            _speed.Subscribe(OnSpeedChanged).AddTo(Disposables);
            _is_playing.Subscribe(OnPlayingChanged).AddTo(Disposables);
        }

        public void Initialize()
        {
            _model.Initialize();
            UpdateAnimationState();
        }

        public void UpdateAnimation()
        {
            _model.Update();
            UpdateAnimationState();
        }

        public void HandleAnimation(string animationName)
        {
            _model.PlayAnimation(animationName);
        }

        private void UpdateAnimationState()
        {
            _current_animation.Value = _model.CurrentAnimation;
            _speed.Value = _model.Speed;
            _is_playing.Value = _model.IsPlaying;
        }

        private void OnAnimationChanged(string animation)
        {
            EventBus.Publish(new AnimationChangedEvent(animation));
        }

        private void OnSpeedChanged(float speed)
        {
            EventBus.Publish(new AnimationSpeedChangedEvent(speed));
        }

        private void OnPlayingChanged(bool playing)
        {
            EventBus.Publish(new AnimationPlayingChangedEvent(playing));
        }
    }
}
