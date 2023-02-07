using System;

namespace Turnip.Models
{
    public class GameModel : PageViewModel
    {
        public GameModel()
        {
            TilePosition.PositionChanged += OnTilePositionChanged;
        }

        private void OnTilePositionChanged(object? sender, EventArgs e)
        {
            Notify("TilePosition");
            Notify("DebugInfo");
        }

        public TilePosition TilePosition { get; } = new TilePosition(3, 4);
        public bool SelectTile { get; private set; } = false;

        public void SetSelectTileStatus(bool value)
        {
            SelectTile = value;
            Notify("SelectTile");
            Notify("DebugInfo");
        }

        public string DebugInfo
        {
            get
            {
                return
                    $"tilePos={TilePosition};\n" +
                    $"select={SelectTile};\n" +
                    $"{DateTime.Now:HH:mm:ss}";
            }
        }
    }
}