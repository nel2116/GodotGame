using NUnit.Framework;
using Systems.Common.State;

namespace Tests.Core.State
{
    public class CommonStateModelTests
    {
        [Test]
        public void ChangeState_ValidTransition_UpdatesCurrentState()
        {
            var model = new CommonStateModel();
            model.Initialize();
            model.ChangeState("Walk");
            Assert.AreEqual("Walk", model.CurrentState);
        }

        [Test]
        public void CanTransitionTo_Invalid_ReturnsFalse()
        {
            var model = new CommonStateModel();
            model.Initialize();
            Assert.IsFalse(model.CanTransitionTo("Invalid"));
        }

        [Test]
        public void ChangeState_Invalid_DoesNotChangeState()
        {
            var model = new CommonStateModel();
            model.Initialize();
            model.ChangeState("Invalid");
            Assert.AreEqual("Idle", model.CurrentState);
        }

        [Test]
        public void CanTransitionTo_Valid_ReturnsTrue()
        {
            var model = new CommonStateModel();
            model.Initialize();
            Assert.IsTrue(model.CanTransitionTo("Walk"));
        }
        [Test]
        public void ChangeState_Sequence_ValidPath()
        {
            var model = new CommonStateModel();
            model.Initialize();
            model.ChangeState("Walk");
            model.ChangeState("Run");
            model.ChangeState("Jump");
            model.ChangeState("Fall");
            model.ChangeState("Idle");
            Assert.AreEqual("Idle", model.CurrentState);
        }

        [Test]
        public void ChangeState_InvalidAfterValid_NoChange()
        {
            var model = new CommonStateModel();
            model.Initialize();
            model.ChangeState("Walk");
            model.ChangeState("Jump");
            Assert.AreEqual("Walk", model.CurrentState);
        }

        [Test]
        public void Initialize_SetsTransitions()
        {
            var model = new CommonStateModel();
            model.Initialize();
            Assert.IsTrue(model.StateTransitions.ContainsKey("Idle"));
            Assert.AreEqual("Walk", model.StateTransitions["Idle"]);
        }
    }
}
