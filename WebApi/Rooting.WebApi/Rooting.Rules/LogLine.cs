namespace Rooting.Rules
{
    public class LogLine
    {
        public LogLine()
        {
        }

        public LogLine(LogLineType type, string name, string message)
        {
            Type = type;
            Name = name;
            Message = message;
        }

        public LogLineType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

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