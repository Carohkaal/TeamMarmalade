using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using Rooting.Models.ResponseModels;

namespace Rooting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : MarmaladeController<GameController>
    {
        public GameController(
            GameStatistics gameStatistics,
            ILogger<GameController> logger) : base(gameStatistics, logger)
        {
        }

        [HttpGet("CardsToPlay/{id}")]
        public ActionResult<PlayingCard[]> CardsToPlay(string id)
        {
            return ExecuteForUser(id, (player) =>
            {
                return gameStatistics.CurrentInHand(player.FamilyType);
            });
        }

        [HttpGet("CardDefinitions")]
        public CardModel[] CardDefinitions()
        {
            return gameStatistics.Cards();
        }
    }
}