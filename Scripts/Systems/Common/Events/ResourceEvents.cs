using System.Collections.Generic;
using Core.Events;

namespace Systems.Common.Events
{
    /// <summary>
    /// リソースキャッシュ変更イベント
    /// </summary>
    public class ResourceCacheChangedEvent : GameEvent
    {
        public IReadOnlyDictionary<string, Resource.ResourceData> Cache { get; }
        public ResourceCacheChangedEvent(IReadOnlyDictionary<string, Resource.ResourceData> cache)
        {
            Cache = cache;
        }
    }
}
