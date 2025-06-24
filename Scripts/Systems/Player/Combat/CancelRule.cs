using System.Collections.Generic;
using System.Linq;

namespace Systems.Player.Combat
{
    /// <summary>
    /// キャンセルルールを定義するクラス
    /// </summary>
    public class CancelRule
    {
        public string FromAction { get; }
        public int StartFrame { get; }
        public int EndFrame { get; }
        public List<string> AllowedActions { get; }
        public int Priority { get; }

        public CancelRule(string fromAction, int startFrame, int endFrame, IEnumerable<string> allowedActions, int priority)
        {
            FromAction = fromAction;
            StartFrame = startFrame;
            EndFrame = endFrame;
            AllowedActions = allowedActions.ToList();
            Priority = priority;
        }

        /// <summary>
        /// キャンセル可能か判定する
        /// </summary>
        public bool CanCancel(string toAction, int frameOffset)
        {
            return frameOffset >= StartFrame && frameOffset <= EndFrame && AllowedActions.Contains(toAction);
        }
    }
}
