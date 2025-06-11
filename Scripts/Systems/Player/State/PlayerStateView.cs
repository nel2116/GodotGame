using Core.Events;
using Godot;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤー状態ビュー
    /// </summary>
    public partial class PlayerStateView : Node
    {
        private PlayerStateViewModel _view_model = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new PlayerStateModel(bus);
            _view_model = new PlayerStateViewModel(model, bus);
            _view_model.Initialize();
        }

        public override void _Process(double delta)
        {
            _view_model.UpdateState();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }
    }
}
