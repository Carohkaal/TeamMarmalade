using System;

namespace Turnip.Models
{
    public class TileCoordinatesEventArgs : EventArgs, IGameEvent
    {
        public TileCoordinatesEventArgs()
        {
        }

        public TileCoordinatesEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;
    }
}