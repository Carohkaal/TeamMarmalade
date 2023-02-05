using Rooting.Models;

namespace Rooting.Rules
{
    public interface IGameEngine
    {
        void ExecuteLoop(IGameStatistics gameStatistics);

        void ApplyRule(string ruleName, IOrigin origin, WorldMap map);
    }
}