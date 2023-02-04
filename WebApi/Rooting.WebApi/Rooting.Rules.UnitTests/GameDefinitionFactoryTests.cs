using Newtonsoft.Json;

namespace Rooting.Rules.UnitTests
{
    [TestClass]
    public class GameDefinitionFactoryTests
    {
        [TestMethod]
        public void GameDefinitionFactoryCanInitialize()
        {
            var sut = new GameDefinitionFactory();
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void GameDefinitionFactoryProviderGames()
        {
            var sut = new GameDefinitionFactory();
            var game = sut.NewGame(1);
            Assert.IsNotNull(game);
            Console.WriteLine(JsonConvert.SerializeObject(game, Formatting.Indented));
        }
    }
}