using NUnit.Framework;
using Godot;
using Systems.Dungeon.Utilities;

namespace Tests.Core.Dungeon
{
    public class DungeonUtilsTests
    {
        [Test]
        public void CalculateDistance_DifferentPositions_ReturnsPositiveValue()
        {
            // 3-4-5 の直角三角形で距離が 5 になること
            var distance = DungeonUtils.CalculateDistance(new Vector2I(0, 0), new Vector2I(3, 4));

            Assert.AreEqual(5.0f, distance, 0.0001f);
        }

        [Test]
        public void CalculateDistance_SamePosition_ReturnsZero()
        {
            var distance = DungeonUtils.CalculateDistance(new Vector2I(16, -16), new Vector2I(16, -16));

            Assert.AreEqual(0.0f, distance, 0.0001f);
        }

        [Test]
        public void CalculateDistance_IsSymmetric()
        {
            var pos1 = new Vector2I(-16, 32);
            var pos2 = new Vector2I(48, -48);

            // 引数の順序を入れ替えても同じ距離になること
            Assert.AreEqual(
                DungeonUtils.CalculateDistance(pos1, pos2),
                DungeonUtils.CalculateDistance(pos2, pos1),
                0.0001f);
        }

        [Test]
        public void CalculateRoomPosition_InsideFirstRoom_ReturnsOrigin()
        {
            // 部屋内部の座標は部屋の左上（原点）に丸められること
            var result = DungeonUtils.CalculateRoomPosition(new Vector2(15.9f, 0.5f));

            Assert.AreEqual(new Vector2I(0, 0), result);
        }

        [Test]
        public void CalculateRoomPosition_ExactRoomBoundary_ReturnsNextRoom()
        {
            // 部屋境界ちょうど（16.0）は次の部屋の開始位置になること
            var result = DungeonUtils.CalculateRoomPosition(new Vector2(16.0f, 16.0f));

            Assert.AreEqual(new Vector2I(16, 16), result);
        }

        [Test]
        public void CalculateRoomPosition_NegativeCoordinates_FloorsTowardsNegative()
        {
            // 負の座標は負方向に切り下げられること（-0.1 → -16）
            var result = DungeonUtils.CalculateRoomPosition(new Vector2(-0.1f, -16.0f));

            Assert.AreEqual(new Vector2I(-16, -16), result);
        }

        [Test]
        public void IsValidRoomPosition_AlignedAndInRange_ReturnsTrue()
        {
            // グリッドに整列し範囲内の位置は有効
            Assert.IsTrue(DungeonUtils.IsValidRoomPosition(new Vector2I(0, 0)));
            Assert.IsTrue(DungeonUtils.IsValidRoomPosition(new Vector2I(16, -32)));
            Assert.IsTrue(DungeonUtils.IsValidRoomPosition(new Vector2I(48, 48)));
            Assert.IsTrue(DungeonUtils.IsValidRoomPosition(new Vector2I(-48, -48)));
        }

        [Test]
        public void IsValidRoomPosition_NotAligned_ReturnsFalse()
        {
            // ROOM_SIZE の倍数でない位置は無効
            Assert.IsFalse(DungeonUtils.IsValidRoomPosition(new Vector2I(8, 0)));
            Assert.IsFalse(DungeonUtils.IsValidRoomPosition(new Vector2I(0, -1)));
        }

        [Test]
        public void IsValidRoomPosition_OutOfRange_ReturnsFalse()
        {
            // 配置範囲（±GENERATION_GRID_RANGE グリッド）を超える位置は無効
            Assert.IsFalse(DungeonUtils.IsValidRoomPosition(new Vector2I(64, 0)));
            Assert.IsFalse(DungeonUtils.IsValidRoomPosition(new Vector2I(0, -64)));
        }
    }
}
