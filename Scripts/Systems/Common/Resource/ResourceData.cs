using System;

namespace Systems.Common.Resource
{
    /// <summary>
    /// リソースデータ
    /// </summary>
    public class ResourceData : IDisposable
    {
        public int Size { get; init; }
        public bool IsInUse { get; set; }
        public DateTime LastAccessTime { get; private set; } = DateTime.UtcNow;

        public void Touch()
        {
            LastAccessTime = DateTime.UtcNow;
        }

        public void Dispose()
        {
            // リソース解放処理
        }
    }
}
