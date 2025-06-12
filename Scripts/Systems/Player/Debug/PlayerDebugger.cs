using Godot;
using Core.Events;
using Systems.Player.Input;
using Systems.Player.Movement;

namespace Systems.Player.Debug
{
    /// <summary>
    /// プレイヤーのデバッグ情報を管理するクラス
    /// </summary>
    public class PlayerDebugger
    {
        private readonly PlayerInputViewModel _input_vm;
        private readonly PlayerMovementViewModel _movement_vm;
        private bool _isEnabled;

        public PlayerDebugger(PlayerInputViewModel input_vm, PlayerMovementViewModel movement_vm)
        {
            _input_vm = input_vm;
            _movement_vm = movement_vm;
            _isEnabled = false;
        }

        /// <summary>
        /// デバッグ出力を有効/無効にします
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// デバッグ情報を出力します
        /// </summary>
        public void PrintDebugInfo()
        {
            if (!_isEnabled) return;

            GD.Print($"Input State: {_input_vm.CurrentState.Value.MovementInput}");
            GD.Print($"Velocity 2D: {_movement_vm.Velocity.Value}");
            GD.Print($"Is Grounded: {_movement_vm.IsGrounded.Value}");
            GD.Print($"Final Velocity 3D: {_movement_vm.Model.VerticalVelocity}");
        }
    }
} 