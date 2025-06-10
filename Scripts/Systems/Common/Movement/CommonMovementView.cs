using Core.Events;
using Godot;

namespace Systems.Common.Movement
{
    /// <summary>
    /// 共通移動システムのビュー
    /// </summary>
    public partial class CommonMovementView : Node3D
    {
        private CommonMovementViewModel _view_model = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new CommonMovementModel();
            _view_model = new CommonMovementViewModel(model, bus);
            _view_model.Initialize();
        }

        public override void _Process(double delta)
        {
            _view_model.UpdateMovement();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }
    }
}
