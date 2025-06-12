using Core.Events;
using Godot;

namespace Systems.Player.Input
{
    /// <summary>
    /// プレイヤー入力ビュー
    /// </summary>
    public partial class PlayerInputView : Node
    {
        private PlayerInputViewModel _viewModel = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            _viewModel = new PlayerInputViewModel(model, bus);
            _viewModel.Initialize();
        }

        public override void _Process(double delta)
        {
            _viewModel.UpdateInput();
        }

        public override void _ExitTree()
        {
            _viewModel.Dispose();
        }
    }
}
