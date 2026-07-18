using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;

namespace Systems.Dungeon.Navigation
{
    /// <summary>
    /// ナビゲーション管理
    /// <see cref="NavigationMesh"/> の生成・保持と経路探索の窓口となる
    /// レベル生成直後、およびギミック発動後（扉の状態が変わった後）に <see cref="BuildMesh"/> を呼び出す想定
    /// </summary>
    public class NavigationManager
    {
        /// <summary>
        /// 通行可能領域を保持するナビゲーションメッシュ
        /// </summary>
        private readonly NavigationMesh navigationMesh = new();

        /// <summary>
        /// ナビゲーションメッシュ上で経路探索を行う探索器
        /// </summary>
        private readonly PathFinder pathFinder;

        /// <summary>
        /// コンストラクタ
        /// 内部のナビゲーションメッシュと、それを参照する経路探索器を初期化する
        /// </summary>
        public NavigationManager()
        {
            pathFinder = new PathFinder(navigationMesh);
        }

        /// <summary>
        /// ナビゲーションメッシュを（再）構築する
        /// レベル生成直後、およびギミック発動後（扉の状態が変わった後）に呼び出すことで
        /// 通行可能領域を最新の状態に更新する
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomTemplates">部屋テンプレートの辞書（部屋位置がキー）</param>
        public void BuildMesh(Dictionary<Vector2I, RoomData> rooms, IReadOnlyDictionary<Vector2I, RoomTemplate> roomTemplates)
        {
            navigationMesh.Build(rooms, roomTemplates);
        }

        /// <summary>
        /// 指定した部屋のみナビゲーションメッシュを部分再構築する
        /// ギミック発動等で影響範囲（変化した部屋＋接続先部屋）が既知の場合、全体再構築（<see cref="BuildMesh"/>）の代わりに使用することで
        /// 更新コストを変化した部屋数に比例させる
        /// </summary>
        /// <param name="positions">再構築対象の部屋位置の一覧</param>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomTemplates">部屋テンプレートの辞書（部屋位置がキー）</param>
        public void RebuildRooms(IEnumerable<Vector2I> positions, Dictionary<Vector2I, RoomData> rooms, IReadOnlyDictionary<Vector2I, RoomTemplate> roomTemplates)
        {
            foreach (var position in positions)
            {
                if (!rooms.TryGetValue(position, out var room))
                {
                    continue;
                }

                roomTemplates.TryGetValue(position, out var template);
                navigationMesh.RebuildRoom(position, room, template);
            }
        }

        /// <summary>
        /// 現在のナビゲーションメッシュ上で開始地点から目標地点までの経路を探索する
        /// </summary>
        /// <param name="start">開始地点のワールドタイル座標</param>
        /// <param name="goal">目標地点のワールドタイル座標</param>
        /// <returns>開始地点から目標地点までの経路（両端を含む）。経路が存在しない場合は空リスト</returns>
        public List<Vector2I> FindPath(Vector2I start, Vector2I goal)
        {
            return pathFinder.FindPath(start, goal);
        }
    }
}
