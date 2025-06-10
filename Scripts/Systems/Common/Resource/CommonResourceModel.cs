using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Reactive;

namespace Systems.Common.Resource
{
    /// <summary>
    /// 共通リソース管理モデル
    /// </summary>
    public class CommonResourceModel : IResourceSystem
    {
        private readonly CompositeDisposable _disposables = new();
        private Dictionary<string, ResourceData> _resource_cache = new();
        private Dictionary<string, ResourcePool> _resource_pools = new();
        private int _max_cache_size;
        private int _current_cache_size;

        /// <summary>
        /// キャッシュクリア時に削除するリソース数
        /// </summary>
        private const int MAX_RESOURCES_TO_EVICT = 5;

        /// <summary>
        /// キャッシュ
        /// </summary>
        public Dictionary<string, ResourceData> ResourceCache => _resource_cache;

        /// <summary>
        /// 現在のキャッシュサイズ
        /// </summary>
        public int CurrentCacheSize => _current_cache_size;

        /// <inheritdoc />
        public void Initialize()
        {
            _resource_cache = new Dictionary<string, ResourceData>();
            _resource_pools = new Dictionary<string, ResourcePool>();
            _max_cache_size = 1024 * 1024 * 100;
            _current_cache_size = 0;
        }

        /// <inheritdoc />
        public void Update()
        {
            UpdateResourceState();
            CleanupUnusedResources();
        }

        /// <inheritdoc />
        public async Task<ResourceData?> LoadResource(string path)
        {
            if (_resource_cache.ContainsKey(path))
            {
                var cached = _resource_cache[path];
                cached.Touch();
                return cached;
            }

            if (_current_cache_size >= _max_cache_size)
            {
                CleanupOldestResources();
            }

            var data = await LoadResourceAsync(path);
            if (data != null)
            {
                _resource_cache[path] = data;
                _current_cache_size += data.Size;
            }
            return data;
        }

        /// <inheritdoc />
        public void UnloadResource(string path)
        {
            if (_resource_cache.ContainsKey(path))
            {
                var data = _resource_cache[path];
                _current_cache_size -= data.Size;
                _resource_cache.Remove(path);
                data.Dispose();
            }
        }

        /// <inheritdoc />
        public ResourceData? GetResource(string path)
        {
            return _resource_cache.TryGetValue(path, out var data) ? data : null;
        }

        private async Task<ResourceData?> LoadResourceAsync(string path)
        {
            return await Task.Run(() => new ResourceData { Size = 1 });
        }

        private void UpdateResourceState()
        {
            foreach (var pool in _resource_pools.Values)
            {
                pool.Update();
            }
        }

        private void CleanupUnusedResources()
        {
            var unused = _resource_cache.Where(kvp => !kvp.Value.IsInUse)
                .Select(kvp => kvp.Key).ToList();
            foreach (var key in unused)
            {
                UnloadResource(key);
            }
        }

        private void CleanupOldestResources()
        {
            var oldest = _resource_cache.OrderBy(kvp => kvp.Value.LastAccessTime)
                .Take(MAX_RESOURCES_TO_EVICT).Select(kvp => kvp.Key).ToList();
            foreach (var key in oldest)
            {
                UnloadResource(key);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
