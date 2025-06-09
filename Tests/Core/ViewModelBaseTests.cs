using NUnit.Framework;
using System;
using Core.Events;
using Core.ViewModels;
using Core.Reactive;

namespace Tests.Core
{
    public class ViewModelBaseTests
    {
        private class TestEvent : GameEvent { }

        private class TestViewModel : ViewModelBase
        {
            public TestViewModel(IGameEventBus eventBus) : base(eventBus) { }

            public IDisposable ExposeSubscribe<T>(Action<T> onNext) where T : GameEvent
            {
                return SubscribeToEvent(onNext);
            }

            public T ExposeGet<T>(IReactiveProperty<T> property)
            {
                return GetValue(property);
            }

            public void ExposeSet<T>(IReactiveProperty<T> property, T value)
            {
                SetValue(property, value);
            }
        }

        [Test]
        public void SubscribeToEvent_AddsToDisposables()
        {
            var eventBus = new GameEventBus();
            var viewModel = new TestViewModel(eventBus);
            bool received = false;

            viewModel.ExposeSubscribe<TestEvent>(_ => received = true);
            eventBus.Publish(new TestEvent());

            Assert.IsTrue(received);
            viewModel.Dispose();
        }

        [Test]
        public void Dispose_UnsubscribesEvents()
        {
            var eventBus = new GameEventBus();
            var viewModel = new TestViewModel(eventBus);
            bool received = false;

            viewModel.ExposeSubscribe<TestEvent>(_ => received = true);
            viewModel.Dispose();
            eventBus.Publish(new TestEvent());

            Assert.IsFalse(received);
        }

        [Test]
        public void GetValue_ReturnsPropertyValue()
        {
            var eventBus = new GameEventBus();
            var viewModel = new TestViewModel(eventBus);
            var property = new ReactiveProperty<int>(123);

            int value = viewModel.ExposeGet(property);

            Assert.AreEqual(123, value);
        }

        [Test]
        public void SetValue_UpdatesPropertyValue()
        {
            var eventBus = new GameEventBus();
            var viewModel = new TestViewModel(eventBus);
            var property = new ReactiveProperty<string>("old");

            viewModel.ExposeSet(property, "new");

            Assert.AreEqual("new", property.Value);
        }
    }
}
