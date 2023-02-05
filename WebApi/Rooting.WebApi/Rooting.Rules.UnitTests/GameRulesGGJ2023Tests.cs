using Newtonsoft.Json;
using Rooting.Models;
using Rooting.Models.ResponseModels;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class GameRulesGGJ2023Tests
    {
        private readonly IGameDefinitionFactory _factory = new GameDefinitionFactory();
        private readonly IGameEngine _engine = new GameRulesGGJ2023();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private GameStatistics _game;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [TestInitialize]
        public void TestInit()
        {
            _game = new GameStatistics(_factory, _engine);
        }

        [TestMethod]
        public void TestColonizationRule()
        {
            var o = new Origin(2, 2);
            var map = _game.WorldMap;
            _engine.ApplyRule("plantRuleColonization", o, map);

            var tile = _game.WorldMap.Tile(o);
            Assert.IsNotNull(tile);
            Console.WriteLine(JsonConvert.SerializeObject(tile, Formatting.Indented));
            Assert.AreEqual(FamilyTypes.Plant, tile.FamilyType);
            Assert.AreEqual(10, tile.ScoringClass.FamilyScore[FamilyTypes.Plant]);
            Assert.IsFalse(tile.ScoringClass.Tokens.Any(m => m.Family == FamilyTypes.Plant));
        }

        [TestMethod]
        public void TestColonizationRuleTwice()
        {
            var o = new Origin(2, 2);
            var map = _game.WorldMap;
            _engine.ApplyRule("plantRuleColonization", o, map);
            _engine.ApplyRule("plantRuleColonization", o, map);

            var tile = _game.WorldMap.Tile(o);
            Assert.IsNotNull(tile);
            Console.WriteLine(JsonConvert.SerializeObject(tile, Formatting.Indented));
            Assert.AreEqual(FamilyTypes.Plant, tile.FamilyType);
            Assert.AreEqual(21, tile.ScoringClass.FamilyScore[FamilyTypes.Plant]);
            Assert.IsNotNull(tile.ScoringClass.Tokens.Single(m => m.Family == FamilyTypes.Plant && m.Token == TokenType.Village));
        }

        [TestMethod]
        public void TestFloweringBloomRule()
        {
            var o = new Origin(2, 1);
            var map = _game.WorldMap;
            _engine.ApplyRule("plantRuleFloweringBloom", o, map);

            var tile = _game.WorldMap.Tile(o);
            Assert.IsNotNull(tile);
            Console.WriteLine(JsonConvert.SerializeObject(tile, Formatting.Indented));
            Assert.AreEqual(FamilyTypes.All, tile.FamilyType);

            var o2 = new Origin(2, 2);
            var otherTile = _game.WorldMap.Tile(o2);
            Assert.IsNotNull(otherTile);
            Console.WriteLine(JsonConvert.SerializeObject(otherTile, Formatting.Indented));
            Assert.AreEqual(FamilyTypes.Plant, otherTile.FamilyType);
            Assert.IsNull(otherTile.ScoringClass.Tokens.FirstOrDefault(m => m.Family == FamilyTypes.Plant && m.Token == TokenType.Village));
        }

        //plantRuleFloweringBloom
    }
}