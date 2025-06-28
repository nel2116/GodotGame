using Godot;
using Systems.Player.Input;
using Systems.Player.Movement;
using Core.Utilities;

namespace Systems.Player.Debug
{
    /// <summary>
    /// プレイヤーデバッガー
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
        /// デバッグモードを設定
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// デバッグ情報を出力
        /// </summary>
        public void PrintDebugInfo()
        {
            if (!_isEnabled) return;

            GodotMock.Print($"Input State: {_input_vm.CurrentState.Value.MovementInput}");
            GodotMock.Print($"Velocity 2D: {_movement_vm.Velocity.Value}");
            GodotMock.Print($"Is Grounded: {_movement_vm.IsGrounded.Value}");
            GodotMock.Print($"Final Velocity 3D: {_movement_vm.Model.VerticalVelocity}");
        }
    }
} 