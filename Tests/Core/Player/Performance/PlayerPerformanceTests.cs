using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.Movement;
using Systems.Player.Progression;
using Core.Events;

namespace Tests.Core.Player.Performance
{
    public class PlayerPerformanceTests
    {
        [Test, MaxTime(500)]
        public void Combat_Attack_Performance()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            model.Initialize();
            for (int i = 0; i < 1000; i++)
            {
                model.Attack("BasicAttack");
            }
        }

        [Test, MaxTime(500)]
        public void Movement_Update_Performance()
        {
            var bus = new GameEventBus();
            var model = new PlayerMovementModel(bus);
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();
            for (int i = 0; i < 1000; i++)
            {
                viewModel.UpdateMovement();
            }
        }

        [Test, MaxTime(500)]
        public void Progression_AddExperience_Performance()
        {
            var model = new PlayerProgressionModel();
            model.Initialize();
            for (int i = 0; i < 10000; i++)
            {
                model.AddExperience(1);
            }
        }

    }
}
