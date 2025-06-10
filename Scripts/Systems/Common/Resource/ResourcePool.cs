using System.Collections.Generic;

namespace Systems.Common.Resource
{
    /// <summary>
    /// リソースプール
    /// </summary>
    public class ResourcePool
    {
        private readonly List<ResourceData> _resources = new();

        public void Add(ResourceData data)
        {
            _resources.Add(data);
        }

        public void Update()
        {
            foreach (var res in _resources)
            {
                res.Touch();
            }
        }
    }
}
