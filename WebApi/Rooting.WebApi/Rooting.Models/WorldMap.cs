namespace Rooting.Models
{
    public class WorldMap
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public int Generation { get; set; }
        public IEnumerable<TileBase> Tiles => tiles;
        private readonly List<TileBase> tiles = new();

        private static readonly TileBase EmptyTile = new TileBase();

        public WorldMap()
        {
        }

        public void InitWorld(int rows, int cols, IEnumerable<TileBase> tileMap)
        {
            tiles.Clear();
            Rows = rows; Cols = cols;
            for (var r = 0; r < Rows; r++)
                for (var c = 0; c < Cols; c++)
                {
                    TileBase newTile;
                    var tile = tileMap.FirstOrDefault(t => t.Row == r && t.Col == c);

                    if (tile != null)
                        newTile = (TileBase)tile.Clone();
                    else
                        newTile = (TileBase)EmptyTile.Clone();

                    newTile.Uuid = Guid.NewGuid();
                    newTile.Col = c;
                    newTile.Row = r;
                    newTile.Name = newTile.FamilyType.ToString();

                    tiles.Add(newTile);
                }
        }
    }
}