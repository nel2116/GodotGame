using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Core.Utilities;

namespace Tests.Core
{
    public class CommandTests
    {
        [Test]
        public void ReactiveCommand_Execute_Notifies()
        {
            var cmd = new ReactiveCommand();
            bool executed = false;
            using (cmd.ExecuteObservable.Subscribe(_ => executed = true))
            {
                cmd.Execute(null);
            }
            Assert.IsTrue(executed);
        }

        [Test]
        public void ReactiveCommandT_Execute_PassesValue()
        {
            var cmd = new ReactiveCommand<int>();
            int received = 0;
            using (cmd.ExecuteObservable.Subscribe(v => received = v))
            {
                cmd.Execute(42);
            }
            Assert.AreEqual(42, received);
        }

        [Test]
        public async Task AsyncCommand_Execute_UpdatesState()
        {
            bool run = false;
            var cmd = new AsyncCommand(async () =>
            {
                await Task.Delay(10);
                run = true;
            });
            cmd.Execute(null);
            await Task.Delay(20);
            Assert.IsTrue(run);
        }

        [Test]
        public void ReactiveCommand_CanExecuteChanged_Raises()
        {
            var cmd = new ReactiveCommand();
            bool raised = false;
            cmd.CanExecuteChanged += (_, _) => raised = true;
            cmd.SetCanExecute(false);
            Assert.IsTrue(raised);
        }
    }
}
