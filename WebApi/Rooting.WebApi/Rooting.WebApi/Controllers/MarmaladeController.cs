using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using Rooting.Rules;

namespace Rooting.WebApi.Controllers
{
    public abstract class MarmaladeController : ControllerBase
    {
        protected readonly GameStatistics gameStatistics;

        public MarmaladeController(GameStatistics gameStatistics)
        {
            this.gameStatistics = gameStatistics;
        }
    }

    public abstract class MarmaladeController<TController> : MarmaladeController where TController : MarmaladeController
    {
        protected readonly ILogger<TController> logger;

        public MarmaladeController(GameStatistics gameStatistics, ILogger<TController> logger) : base(gameStatistics)
        {
            this.logger = logger;
        }

        protected ActionResult<TModel> ExecuteForUser<TModel>(string playerId, Func<Player, TModel> func)
        {
            var client = Request.HttpContext.Connection.RemotePort.ToString() ?? "0";
            if (!Guid.TryParse(playerId, out var playerUuId))
            {
                logger.LogWarning($"Invalid id used in UpdatePlayer: '{playerUuId}'");
                return BadRequest("id is not a valid player id");
            }

            var playerData = gameStatistics.Player(playerUuId);
            if (playerData == null)
            {
                return NotFound();
            }
            if (playerData.RemoteIp != client)
            {
                logger.LogError($"Client '{client}' tried to access user: '{playerId}'.");
                return Forbid("Detected invalid access.");
            }

            return func.Invoke(playerData);
        }
    }
}