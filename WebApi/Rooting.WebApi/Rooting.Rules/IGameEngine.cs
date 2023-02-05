using Rooting.Models;

namespace Rooting.Rules
{
    public interface IGameEngine
    {
        void ExecuteLoop(IGameStatistics gameStatistics);

        void ApplyRule(string ruleName, IOrigin origin, WorldMap map);

        (PlayingState state, string? message, int costs) PlayCard(WorldMap map, CardBase cardRule, PlayingCard cardItem, TileBase tile, int PlayerTier);
    }
}