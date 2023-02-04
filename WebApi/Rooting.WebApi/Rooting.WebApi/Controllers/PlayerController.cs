using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using Rooting.Models.ResponseModels;
using Rooting.Rules;

namespace Rooting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : MarmaladeController<PlayerController>
    {
        public PlayerController(GameStatistics gameStatistics, ILogger<PlayerController> logger) : base(gameStatistics, logger)
        {
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
            return ExecuteForUser(id, (playerData) =>
            {
                gameStatistics.UpdatePlayer(playerData.FamilyType, player.Name, player.Avatar);
                return player;
            });
        }

        [HttpPost("ResetGame")]
        public long ResetGame()
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