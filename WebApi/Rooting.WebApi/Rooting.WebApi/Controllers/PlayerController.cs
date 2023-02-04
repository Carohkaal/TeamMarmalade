using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using System.Collections.Concurrent;

namespace Rooting.WebApi.Controllers
{
    public static class Constants
    {
        public static Guid Player1 => new Guid("AFCAE5C4-278F-4411-B4F2-6C0785E11E0A");
        public static Guid Player2 => new Guid("BFCAE5C4-278F-4411-B4F2-6C0785E11E0B");
        public static Guid Player3 => new Guid("CFCAE5C4-278F-4411-B4F2-6C0785E11E0C");
    }

    public class GameStatistics : IGameStatistics
    {
        private Guid gameId = Guid.NewGuid();
        private readonly ConcurrentDictionary<FamilyTypes, Player> activePlayers = new();
        public Guid GameId => gameId;
        public int Generation { get; private set; }
        public DateTime TimeStarted { get; private set; }
        public bool GameStarted { get; private set; }
        public IEnumerable<Player> Players => activePlayers.Values;

        public Player ClaimPlayer(Player player)
        {
            if (activePlayers.TryAdd(player.FamilyType, player))
            {
                player.Message = $"Claimed {player.FamilyType}";
            }
            else
            {
                player.Uuid = Guid.Empty;
                player.Message = $"Someone already claimed {player.FamilyType}";
            }
            return player;
        }

        public void ResetGame()
        {
            gameId = Guid.NewGuid();
            Generation = 0;
            TimeStarted = DateTime.MinValue;
            GameStarted = false;
            activePlayers.Clear();
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> logger;
        private readonly GameStatistics gameStatistics;

        public PlayerController(ILogger<PlayerController> logger, GameStatistics gameStatistics)
        {
            this.logger = logger;
            this.gameStatistics = gameStatistics;
        }

        [HttpGet("Statistics")]
        public ActionResult<GameStatistics> GameStatistics(Guid gameId)
        {
            if (gameId == gameStatistics.GameId) return gameStatistics;
            logger.LogWarning("Invalid game id requested");
            return NotFound();
        }

        [HttpGet("CurrentPlayers")]
        public IEnumerable<Player> Get()
        {
            return gameStatistics.Players;
        }

        [HttpPost("ClaimFamily")]
        public Player ClaimPlayer(Player player)
        {
            var id = GetPLayerId(player.FamilyType);
            player.Uuid = id;
            return gameStatistics.ClaimPlayer(player);
        }

        [HttpPost("ResetGame")]
        public Guid ResetGame()
        {
            gameStatistics.ResetGame();
            return gameStatistics.GameId;
        }

        private static Guid GetPLayerId(FamilyTypes type) => type switch
        {
            FamilyTypes.Tree => Constants.Player1,
            FamilyTypes.Animal => Constants.Player2,
            FamilyTypes.Fungi => Constants.Player3,
            FamilyTypes.None => Guid.Empty,
            _ => Guid.Empty
        };
    }
}