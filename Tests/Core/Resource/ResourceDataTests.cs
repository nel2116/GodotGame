using NUnit.Framework;
using Systems.Common.Resource;
using System.Threading.Tasks;

namespace Tests.Core.Resource
{
    public class ResourceDataTests
    {
        [Test]
        public void Touch_UpdatesLastAccessTime()
        {
            var data = new ResourceData { Size = 1 };
            var before = data.LastAccessTime;
            Task.Delay(10).Wait();
            data.Touch();
            Assert.Greater(data.LastAccessTime, before);
        }
    }
}
