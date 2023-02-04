namespace Rooting.Models
{
    public class PlayingCard : RootingModelBase
    {
        private CardBase Card { get; set; } = new();
        private bool InStock { get; set; }
        private bool OnWorldMap { get; set; }
    }
}