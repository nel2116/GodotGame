using System.Threading.Tasks;
using NUnit.Framework;
using Systems.Common.Resource;

namespace Tests.Core.Resource
{
    public class CommonResourceModelTests
    {
        [Test]
        public async Task LoadResource_AddsToCache()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            var data = await model.LoadResource("res://dummy");
            Assert.IsNotNull(data);
            Assert.IsTrue(model.ResourceCache.ContainsKey("res://dummy"));
        }

        [Test]
        public async Task UnloadResource_RemovesFromCache()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            await model.LoadResource("res://dummy");
            model.UnloadResource("res://dummy");
            Assert.IsFalse(model.ResourceCache.ContainsKey("res://dummy"));
        }

        [Test]
        public void GetResource_NotLoaded_ReturnsNull()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            var result = model.GetResource("res://none");
            Assert.IsNull(result);
        }
        [Test]
        public async Task LoadResource_Cached_DoesNotIncreaseSize()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            await model.LoadResource("res://dup");
            var size = model.CurrentCacheSize;
            await model.LoadResource("res://dup");
            Assert.AreEqual(size, model.CurrentCacheSize);
        }

        [Test]
        public async Task UnloadResource_DecreasesCacheSize()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            await model.LoadResource("res://a");
            var size = model.CurrentCacheSize;
            model.UnloadResource("res://a");
            Assert.Less(model.CurrentCacheSize, size);
        }

        [Test]
        public async Task GetResource_AfterLoad_ReturnsInstance()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            var data = await model.LoadResource("res://b");
            var retrieved = model.GetResource("res://b");
            Assert.AreSame(data, retrieved);
        }

        [Test]
        public async Task LoadResource_UpdatesLastAccessTime()
        {
            var model = new CommonResourceModel();
            model.Initialize();
            var data = await model.LoadResource("res://c");
            var before = data!.LastAccessTime;
            await Task.Delay(10);
            await model.LoadResource("res://c");
            Assert.Greater(data.LastAccessTime, before);
        }
    }
}
