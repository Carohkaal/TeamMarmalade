namespace Rooting.Models
{
    public class TileBase : RootingModelBase, ICloneable
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

        public ICollection<CardBase> CardsPlayed => cardsPlayed;
        private readonly List<CardBase> cardsPlayed = new List<CardBase>();

        public object Clone()
        {
            return new TileBase
            {
                FamilyType = FamilyType,
                Col = Col,
                Name = Name,
                Row = Row,
                Uuid = Uuid,
            };
        }
    }
}