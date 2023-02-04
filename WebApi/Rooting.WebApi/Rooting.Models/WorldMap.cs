namespace Rooting.Models
{
    public class WorldMap
    {
        public static int Rows => 10;
        public static int Cols => 10;

        public int Generation { get; set; }
        public TileBase[,] Tiles { get; set; } = new TileBase[Rows, Cols];

        private static readonly TileBase EmptyTile = new TileBase();

        public WorldMap()
        {
            for (var r = 0; r < Rows; r++)
                for (var c = 0; c < Cols; c++)
                {
                    Tiles[c, r] = EmptyTile;
                }
        }
    }
}