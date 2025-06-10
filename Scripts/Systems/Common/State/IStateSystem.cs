using System;
using System.Collections.Generic;

namespace Systems.Common.State
{
    /// <summary>
    /// 状態システムのインターフェース
    /// </summary>
    public interface IStateSystem : IDisposable
    {
        /// <summary>
        /// システム初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 毎フレームの更新処理
        /// </summary>
        void Update();

        /// <summary>
        /// 状態を変更
        /// </summary>
        void ChangeState(string newState);

        /// <summary>
        /// 現在の状態
        /// </summary>
        string CurrentState { get; }

        /// <summary>
        /// 状態遷移表
        /// </summary>
        Dictionary<string, string> StateTransitions { get; }

        /// <summary>
        /// 遷移可能か
        /// </summary>
        bool CanTransitionTo(string newState);
    }
}
