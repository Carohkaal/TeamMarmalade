namespace Rooting.Models
{
    public class TileBase : RootingModelBase
    {
        public TileBase()
        {
        }

        public TileBase(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; set; }
        public int Col { get; set; }
        public FamilyTypes FamilyType { get; set; }

        public ICollection<CardBase> CardsPlayed { get; set; } = Array.Empty<CardBase>();
    }
}