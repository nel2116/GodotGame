using System;
using NUnit.Framework;
using Systems.Dungeon.Optimization;

namespace Tests.Core.Dungeon.Optimization
{
    public class LruCacheTests
    {
        [Test]
        public void Add_WithinCapacity_AllEntriesRetrievable()
        {
            var cache = new LruCache<string, int>(3);
            cache.Add("a", 1);
            cache.Add("b", 2);
            cache.Add("c", 3);

            Assert.IsTrue(cache.TryGet("a", out var a));
            Assert.AreEqual(1, a);
            Assert.IsTrue(cache.TryGet("b", out var b));
            Assert.AreEqual(2, b);
            Assert.IsTrue(cache.TryGet("c", out var c));
            Assert.AreEqual(3, c);
        }

        [Test]
        public void Add_ExceedsCapacity_EvictsLeastRecentlyUsed()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Add("b", 2);
            cache.Add("c", 3);

            Assert.IsFalse(cache.TryGet("a", out _));
            Assert.IsTrue(cache.TryGet("b", out _));
            Assert.IsTrue(cache.TryGet("c", out _));
        }

        [Test]
        public void TryGet_RefreshesUsageOrder_PreventsEvictionOfRecentlyAccessed()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Add("b", 2);

            cache.TryGet("a", out _);
            cache.Add("c", 3);

            Assert.IsTrue(cache.TryGet("a", out _));
            Assert.IsFalse(cache.TryGet("b", out _));
            Assert.IsTrue(cache.TryGet("c", out _));
        }

        [Test]
        public void Add_ExistingKey_UpdatesValueWithoutEviction()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Add("b", 2);
            cache.Add("a", 100);

            Assert.AreEqual(2, cache.Count);
            Assert.IsTrue(cache.TryGet("a", out var value));
            Assert.AreEqual(100, value);
        }

        [Test]
        public void Clear_RemovesAllEntries()
        {
            var cache = new LruCache<string, int>(2);
            cache.Add("a", 1);
            cache.Clear();

            Assert.AreEqual(0, cache.Count);
            Assert.IsFalse(cache.TryGet("a", out _));
        }

        [Test]
        public void Constructor_NonPositiveCapacity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = new LruCache<string, int>(0));
        }
    }
}
