using System.Threading.Tasks;
using NUnit.Framework;
using Core.Events;
using Systems.Common.Resource;
using Systems.Common.Events;

namespace Tests.Core.Resource
{
    public class CommonResourceViewModelTests
    {
        [Test]
        public void Initialize_PublishesCacheEvent()
        {
            var bus = new GameEventBus();
            var model = new CommonResourceModel();
            ResourceCacheChangedEvent? received = null;
            bus.GetEventStream<ResourceCacheChangedEvent>().Subscribe(e => received = e);
            var vm = new CommonResourceViewModel(model, bus);
            vm.Initialize();
            Assert.IsNotNull(received);
            Assert.AreEqual(0, received!.Cache.Count);
        }
    }
}

