using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using Rooting.Models;
using Rooting.Models.ResponseModels;
using Rooting.Rules;
using Rooting.Rules.Repository;
using System.Numerics;

namespace Rooting.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : MarmaladeController<PlayerController>
    {
        private readonly IGameDataRepository repository;

        public PlayerController(
            IGameDataRepository repository,
            GameStatistics gameStatistics,
            ILogger<PlayerController> logger) : base(gameStatistics, logger)
        {
            this.repository = repository;
        }

        [HttpGet("CurrentPlayers/{gameId}")]
        public IEnumerable<PlayerModel> Get(string gameId)
        {
            return gameManagement
                .Players
                .Where(m => m.GameId == gameId)
                .Select(m => new PlayerModel
                {
                    Uuid = m.Uuid,
                    Name = m.Name,
                    Avatar = m.Avatar,
                    GameId = gameId,
                    FamilyType = m.FamilyType
                });
        }

        /// <summary>
        /// Use this method to claim a position is a given game. Each family
        /// can be claimed by only one person per game. The game id is part of the PlayerModel.
        /// The method return the player model with a valid Guid in the uuid property,
        /// or a message and an empty Guid in the uuid if the claim failed.
        /// The player model is registered in a repository.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        [HttpPost("ClaimFamily")]
        public PlayerModel ClaimPlayer(PlayerModel player)
        {
            var client = Request.HttpContext.Connection.RemotePort.ToString() ?? "0";
            var id = repository.ClaimPlayerId(player.GameId, player.FamilyType);
            player.Uuid = id;
            return gameManagement.ClaimPlayer(player, client);
        }

        /// <summary>
        /// Update the name and/or avatar for a player. The familty type cannot be modified.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="player"></param>
        /// <returns>The current player model is returned</returns>
        [HttpPut("Player/{id}")]
        public ActionResult<PlayerModel> UpdatePlayer(string id, [FromBody] PlayerModel player)
        {
            return ExecuteForUser(id, (playerData) =>
            {
                gameManagement.UpdatePlayer(playerData.Uuid, player.Name, player.Avatar);
                return player;
            });
        }

        /// <summary>
        /// Resign from a game. The family is the game is released and the player
        /// data is removed from the repository.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Resign/{playerId}")]
        public ActionResult<PlayerModel?> ExitGame(string playerId)
        {
            return ExecuteForUser(playerId, (playerData) =>
            {
                return gameManagement.ResignPlayer(playerData.Uuid);
            });
        }
    }
}