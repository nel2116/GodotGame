using Godot;

namespace Systems.Player.Config
{
    /// <summary>
    /// プレイヤーの入力設定を管理するクラス
    /// </summary>
    public static class PlayerInputConfig
    {
        private static readonly Dictionary<string, (Key Primary, Key? Secondary)> _keyBindings = new()
        {
            { "jump", (Key.Space, null) },
            { "attack", (Key.Z, null) },
            { "dash", (Key.Shift, null) },
            { "ui_left", (Key.A, Key.Left) },
            { "ui_right", (Key.D, Key.Right) },
            { "ui_up", (Key.W, Key.Up) },
            { "ui_down", (Key.S, Key.Down) }
        };

        /// <summary>
        /// 入力設定を初期化します
        /// </summary>
        public static void Initialize()
        {
            foreach (var (actionName, (primary, secondary)) in _keyBindings)
            {
                if (!InputMap.HasAction(actionName))
                {
                    InputMap.AddAction(actionName);
                }

                // プライマリキーの設定
                var primaryEvent = new InputEventKey { Keycode = primary };
                if (!InputMap.ActionHasEvent(actionName, primaryEvent))
                {
                    InputMap.ActionAddEvent(actionName, primaryEvent);
                }

                // セカンダリキーの設定（存在する場合）
                if (secondary.HasValue)
                {
                    var secondaryEvent = new InputEventKey { Keycode = secondary.Value };
                    if (!InputMap.ActionHasEvent(actionName, secondaryEvent))
                    {
                        InputMap.ActionAddEvent(actionName, secondaryEvent);
                    }
                }
            }
        }

        /// <summary>
        /// キー設定を変更します
        /// </summary>
        /// <param name="actionName">アクション名</param>
        /// <param name="primary">プライマリキー</param>
        /// <param name="secondary">セカンダリキー（オプション）</param>
        public static void ChangeKeyBinding(string actionName, Key primary, Key? secondary = null)
        {
            if (!_keyBindings.ContainsKey(actionName))
            {
                throw new ArgumentException($"Invalid action name: {actionName}");
            }

            _keyBindings[actionName] = (primary, secondary);
            Initialize(); // 設定を再適用
        }
    }
} 