using NUnit.Framework;
using System;
using System.Threading.Tasks;
using System.Threading;
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

        [Test]
        public void ReactiveCommandT_InvalidParameter_Throws()
        {
            var cmd = new ReactiveCommand<int>();
            Assert.Throws<ArgumentException>(() => cmd.Execute("string"));
        }

        [Test]
        public async Task AsyncCommand_ExecuteWhileRunning_Ignored()
        {
            int count = 0;
            var tcs = new TaskCompletionSource<bool>();
            var cmd = new AsyncCommand(async () =>
            {
                Interlocked.Increment(ref count);
                await tcs.Task;
            });

            cmd.Execute(null);
            cmd.Execute(null); // 実行中は無視されるはず

            await Task.Delay(10);
            Assert.AreEqual(1, count);

            tcs.SetResult(true);
        }

        [Test]
        public void ReactiveCommand_Disposed_Throws()
        {
            var cmd = new ReactiveCommand();
            cmd.Dispose();
            Assert.Throws<ObjectDisposedException>(() => cmd.Execute(null));
        }

        [Test]
        public void ReactiveCommandT_NullParameter_Default()
        {
            var cmd = new ReactiveCommand<int>();
            int value = -1;
            using (cmd.ExecuteObservable.Subscribe(v => value = v))
            {
                cmd.Execute(null);
            }
            Assert.AreEqual(0, value);
        }

        [Test]
        public void AsyncCommand_NullExecute_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AsyncCommand(null!));
        }

    }
}
