using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;

namespace Tests.Core.Dungeon.Navigation
{
    public class NavigationManagerTests
    {
        private static readonly Vector2I RoomAPosition = new(0, 0);
        private static readonly Vector2I RoomBPosition = new(16, 0);
        private static readonly Vector2I DoorAPosition = new(15, 8);
        private static readonly Vector2I DoorBPosition = new(16, 8);

        /// <summary>
        /// 鍵扉ギミック付きで接続された 2 部屋（部屋テンプレート付き）を作成する
        /// </summary>
        private static (Dictionary<Vector2I, RoomData> Rooms, Dictionary<Vector2I, RoomTemplate> Templates) CreateRoomsWithLockedDoor()
        {
            var roomA = new RoomData { Position = RoomAPosition, Type = RoomType.Combat };
            var roomB = new RoomData { Position = RoomBPosition, Type = RoomType.Treasure };

            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = DoorType.Locked, IsLocked = true, ConnectedRoomPosition = RoomBPosition });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = DoorType.Locked, IsLocked = true, ConnectedRoomPosition = RoomAPosition });

            roomA.AddGimmick(new GimmickData { Position = DoorAPosition, Type = GimmickType.LockedDoor, IsActive = false });
            roomB.AddGimmick(new GimmickData { Position = DoorBPosition, Type = GimmickType.LockedDoor, IsActive = false });

            var rooms = new Dictionary<Vector2I, RoomData> { [RoomAPosition] = roomA, [RoomBPosition] = roomB };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [RoomAPosition] = new RoomTemplate { Type = RoomType.Combat },
                [RoomBPosition] = new RoomTemplate { Type = RoomType.Treasure }
            };

            return (rooms, templates);
        }

        [Test]
        public void FindPath_AcrossConnectedRooms_ReturnsPathThroughDoor()
        {
            var roomA = new RoomData { Position = RoomAPosition, Type = RoomType.Combat };
            var roomB = new RoomData { Position = RoomBPosition, Type = RoomType.Combat };
            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = DoorType.Normal, IsLocked = false, ConnectedRoomPosition = RoomBPosition });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = DoorType.Normal, IsLocked = false, ConnectedRoomPosition = RoomAPosition });

            var rooms = new Dictionary<Vector2I, RoomData> { [RoomAPosition] = roomA, [RoomBPosition] = roomB };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [RoomAPosition] = new RoomTemplate { Type = RoomType.Combat },
                [RoomBPosition] = new RoomTemplate { Type = RoomType.Combat }
            };

            var manager = new NavigationManager();
            manager.BuildMesh(rooms, templates);

            var start = RoomAPosition + new Vector2I(1, 1);
            var goal = RoomBPosition + new Vector2I(1, 1);
            var path = manager.FindPath(start, goal);

            Assert.IsNotEmpty(path);
            Assert.AreEqual(start, path[0]);
            Assert.AreEqual(goal, path[^1]);
            Assert.Contains(DoorAPosition, path);
            Assert.Contains(DoorBPosition, path);
        }

        [Test]
        public void BuildMesh_AfterLockedDoorGimmickActivated_PathBecomesAvailable()
        {
            var (rooms, templates) = CreateRoomsWithLockedDoor();
            var manager = new NavigationManager();
            manager.BuildMesh(rooms, templates);

            var start = RoomAPosition + new Vector2I(1, 1);
            var goal = RoomBPosition + new Vector2I(1, 1);

            // 施錠されている間は経路が存在しないこと
            Assert.IsEmpty(manager.FindPath(start, goal));

            // 鍵扉ギミックを発動（解錠）した後、メッシュを再構築すると経路が通ること
            var activator = new GimmickActivator();
            bool activated = activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: true);
            Assert.IsTrue(activated);

            manager.BuildMesh(rooms, templates);
            var pathAfterUnlock = manager.FindPath(start, goal);

            Assert.IsNotEmpty(pathAfterUnlock);
            Assert.AreEqual(start, pathAfterUnlock[0]);
            Assert.AreEqual(goal, pathAfterUnlock[^1]);
        }

        [Test]
        public void RebuildRooms_AfterLockedDoorGimmickActivated_PathBecomesAvailableWithoutFullRebuild()
        {
            var (rooms, templates) = CreateRoomsWithLockedDoor();
            var manager = new NavigationManager();
            manager.BuildMesh(rooms, templates);

            var start = RoomAPosition + new Vector2I(1, 1);
            var goal = RoomBPosition + new Vector2I(1, 1);
            Assert.IsEmpty(manager.FindPath(start, goal));

            var activator = new GimmickActivator();
            bool activated = activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: true);
            Assert.IsTrue(activated);

            // 全体再構築（BuildMesh）ではなく、影響を受けた 2 部屋のみの部分再構築で経路が通ること
            manager.RebuildRooms(new[] { RoomAPosition, RoomBPosition }, rooms, templates);
            var pathAfterUnlock = manager.FindPath(start, goal);

            Assert.IsNotEmpty(pathAfterUnlock);
            Assert.AreEqual(start, pathAfterUnlock[0]);
            Assert.AreEqual(goal, pathAfterUnlock[^1]);
        }

        [Test]
        public void RebuildRooms_UnknownPosition_IsSkippedWithoutError()
        {
            var (rooms, templates) = CreateRoomsWithLockedDoor();
            var manager = new NavigationManager();
            manager.BuildMesh(rooms, templates);

            Assert.DoesNotThrow(() => manager.RebuildRooms(new[] { new Vector2I(999, 999) }, rooms, templates));
        }
    }
}
