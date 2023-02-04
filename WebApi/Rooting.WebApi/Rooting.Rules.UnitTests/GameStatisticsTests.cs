using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Rooting.Models;
using Rooting.Models.ResponseModels;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class GameStatisticsTests
    {
        private readonly IGameDefinitionFactory _factory = new GameDefinitionFactory();
        private readonly IGameEngine _engine = new GameRulesGGJ2023();

        private readonly PlayerModel _fakePlayerModel = new PlayerModel
        {
            FamilyType = FamilyTypes.Fungi,
            Name = "Fake",
            Uuid = Guid.NewGuid(),
        };

        private readonly Player _fakePlayer = new Player
        {
            FamilyType = FamilyTypes.Fungi,
            Name = "Fake",
            Uuid = Guid.NewGuid(),
        };

        [TestMethod]
        public void GameStatisticsCanInitialize()
        {
            var sut = new GameStatistics(_factory, _engine);
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void NewGameStatisticsIsWaitingForPlayers()
        {
            var sut = new GameStatistics(_factory, _engine);
            Assert.AreEqual(GameStatus.WaitingForPlayers, sut.CurrentGameStatus);
        }

        [TestMethod]
        public void NewGameStatisticsCannotStartWithoutPlayers()
        {
            var sut = new GameStatistics(_factory, _engine);

            Assert.ThrowsException<GameException>(() =>
            {
                var result = sut.StartGame(_fakePlayer, true);
            });
        }

        [TestMethod]
        public void NewGameStatisticsCanRegisterPlayers()
        {
            var sut = new GameStatistics(_factory, _engine);
            var player = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            Assert.IsNotNull(player);
            Assert.AreNotEqual(Guid.Empty, player.Uuid);
            Assert.AreEqual("Claimed Fungi", player.Message);
        }

        [TestMethod]
        public void NewGameStatisticsCanFindRegistsredPlayers()
        {
            var sut = new GameStatistics(_factory, _engine);
            var playerModel = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            var player = sut.Player(playerModel.Uuid);
            Assert.IsNotNull(player);
            Console.WriteLine(JsonConvert.SerializeObject(player, Formatting.Indented));
        }

        [TestMethod]
        public void NewGameStatisticsCanStartWithOnePlayers()
        {
            var sut = new GameStatistics(_factory, _engine);
            var playerModel = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            var player = sut.Player(playerModel.Uuid);
            Assert.IsNotNull(player);
            var result = sut.StartGame(player, true);
            Assert.IsNotNull(result);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.AreEqual("Game Started", result.Shout);
            Assert.IsTrue(result.NextTurn > DateTime.Now);
        }

        [TestMethod]
        public void NewGameAfterStartupPlayerIsPlaying()
        {
            var sut = new GameStatistics(_factory, _engine);
            var playerModel = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            var player = sut.Player(playerModel.Uuid);
            Assert.IsNotNull(player);
            _ = sut.StartGame(player, true);
            Assert.IsTrue(sut.IsPlayerPlaying(player.FamilyType));
        }

        [TestMethod]
        public void NewGameAfterStartupPlayerHasCards()
        {
            var sut = new GameStatistics(_factory, _engine);
            var playerModel = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            var player = sut.Player(playerModel.Uuid);
            Assert.IsNotNull(player);
            var result = sut.StartGame(player, true);
            Assert.IsNotNull(result);

            var hand = sut.CurrentInHand(player.FamilyType);
            Assert.IsNotNull(hand);
            Console.WriteLine(JsonConvert.SerializeObject(hand, Formatting.Indented));
            Assert.IsTrue(hand.Length > 0);
        }

        [TestMethod]
        public void NewGameStatisticsCannotRestartWhenStarted()
        {
            var sut = new GameStatistics(_factory, _engine);
            var playerModel = sut.ClaimPlayer(_fakePlayerModel, "REMOTE");
            var player = sut.Player(playerModel.Uuid);
            Assert.IsNotNull(player);
            _ = sut.StartGame(player, true);
            var result2 = sut.StartGame(player, true);
            Assert.IsNotNull(result2);
            Console.WriteLine(JsonConvert.SerializeObject(result2, Formatting.Indented));
            Assert.AreNotEqual("Game Started", result2.Shout);
            Assert.IsTrue(result2.NextTurn > DateTime.Now);
        }
    }
}