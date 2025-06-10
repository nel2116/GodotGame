using Core.Events;
using Godot;

namespace Systems.Common.Movement
{
    /// <summary>
    /// 共通移動システムのビュー
    /// </summary>
    public partial class CommonMovementView : Node3D
    {
        private readonly GameEventBus _bus;
        private CommonMovementViewModel _view_model = default!;

        public CommonMovementView() : this(new GameEventBus())
        {
        }

        public CommonMovementView(GameEventBus bus)
        {
            _bus = bus;
        }

        public override void _Ready()
        {
            var model = new CommonMovementModel();
            _view_model = new CommonMovementViewModel(model, _bus);
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
