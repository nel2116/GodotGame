using System.Linq;
using Godot;
namespace Systems.Player.Input
{
    /// <summary>
    /// 入力バッファリングシステム
    /// </summary>
    public class InputBuffer
    {
        private readonly InputRingBuffer<string> _action_buffer = new(12);
        private readonly InputRingBuffer<Vector2> _movement_buffer = new(12);

        private const string DashAction = PlayerInputActionNames.Dash;
        private const string JumpAction = PlayerInputActionNames.Jump;
        private const string AttackAction = PlayerInputActionNames.Attack;

        public void CollectInputState(InputState state)
        {
            if (state.IsButtonPressed(DashAction))
            {
                _action_buffer.Add(DashAction);
            }

            if (state.IsButtonPressed(JumpAction))
            {
                _action_buffer.Add(JumpAction);
            }

            if (state.IsButtonPressed(AttackAction))
            {
                _action_buffer.Add(AttackAction);
            }

            if (state.MovementInput != Vector2.Zero)
            {
                _movement_buffer.Add(state.MovementInput);
            }
        }

        /// <summary>
        /// 優先度順にアクションを取得する
        /// </summary>
        public string? PopAction()
        {
            string? selected = null;
            var best = int.MaxValue;
            foreach (var a in _action_buffer.GetItems())
            {
                var priority = a switch
                {
                    DashAction => 0,
                    JumpAction => 1,
                    AttackAction => 2,
                    _ => int.MaxValue
                };
                if (priority < best)
                {
                    selected = a;
                    best = priority;
                }
            }

            if (selected != null)
            {
                _action_buffer.Clear();
            }
            return selected;
        }

        /// <summary>
        /// 最新の移動入力を取得する
        /// </summary>
        public Vector2 GetMovement()
        {
            return _movement_buffer.GetItems().LastOrDefault();
        }
    }
}
