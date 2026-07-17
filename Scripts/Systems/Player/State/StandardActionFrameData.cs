namespace Systems.Player.State
{
    /// <summary>
    /// 企画仕様（プレイヤーアクション・フレーム表）に基づく標準アクションデータのファクトリ
    /// </summary>
    public static class StandardActionFrameData
    {
        public const string AttackL1Name = "Attack_L1";
        public const string AttackL2Name = "Attack_L2";
        public const string ChargeAttackName = "ChargeAttack";
        public const string DodgeName = "Dodge";
        public const string JumpName = "Jump";

        /// <summary>
        /// 通常攻撃_L1（総20F/発生4F/持続4F/硬直12F、移動距離0.3m）
        /// </summary>
        public static ActionFrameData AttackL1() => new ActionFrameData(
            AttackL1Name,
            totalFrames: 20,
            startupFrames: 4,
            activeFrames: 4,
            recoveryFrames: 12,
            movementDistance: 0.3f,
            cancelableTo: new[] { DodgeName, ChargeAttackName },
            priority: 1);

        /// <summary>
        /// 通常攻撃_L2（総22F/発生3F/持続5F/硬直14F、移動距離0.35m）
        /// </summary>
        public static ActionFrameData AttackL2() => new ActionFrameData(
            AttackL2Name,
            totalFrames: 22,
            startupFrames: 3,
            activeFrames: 5,
            recoveryFrames: 14,
            movementDistance: 0.35f,
            cancelableTo: new[] { DodgeName, ChargeAttackName },
            priority: 1);

        /// <summary>
        /// チャージ攻撃（総40F/発生16F/持続6F/硬直18F、移動距離0.5m）
        /// </summary>
        public static ActionFrameData ChargeAttack() => new ActionFrameData(
            ChargeAttackName,
            totalFrames: 40,
            startupFrames: 16,
            activeFrames: 6,
            recoveryFrames: 18,
            movementDistance: 0.5f,
            cancelableTo: new[] { DodgeName },
            priority: 1);

        /// <summary>
        /// 回避ロール（総26F/発生1F/持続8F/硬直17F、3-10F完全無敵、移動距離3m）
        /// </summary>
        public static ActionFrameData Dodge() => new ActionFrameData(
            DodgeName,
            totalFrames: 26,
            startupFrames: 1,
            activeFrames: 8,
            recoveryFrames: 17,
            invincibilityStartFrame: 3,
            invincibilityEndFrame: 10,
            movementDistance: 3f,
            cancelableTo: new[] { AttackL1Name, ChargeAttackName },
            priority: 2);

        /// <summary>
        /// ジャンプ（総30F/発生2F、縦移動距離4m、空中制御率60%）
        /// </summary>
        public static ActionFrameData Jump() => new ActionFrameData(
            JumpName,
            totalFrames: 30,
            startupFrames: 2,
            activeFrames: 28,
            recoveryFrames: 0,
            movementDistance: 4f,
            airControlRate: 0.6f,
            cancelableTo: new[] { DodgeName },
            priority: 0);
    }
}
