using Rooting.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rooting.Rules.Repository
{
    public interface IGameDataRepository
    {
        Guid ClaimPlayerId(string GameId, FamilyTypes type);
    }

    public class GameDataRepository
    {
        private readonly ConcurrentDictionary<string, Guid> gameKeys = new();

        public Guid ClaimPlayerId(string GameId, FamilyTypes type)
        {
            var key = $"{GameId}:{type}";
            if (gameKeys.ContainsKey(key)) return Guid.Empty;
            var id = Guid.NewGuid();
            return gameKeys.TryAdd(key, id)
                ? id : Guid.Empty;
        }
    }
}