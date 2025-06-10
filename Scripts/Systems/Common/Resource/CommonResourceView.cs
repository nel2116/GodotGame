using Core.Events;
using Godot;

namespace Systems.Common.Resource
{
    /// <summary>
    /// 共通リソース管理ビュー
    /// </summary>
    public partial class CommonResourceView : Node3D
    {
        private CommonResourceViewModel _view_model = default!;

        public override void _Ready()
        {
            var bus = new GameEventBus();
            var model = new CommonResourceModel();
            _view_model = new CommonResourceViewModel(model, bus);
            _view_model.Initialize();
        }

        public override void _Process(double delta)
        {
            _view_model.UpdateResource();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }
    }
}
