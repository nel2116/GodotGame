using Godot;
using Core.Events;

namespace Systems.Player.Progression
{
    /// <summary>
    /// プレイヤー進行ビュー
    /// </summary>
    public partial class PlayerProgressionView : Node
    {
        private PlayerProgressionViewModel _view_model = default!;

        public override void _Ready()
        {
            var model = new PlayerProgressionModel();
            var bus = GameEventBus.Instance;
            _view_model = new PlayerProgressionViewModel(model, bus);
            _view_model.Initialize();
        }

        public override void _ExitTree()
        {
            _view_model.Dispose();
        }

        public void OnExperienceGained(int exp)
        {
            _view_model.AddExperience(exp);
        }

        public void OnSkillUnlockRequested(string skillName)
        {
            _view_model.UnlockSkill(skillName);
        }
    }
}
