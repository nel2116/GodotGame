using System;
using Godot;

namespace Systems.Common.Movement
{
    /// <summary>
    /// 移動システムのインターフェース
    /// </summary>
    public interface IMovementSystem : IDisposable
    {
        /// <summary>
        /// システムを初期化する
        /// </summary>
        void Initialize();

        /// <summary>
        /// 毎フレーム更新処理を行う
        /// </summary>
        void Update();

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="direction">移動方向</param>
        void Move(Vector2 direction);

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        void Jump();

        /// <summary>
        /// ダッシュ処理
        /// </summary>
        void Dash();
    }
}
