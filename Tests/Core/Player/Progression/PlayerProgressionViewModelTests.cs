using NUnit.Framework;
using Systems.Player.Progression;
using Systems.Player.Events;
using Core.Events;
using System.Threading.Tasks;

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
        public async Task UnlockSkill_ValidSkill_PublishesEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            SkillUnlockedEvent? received = null;
            bus.GetEventStream<SkillUnlockedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerProgressionModel();
            var viewModel = new PlayerProgressionViewModel(model, bus);
            viewModel.Initialize();
            viewModel.AddExperience(100); // レベル 2 に到達

            var result = viewModel.UnlockSkill("Fireball");

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsTrue(result);
            Assert.IsNotNull(received);
            Assert.AreEqual("Fireball", received!.SkillName);
            Assert.That(viewModel.UnlockedSkills.Value.Exists(s => s.SkillName == "Fireball"));
        }

        [Test]
        public async Task AddExperience_PublishesExperienceAndLevelUp()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            ExperienceChangedEvent? exp = null;
            LevelUpEvent? level = null;
            bus.GetEventStream<ExperienceChangedEvent>().Subscribe(e => exp = e);
            bus.GetEventStream<LevelUpEvent>().Subscribe(e => level = e);
            
            var model = new PlayerProgressionModel();
            var viewModel = new PlayerProgressionViewModel(model, bus);
            viewModel.Initialize();

            viewModel.AddExperience(150);

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(exp);
            // レベルアップ時にCurrentExpは次レベルの必要経験値分を消費するため、150 - 100 = 50が正しい
            Assert.AreEqual(50, exp!.Experience);
            Assert.IsNotNull(level);
            Assert.AreEqual(2, level!.Level);
        }
    }
}
