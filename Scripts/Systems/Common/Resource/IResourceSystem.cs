using System;
using System.Threading.Tasks;

namespace Systems.Common.Resource
{
    /// <summary>
    /// リソースシステムのインターフェース
    /// </summary>
    public interface IResourceSystem : IDisposable
    {
        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update();

        /// <summary>
        /// リソースをロード
        /// </summary>
        Task<ResourceData?> LoadResource(string path);

        /// <summary>
        /// リソースをアンロード
        /// </summary>
        void UnloadResource(string path);

        /// <summary>
        /// リソース取得
        /// </summary>
        ResourceData? GetResource(string path);
    }
}
