using NUnit.Framework;
using Systems.Player.Progression;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.Progression
{
    public class PlayerProgressionViewModelTests
    {
        [Test]
        public void AddExperience_LevelUpIncrements()
        {
            var bus = new GameEventBus();
            var model = new PlayerProgressionModel();
            var viewModel = new PlayerProgressionViewModel(model, bus);
            viewModel.Initialize();
            viewModel.AddExperience(1000);
            Assert.That(viewModel.Level.Value, Is.EqualTo(2));
        }

        [Test]
        public void UnlockSkill_ValidSkill_PublishesEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerProgressionModel();
            var viewModel = new PlayerProgressionViewModel(model, bus);
            viewModel.Initialize();
            viewModel.AddExperience(100); // レベル 2 に到達

            SkillUnlockedEvent? received = null;
            bus.GetEventStream<SkillUnlockedEvent>().Subscribe(e => received = e);

            var result = viewModel.UnlockSkill("Fireball");

            Assert.IsTrue(result);
            Assert.IsNotNull(received);
            Assert.AreEqual("Fireball", received!.SkillName);
            Assert.That(viewModel.UnlockedSkills.Value.Exists(s => s.SkillName == "Fireball"));
        }

        [Test]
        public void AddExperience_PublishesExperienceAndLevelUp()
        {
            var bus = new GameEventBus();
            var model = new PlayerProgressionModel();
            var viewModel = new PlayerProgressionViewModel(model, bus);
            viewModel.Initialize();

            ExperienceChangedEvent? exp = null;
            LevelUpEvent? level = null;
            bus.GetEventStream<ExperienceChangedEvent>().Subscribe(e => exp = e);
            bus.GetEventStream<LevelUpEvent>().Subscribe(e => level = e);

            viewModel.AddExperience(150);

            Assert.IsNotNull(exp);
            Assert.AreEqual(150, exp!.Experience);
            Assert.IsNotNull(level);
            Assert.AreEqual(2, level!.Level);
        }
    }
}
