using System.Linq;
using Core.Events;
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

        public InputBuffer()
        {
        }

        /// <summary>
        /// アクション入力をバッファする
        /// </summary>
        public void BufferAction(string action)
        {
            _action_buffer.Add(action);
        }

        /// <summary>
        /// 移動入力をバッファする
        /// </summary>
        public void BufferMovement(Vector2 dir)
        {
            _movement_buffer.Add(dir);
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
                    "Dash" => 0,
                    "Jump" => 1,
                    "Attack" => 2,
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
