using System.Collections.Generic;

namespace Systems.Player.Input
{
    /// <summary>
    /// プレイヤー入力で使用するアクション名と InputMap 名をまとめた定義。
    /// </summary>
    public static class PlayerInputActionNames
    {
        public const string Move = "Move";
        public const string Jump = "Jump";
        public const string Attack = "Attack";
        public const string Dash = "Dash";

        public static readonly IReadOnlyList<string> ButtonNames = new[]
        {
            Jump,
            Attack,
            Dash
        };

        public static class InputMap
        {
            public const string MoveLeft = "move_left";
            public const string MoveRight = "move_right";
            public const string MoveUp = "move_up";
            public const string MoveDown = "move_down";
            public const string Jump = "jump";
            public const string Attack = "attack";
            public const string Dash = "dash";
        }
    }
}
