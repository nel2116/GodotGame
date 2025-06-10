using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Common.Events;

namespace Systems.Common.Resource
{
    /// <summary>
    /// 共通リソース管理ビューモデル
    /// </summary>
    public class CommonResourceViewModel : ViewModelBase
    {
        private readonly CommonResourceModel _model;
        public ReactiveProperty<Dictionary<string, ResourceData>> ResourceCache { get; }
        public ReactiveProperty<int> CacheSize { get; }

        public CommonResourceViewModel(CommonResourceModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            ResourceCache = new ReactiveProperty<Dictionary<string, ResourceData>>().AddTo(Disposables);
            CacheSize = new ReactiveProperty<int>().AddTo(Disposables);

            ResourceCache.Subscribe(OnResourceCacheChanged).AddTo(Disposables);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            _model.Initialize();
            UpdateResourceState();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void UpdateResource()
        {
            _model.Update();
            UpdateResourceState();
        }

        /// <summary>
        /// リソースロード
        /// </summary>
        public async Task<ResourceData?> LoadResource(string path)
        {
            var res = await _model.LoadResource(path);
            UpdateResourceState();
            return res;
        }

        /// <summary>
        /// アンロード
        /// </summary>
        public void UnloadResource(string path)
        {
            _model.UnloadResource(path);
            UpdateResourceState();
        }

        /// <summary>
        /// 取得
        /// </summary>
        public ResourceData? GetResource(string path)
        {
            return _model.GetResource(path);
        }

        private void UpdateResourceState()
        {
            ResourceCache.Value = _model.ResourceCache;
            CacheSize.Value = _model.CurrentCacheSize;
        }

        private void OnResourceCacheChanged(Dictionary<string, ResourceData> cache)
        {
            EventBus.Publish(new ResourceCacheChangedEvent(cache));
        }
    }
}
