namespace Rooting.Rules
{
    public interface IGameDefinitionFactory
    {
        GameSetup NewGame(int gameId);
    }
}