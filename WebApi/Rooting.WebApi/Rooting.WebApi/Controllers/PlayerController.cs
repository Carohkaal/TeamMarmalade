using Microsoft.AspNetCore.Mvc;
using Rooting.Models;

namespace Rooting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> logger;
        private readonly GameStatistics gameStatistics;

        public GameController(ILogger<GameController> logger, GameStatistics gameStatistics)
        {
            this.logger = logger;
            this.gameStatistics = gameStatistics;
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

        [HttpGet("CurrentPlayers")]
        public IEnumerable<PlayerModel> Get()
        {
            return gameStatistics.Players.Select(m => new PlayerModel
            {
                Uuid = m.Uuid,
                Name = m.Name,
                Avatar = m.Avatar,
                FamilyType = m.FamilyType
            });
        }

        [HttpPost("ClaimFamily")]
        public PlayerModel ClaimPlayer(PlayerModel player)
        {
            var client = Request.HttpContext.Connection.RemotePort.ToString() ?? "0";
            var id = GetPlayerId(player.FamilyType);
            player.Uuid = id;
            return gameStatistics.ClaimPlayer(player, client);
        }

        [HttpPut("Player/{id}")]
        public ActionResult<PlayerModel> UpdatePlayer(string id, [FromBody] PlayerModel player)
        {
            var client = Request.HttpContext.Connection.RemotePort.ToString() ?? "0";
            if (!Guid.TryParse(id, out var playerId))
            {
                logger.LogWarning($"Invalid id used in UpdatePlayer: '{id}'");
                return BadRequest("id is not a valid player id");
            }
            var playerData = gameStatistics.Player(playerId);
            if (playerData == null)
            {
                return NotFound();
            }
            if (playerData.RemoteIp != client)
            {
                logger.LogError($"Client '{client}' tried to access user: '{id}'.");
                return Forbid("Detected invalid access.");
            }

            gameStatistics.UpdatePlayer(playerData.FamilyType, player.Name, player.Avatar);
            return player;
        }

        [HttpPost("ResetGame")]
        public Guid ResetGame()
        {
            gameStatistics.ResetGame();
            return gameStatistics.GameId;
        }

        private static Guid GetPlayerId(FamilyTypes type) => type switch
        {
            FamilyTypes.Tree => Constants.Player1,
            FamilyTypes.Animal => Constants.Player2,
            FamilyTypes.Fungi => Constants.Player3,
            FamilyTypes.None => Guid.Empty,
            _ => Guid.Empty
        };
    }
}