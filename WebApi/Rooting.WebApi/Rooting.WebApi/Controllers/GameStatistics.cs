using Rooting.Models;
using System.Collections.Concurrent;

namespace Rooting.WebApi.Controllers
{
    public class GameStatistics : IGameStatistics
    {
        private Guid gameId = Guid.NewGuid();
        private readonly ConcurrentDictionary<FamilyTypes, Player> activePlayers = new();
        public Guid GameId => gameId;
        public int Generation { get; private set; }
        public DateTime TimeStarted { get; private set; }
        public bool GameStarted { get; private set; }
        public IEnumerable<Player> Players => activePlayers.Values;

        public PlayerModel ClaimPlayer(PlayerModel model, string remoteIp)
        {
            var message = string.Empty;
            var player = new Player
            {
                Uuid = model.Uuid,
                Name = model.Name,
                FamilyType = model.FamilyType,
                Avatar = model.Avatar,
                RemoteIp = remoteIp
            };
            if (activePlayers.Values.Any(m => m.RemoteIp == remoteIp))
            {
                return new PlayerModel
                {
                    Name = player.Name,
                    FamilyType = player.FamilyType,
                    Avatar = player.Avatar,
                    Uuid = Guid.Empty,
                    Message = "This client already claimed a player."
                };
            }
            if (activePlayers.TryAdd(player.FamilyType, player))
            {
                message = $"Claimed {player.FamilyType}";
            }
            else
            {
                player.Uuid = Guid.Empty;
                message = $"Someone already claimed {player.FamilyType}";
            }
            return new PlayerModel
            {
                Name = player.Name,
                FamilyType = player.FamilyType,
                Avatar = player.Avatar,
                Message = message,
                Uuid = player.Uuid
            };
        }

        public void ResetGame()
        {
            gameId = Guid.NewGuid();
            Generation = 0;
            TimeStarted = DateTime.MinValue;
            GameStarted = false;
            activePlayers.Clear();
        }

        internal Player? Player(Guid playerId)
        {
            return activePlayers.Values.FirstOrDefault(m => m.Uuid == playerId);
        }

        internal void UpdatePlayer(FamilyTypes family, string name, string avatar)
        {
            var player = activePlayers[family];
            player.Name = name;
            player.Avatar = avatar;

            activePlayers.TryUpdate(family, player, player);
        }
    }
}