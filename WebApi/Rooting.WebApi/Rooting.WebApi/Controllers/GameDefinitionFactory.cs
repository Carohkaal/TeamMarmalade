using Rooting.Rules;
using Rooting.WebApi.MapResource;

namespace Rooting.WebApi.Controllers
{
    public class GameDefinitionFactory : IGameDefinitionFactory
    {
        private readonly GameSetup gameSetup;

        public GameDefinitionFactory()
        {
            using var s = ResourceHelper.ReadResource("MapResource", "Game-Setup.json");
            gameSetup = ImportHelper.ImportSetup(s).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initialize a new game setup, based on a game id.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public GameSetup NewGame(int gameId)
        {
            if (gameId == 0) return new GameSetup();
            return (GameSetup)gameSetup.Clone();
        }
    }
}