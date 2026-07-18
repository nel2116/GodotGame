using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// 部屋読み込み同期結果
    /// 直近の <see cref="RoomLifecycleManager.SyncActiveRooms"/> 呼び出しで
    /// 新たに読み込まれた部屋・解放された部屋をまとめて表す（イベントのバッチ発行に使用する）
    /// </summary>
    /// <param name="Loaded">今回新たにタイルマップへ反映した部屋位置の一覧</param>
    /// <param name="Unloaded">今回新たにタイルマップから消去した部屋位置の一覧</param>
    public record RoomSyncResult(IReadOnlyList<Vector2I> Loaded, IReadOnlyList<Vector2I> Unloaded);

    /// <summary>
    /// 部屋ライフサイクル管理
    /// 「アクティブな部屋集合」と「現在タイルマップへ反映済みの部屋集合」の差分を取り、
    /// 新規にアクティブになった部屋の読み込み、非アクティブになった部屋の解放を <see cref="IRoomTileRenderer"/> 経由で行う
    /// </summary>
    public class RoomLifecycleManager
    {
        private readonly HashSet<Vector2I> loadedRooms = new();

        /// <summary>
        /// 現在タイルマップへ反映済みの部屋位置の集合
        /// </summary>
        public IReadOnlySet<Vector2I> LoadedRooms => loadedRooms;

        /// <summary>
        /// アクティブな部屋集合に合わせて、部屋の読み込み・解放を同期する
        /// </summary>
        /// <param name="activeRooms">アクティブにすべき部屋位置の集合</param>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="templates">部屋テンプレートの辞書（部屋位置がキー）</param>
        /// <param name="renderer">部屋タイル描画器</param>
        /// <returns>今回読み込んだ部屋・解放した部屋の一覧</returns>
        public RoomSyncResult SyncActiveRooms(
            IReadOnlySet<Vector2I> activeRooms,
            Dictionary<Vector2I, RoomData> rooms,
            IReadOnlyDictionary<Vector2I, RoomTemplate> templates,
            IRoomTileRenderer renderer)
        {
            var loaded = new List<Vector2I>();
            var unloaded = new List<Vector2I>();

            foreach (var position in activeRooms)
            {
                if (loadedRooms.Contains(position))
                {
                    continue;
                }

                if (!rooms.TryGetValue(position, out var room) || !templates.TryGetValue(position, out var template))
                {
                    continue;
                }

                renderer.ApplyRoom(room, template);
                loadedRooms.Add(position);
                loaded.Add(position);
            }

            foreach (var position in new List<Vector2I>(loadedRooms))
            {
                if (activeRooms.Contains(position))
                {
                    continue;
                }

                if (rooms.TryGetValue(position, out var room))
                {
                    renderer.ClearRoom(room);
                }

                loadedRooms.Remove(position);
                unloaded.Add(position);
            }

            return new RoomSyncResult(loaded, unloaded);
        }
    }
}
