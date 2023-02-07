using System;

namespace Turnip.Models
{
    public class TilePosition
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public override string ToString() => $"R{Row:D2}{Column:D2}";

        public TilePosition()
        {
        }

        public event EventHandler? PositionChanged;

        public TilePosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Update(int row, int column)
        {
            if (Row != row || Column != column)
            {
                Row = row;
                Column = column;
                PositionChanged?.Invoke(this, new TileCoordinatesEventArgs(row, column));
                return true;
            }
            return false;
        }
    }
}