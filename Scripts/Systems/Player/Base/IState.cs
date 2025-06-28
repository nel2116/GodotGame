namespace Systems.Player.Base
{
    /// <summary>
    /// プレイヤー状態インターフェース
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 状態開始時に呼ばれる
        /// </summary>
        void Enter();

        /// <summary>
        /// 状態更新処理
        /// </summary>
        void Update();

        /// <summary>
        /// 状態終了時に呼ばれる
        /// </summary>
        void Exit();
    }
}
