using Core.Events;
using Godot;

namespace Systems.Common.State
{
    /// <summary>
    /// 共通状態システムのビュー
    /// </summary>
    public partial class CommonStateView : Node3D
    {
        private CommonStateViewModel _view_model = default!;

        public override void _Ready()
        {
            var bus = GameEventBus.Instance;
            var model = new CommonStateModel();
            _view_model = new CommonStateViewModel(model, bus);
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
