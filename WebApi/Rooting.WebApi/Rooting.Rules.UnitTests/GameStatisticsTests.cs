using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Rooting.Models;
using Rooting.Models.ResponseModels;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class GameStatisticsTests
    {
        private readonly IGameDefinitionFactory _factory = new GameDefinitionFactory();
        private readonly Mock<IGameEngine> _engine = new Mock<IGameEngine>();

        private readonly Player _fakePlayer = new Player
        {
            FamilyType = FamilyTypes.Plant,
            Name = "Fake",
            Uuid = Guid.NewGuid(),
        };

        [TestMethod]
        public void GameStatisticsCanInitialize()
        {
            var sut = new GameStatistics(_factory, _engine.Object);
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void NewGameStatisticsIsWaitingForPlayers()
        {
            var sut = new GameStatistics(_factory, _engine.Object);
            Assert.AreEqual(GameStatus.WaitingForPlayers, sut.CurrentGameStatus);
        }

        [TestMethod]
        public void NewGameStatisticsCannotStartWithoutPlayers()
        {
            var sut = new GameStatistics(_factory, _engine.Object);
            var result = sut.StartGame(_fakePlayer, true);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(GameStatus.GameWaitingForEndOfTurn, result.GameStatus);
        }
    }
}