﻿namespace Rooting.Models
{
    public class WorldMap
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public int Generation { get; set; }
        public TileBase[,] Tiles { get; set; }

        private static readonly TileBase EmptyTile = new TileBase();

        public WorldMap()
        {
            Tiles = new TileBase[0, 0];
        }

        public void InitWorld(int rows, int cols, IEnumerable<TileBase> tiles)
        {
            Rows = rows; Cols = cols;
            Tiles = new TileBase[cols, rows];
            for (var r = 0; r < Rows; r++)
                for (var c = 0; c < Cols; c++)
                {
                    TileBase newTile;
                    var tile = tiles.FirstOrDefault(t => t.Row == r && t.Col == c);
                    if (tile != null)
                        newTile = tile;
                    else
                        newTile = (TileBase)EmptyTile.Clone();

                    newTile.Uuid = Guid.NewGuid();
                    newTile.Col = c;
                    newTile.Row = r;
                    newTile.Name = newTile.FamilyType.ToString();

                    Tiles[c, r] = newTile;
                }
        }
    }
}