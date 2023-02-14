using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
            GameManagement gameManagement,
            ILogger<GameController> logger) : base(gameManagement, logger)
        {
        }

        [HttpGet("CardsToPlay/{playerId}")]
        public ActionResult<PlayingCard[]> CardsToPlay(string playerId)
        {
            return ExecuteForUser(playerId, (player) =>
            {
                return gameManagement.CurrentInHand(player.FamilyType);
            });
        }

        [HttpPost("PlayCard/{playerId}/{tileId}")]
        public ActionResult<PlayingCard> PlayCard(string playerId, string tileId, [FromBody] PlayingCard playingCard)
        {
            if (!Guid.TryParse(tileId, out var tileReference))
            {
                return BadRequest($"Invalid tile id: {tileId}, GUID expected");
            }

            var tile = gameManagement.WorldMap.Tiles.FirstOrDefault(m => m.Uuid == tileReference);
            if (tile == null)
            {
                return BadRequest($"Invalid tile id: {tileId}, Not found");
            }

            return ExecuteForUser(playerId, (player) =>
            {
                return gameManagement.PlayCard(player, playingCard, tile);
            });
        }

        /// <summary>
        /// Get information for the game properties
        /// </summary>
        /// <returns></returns>
        [HttpGet("CardDefinitions/{gameId}")]
        public ActionResult<CardModel[]> CardDefinitions(string gameId)
        {
            gameManagement.Cards();
        }

        /// <summary>
        /// Get the current game status and timing information for the next run.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GameStatus/{gameId}")]
        public GameGeneration GameStatus() => gameManagement.ReadGameStatus();

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
                return gameManagement.StartGame(player, withForce);
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
                return gameManagement.GameStatusUserIntervention(player, gameStatus);
            });
        }

        /// <summary>
        /// Get the game log for the current game.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GameLog")]
        public GameLog OpenGameLog() => gameManagement.OpenGameLog();

        [HttpGet("World/{gameId}/{playerId}")]
        public ActionResult<WorldMap> WorldMap(string gameId, string playerId)
        {
            if (!long.TryParse(gameId, out var gameReference))
            {
                return BadRequest($"Invalid game id: {gameId}");
            }
            return ExecuteForUser(playerId, (player) =>
            {
                return gameManagement.GetWorldMap(player, gameReference);
            });
        }
    }
}