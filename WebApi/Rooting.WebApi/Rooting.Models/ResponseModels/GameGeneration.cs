namespace Rooting.Models.ResponseModels
{
    public class GameGeneration
    {
        public int Id { get; set; }
        public GameStatus GameStatus { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime NextTurn { get; set; }
        public string Shout { get; set; } = string.Empty;
    }
}