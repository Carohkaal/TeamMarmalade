using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using Rooting.Models.ResponseModels;
using Rooting.Rules;

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

        [HttpGet("CardsToPlay/{playerId}")]
        public ActionResult<PlayingCard[]> CardsToPlay(string playerId)
        {
            return ExecuteForUser(playerId, (player) =>
            {
                return gameStatistics.CurrentInHand(player.FamilyType);
            });
        }

        [HttpPost("PlayCard/{playerId}")]
        public ActionResult<PlayingCard> PlayCard(string playerId, [FromBody] PlayingCard playingCard)
        {
            return ExecuteForUser(playerId, (player) =>
            {
                return gameStatistics.PlayCard(player.FamilyType, playingCard);
            });
        }

        /// <summary>
        /// Get information for the game properties
        /// </summary>
        /// <returns></returns>
        [HttpGet("CardDefinitions")]
        public CardModel[] CardDefinitions() => gameStatistics.Cards();

        /// <summary>
        /// Get the current game status and timing information for the next run.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GameStatus")]
        public GameStatus GameStatus() => gameStatistics.ReadGameStatus();

        /// <summary>
        /// Start the game, return the time the current turn will end.
        /// </summary>
        /// <returns></returns>
        [HttpPost("StartGame/{playerId}/{force}")]
        public ActionResult<GameGeneration> StartCurrentGame(string playerId, string force)
        {
            _ = bool.TryParse(force, out var withForce);
            return ExecuteForUser(playerId, (player) =>
            {
                return gameStatistics.StartGame(player, withForce);
            });
        }

        /// <summary>
        /// Start the game, return the time the current turn will end.
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateStatusGame/{playerId}")]
        public ActionResult<GameGeneration> SetCurrentStatus(string playerId, [FromBody] GameStatus gameStatus)
        {
            return ExecuteForUser(playerId, (player) =>
            {
                return gameStatistics.GameStatusUserIntervention(player, gameStatus);
            });
        }

        /// <summary>
        /// Get the game log for the current game.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GameLog")]
        public GameLog OpenGameLog() => gameStatistics.OpenGameLog();
    }
}