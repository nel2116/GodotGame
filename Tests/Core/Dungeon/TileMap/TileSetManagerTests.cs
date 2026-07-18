using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.TileMap;
using TileType = Systems.Dungeon.TileMap.TileType;

namespace Tests.Core.Dungeon.TileMap
{
    public class TileSetManagerTests
    {
        [Test]
        public void GetTemplate_InjectedMapping_ReturnsInjectedTemplate()
        {
            var injected = new Dictionary<TileType, TileTemplate>
            {
                [TileType.Floor] = new TileTemplate { Type = TileType.Floor, SourceId = 7, AtlasCoords = new Vector2I(3, 4) }
            };
            var manager = new TileSetManager(injected);

            var template = manager.GetTemplate(TileType.Floor);

            Assert.AreEqual(7, template.SourceId);
            Assert.AreEqual(new Vector2I(3, 4), template.AtlasCoords);
        }

        [Test]
        public void GetTemplate_DefaultMapping_ReturnsTemplateForEveryTileType()
        {
            var manager = new TileSetManager();

            foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
            {
                var template = manager.GetTemplate(type);
                Assert.AreEqual(type, template.Type);
            }
        }

        [Test]
        public void GetTemplate_UnregisteredType_Throws()
        {
            var empty = new Dictionary<TileType, TileTemplate>();
            var manager = new TileSetManager(empty);

            Assert.Throws<KeyNotFoundException>(() => manager.GetTemplate(TileType.Floor));
        }

        [Test]
        public void GetTemplate_DefaultMapping_SecretWallLooksLikeWall()
        {
            var manager = new TileSetManager();

            var wall = manager.GetTemplate(TileType.Wall);
            var secretWall = manager.GetTemplate(TileType.SecretWall);

            Assert.AreEqual(wall.SourceId, secretWall.SourceId);
            Assert.AreEqual(wall.AtlasCoords, secretWall.AtlasCoords);
        }
    }
}
