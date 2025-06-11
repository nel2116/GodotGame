using Core.Events;
using Godot;

namespace Systems.Player.Animation
{
    /// <summary>
    /// プレイヤーアニメーションビュー
    /// </summary>
    public partial class PlayerAnimationView : Node
    {
        private PlayerAnimationViewModel _view_model = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            _view_model = new PlayerAnimationViewModel(model, bus);
            _view_model.Initialize();
        }

        public override void _Process(double delta)
        {
            _view_model.UpdateAnimation();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }
    }
}
