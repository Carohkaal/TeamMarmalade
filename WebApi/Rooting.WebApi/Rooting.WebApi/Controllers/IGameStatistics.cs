using Rooting.Models;

namespace Rooting.WebApi.Controllers
{
    public interface IGameStatistics
    {
        Guid GameId { get; }
        bool GameStarted { get; }
        int Generation { get; }
        IEnumerable<Player> Players { get; }
        DateTime TimeStarted { get; }
    }
}