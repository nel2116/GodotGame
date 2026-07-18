using Core.Events;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Events
{
    /// <summary>
    /// レベル生成完了イベント
    /// ダンジョン全体の部屋生成・ギミック配置・ナビゲーションメッシュ構築が完了した際に発行される
    /// </summary>
    public class LevelGeneratedEvent : GameEvent
    {
        /// <summary>
        /// 生成された部屋数
        /// </summary>
        public int RoomCount { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="roomCount">生成された部屋数</param>
        public LevelGeneratedEvent(int roomCount)
        {
            RoomCount = roomCount;
        }
    }

    /// <summary>
    /// 部屋入室イベント
    /// プレイヤーが部屋に入室した際に発行される
    /// </summary>
    public class RoomEnteredEvent : GameEvent
    {
        /// <summary>
        /// 入室した部屋の位置
        /// </summary>
        public Vector2I RoomPosition { get; }

        /// <summary>
        /// 入室した部屋の種類
        /// </summary>
        public RoomType RoomType { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="roomPosition">入室した部屋の位置</param>
        /// <param name="roomType">入室した部屋の種類</param>
        public RoomEnteredEvent(Vector2I roomPosition, RoomType roomType)
        {
            RoomPosition = roomPosition;
            RoomType = roomType;
        }
    }

    /// <summary>
    /// 隠し通路発見イベント
    /// 隠し通路ギミックが発見・開通した際に発行される
    /// </summary>
    public class HiddenPassageRevealedEvent : GameEvent
    {
        /// <summary>
        /// ギミックが属する部屋の位置
        /// </summary>
        public Vector2I RoomPosition { get; }

        /// <summary>
        /// 発動したギミックの位置
        /// </summary>
        public Vector2I GimmickPosition { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動したギミックの位置</param>
        public HiddenPassageRevealedEvent(Vector2I roomPosition, Vector2I gimmickPosition)
        {
            RoomPosition = roomPosition;
            GimmickPosition = gimmickPosition;
        }
    }

    /// <summary>
    /// 鍵扉解錠イベント
    /// 鍵扉ギミックが解錠された際に発行される
    /// </summary>
    public class LockedDoorUnlockedEvent : GameEvent
    {
        /// <summary>
        /// ギミックが属する部屋の位置
        /// </summary>
        public Vector2I RoomPosition { get; }

        /// <summary>
        /// 発動したギミックの位置
        /// </summary>
        public Vector2I GimmickPosition { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動したギミックの位置</param>
        public LockedDoorUnlockedEvent(Vector2I roomPosition, Vector2I gimmickPosition)
        {
            RoomPosition = roomPosition;
            GimmickPosition = gimmickPosition;
        }
    }

    /// <summary>
    /// ギミック発動失敗イベント
    /// ギミックの発動条件を満たさなかった場合（既に発動済み・鍵不足など）に発行される
    /// </summary>
    public class GimmickActivationFailedEvent : GameEvent
    {
        /// <summary>
        /// ギミックが属する部屋の位置
        /// </summary>
        public Vector2I RoomPosition { get; }

        /// <summary>
        /// 発動に失敗したギミックの位置
        /// </summary>
        public Vector2I GimmickPosition { get; }

        /// <summary>
        /// 発動に失敗したギミックの種類
        /// </summary>
        public GimmickType GimmickType { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動に失敗したギミックの位置</param>
        /// <param name="gimmickType">発動に失敗したギミックの種類</param>
        public GimmickActivationFailedEvent(Vector2I roomPosition, Vector2I gimmickPosition, GimmickType gimmickType)
        {
            RoomPosition = roomPosition;
            GimmickPosition = gimmickPosition;
            GimmickType = gimmickType;
        }
    }
}
