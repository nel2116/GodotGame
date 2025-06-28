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
        public async Task Initialize_PublishesCacheEvent()
        {
            // 準備
            var bus = new GameEventBus();
            var model = new CommonResourceModel();
            var vm = new CommonResourceViewModel(model, bus);
            vm.Initialize();
            ResourceCacheChangedEvent receivedEvent = null;
            bus.GetEventStream<ResourceCacheChangedEvent>().Subscribe(e => receivedEvent = e);
            
            // 実行
            vm.Initialize();
            await Task.Delay(20); // イベント処理の遅延を考慮（バッファリング16ms + 余裕）
            
            // 検証
            Assert.That(receivedEvent, Is.Not.Null);
        }
    }
}

