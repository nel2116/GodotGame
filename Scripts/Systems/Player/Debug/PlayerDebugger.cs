using Systems.Player.Input;
using Systems.Player.Movement;

namespace Systems.Player.Debug
{
    /// <summary>
    /// プレイヤー用の簡易デバッグユーティリティ。
    /// </summary>
    public class PlayerDebugger
    {
        private readonly PlayerInputViewModel _inputViewModel;
        private readonly PlayerMovementViewModel _movementViewModel;
        private bool _isEnabled;

        public PlayerDebugger(PlayerInputViewModel inputViewModel, PlayerMovementViewModel movementViewModel)
        {
            _inputViewModel = inputViewModel;
            _movementViewModel = movementViewModel;
        }

        /// <summary>
        /// デバッグ出力の有効無効を設定する。
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// 入力と移動の状態を出力する。
        /// </summary>
        public void PrintDebugInfo()
        {
            if (!_isEnabled)
            {
                return;
            }

            LogPlayerState();
        }

        /// <summary>
        /// 実際のログ出力処理（必要時にコメントを外して使用）。
        /// </summary>
        public void LogPlayerState()
        {
            if (!_isEnabled)
            {
                return;
            }

            // 高頻度なログはパフォーマンスを下げるため、必要なときだけコメントを外す。
            // GD.Print($"Input State: {_inputViewModel.CurrentState.Value.MovementInput}");
            // GD.Print($"Velocity 2D: {_movementViewModel.Velocity.Value}");
            // GD.Print($"Is Grounded: {_movementViewModel.IsGrounded.Value}");
            // GD.Print($"Final Velocity 3D: {_movementViewModel.Model.VerticalVelocity}");
        }
    }
}
