using System;

namespace Core.Interfaces
{
    /// <summary>
    /// 毎フレーム更新処理を提供するインターフェース
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// 毎フレームの更新処理
        /// </summary>
        void Update();
    }
}
