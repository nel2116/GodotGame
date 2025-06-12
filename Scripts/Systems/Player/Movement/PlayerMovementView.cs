using Core.Events;
using Godot;

namespace Systems.Player.Movement
{
    /// <summary>
    /// プレイヤー移動ビュー
    /// </summary>
    public partial class PlayerMovementView : Node
    {
        private PlayerMovementViewModel _viewModel = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new PlayerMovementModel(bus);
            _viewModel = new PlayerMovementViewModel(model, bus);
            _viewModel.Initialize();
        }

        public override void _Process(double delta)
        {
            _viewModel.UpdateMovement();
        }

        public override void _ExitTree()
        {
            _viewModel.Dispose();
        }
    }
}
