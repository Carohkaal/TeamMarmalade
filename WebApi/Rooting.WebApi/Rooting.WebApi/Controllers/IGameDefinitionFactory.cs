using Rooting.Rules;

namespace Rooting.WebApi.Controllers
{
    public interface IGameDefinitionFactory
    {
        GameSetup NewGame(int gameId);
    }
}