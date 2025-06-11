using System;

namespace Systems.Player.Base
{
    /// <summary>
    /// プレイヤーシステムのインターフェース
    /// </summary>
    public interface IPlayerSystem : IDisposable
    {
        /// <summary>
        /// 初期化処理を行う
        /// </summary>
        void Initialize();

        /// <summary>
        /// 毎フレーム更新処理を行う
        /// </summary>
        void Update();
    }
}
