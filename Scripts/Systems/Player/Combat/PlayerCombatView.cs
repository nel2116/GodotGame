using Core.Events;
using Godot;

namespace Systems.Player.Combat
{
    /// <summary>
    /// プレイヤー戦闘ビュー
    /// </summary>
    public partial class PlayerCombatView : Node
    {
        private readonly PlayerCombatViewModel _view_model;

        public PlayerCombatView()
        {
            var bus = GameEventBus.Instance;
            var model = new PlayerCombatModel(bus);
            _view_model = new PlayerCombatViewModel(model, bus);
        }

        public PlayerCombatView(GameEventBus bus, PlayerCombatModel model)
        {
            _view_model = new PlayerCombatViewModel(model, bus);
        }

        public override void _Ready()
        {
            var bus = GameEventBus.Instance;
            _view_model.Initialize();
        }

        public override void _Process(double delta)
        {
            _view_model.UpdateCombat();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }
    }
}
