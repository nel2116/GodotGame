using Godot;

namespace Systems.Dungeon.Navigation
{
    /// <summary>
    /// ナビゲーションノード
    /// A* 探索における探索過程の状態（位置・コスト・経路復元用の親ノード）を保持する
    /// プレーンなデータ保持クラスであり、A* アルゴリズム自体は <see cref="PathFinder"/> に実装する
    /// </summary>
    public class NavigationNode
    {
        /// <summary>
        /// ノードのワールドタイル座標
        /// </summary>
        public Vector2I Position { get; set; }

        /// <summary>
        /// 開始地点からこのノードまでの実コスト
        /// </summary>
        public float GCost { get; set; }

        /// <summary>
        /// このノードから目標地点までのヒューリスティックコスト（マンハッタン距離を想定）
        /// </summary>
        public float HCost { get; set; }

        /// <summary>
        /// 総推定コスト（GCost + HCost）
        /// </summary>
        public float FCost => GCost + HCost;

        /// <summary>
        /// 経路復元に使用する直前のノード（開始ノードの場合は null）
        /// </summary>
        public NavigationNode? Parent { get; set; }
    }
}
