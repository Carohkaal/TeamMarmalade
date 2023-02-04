namespace Rooting.Models
{
    public class TileBase : RootingModelBase
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public ICollection<CardBase> CardsPlayed { get; set; } = Array.Empty<CardBase>();
    }
}