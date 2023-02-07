using System;
using System.Security.Policy;
using System.Windows.Media;
using Turnip.Services;

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
            Notify(this, "TilePosition");
            Notify(this, "DebugInfo");
        }

        public string NickName
        {
            get => nickName; set
            {
                if (value == nickName) return;
                nickName = value;
                Notify(this, nameof(NickName));
            }
        }
        private string nickName = string.Empty;

        public string UserId { get; } = Guid.NewGuid().ToString();

        public Alliance Alliance
        {
            get => alliance; set
            {
                if (alliance == value) return;
                alliance = value;
                Notify(this, new[] { nameof(Alliance), nameof(PlantBgColor), nameof(FunghiBgColor), nameof(AnimauxBgColor) });
            }
        }
        private Alliance alliance;

        public bool GameStarted { get; private set; }
        public string GameId { get; private set; } = string.Empty;
        public string HelpText { get; private set; } = string.Empty;

        public TilePosition TilePosition { get; } = new TilePosition(3, 4);
        public bool SelectTile { get; private set; } = false;

        public void SetSelectTileStatus(bool value)
        {
            SelectTile = value;
            Notify(this, "SelectTile");
            Notify(this, "DebugInfo");
        }

        internal void ResetId()
        {
            if (!GameStarted)
            {
                GameId = ResourceAccessor.GetRandomGameId();
                Notify(this, nameof(GameId));
            }
        }

        public string PlantBgColor => Alliance == Alliance.Plants ? nameof(Colors.DarkGreen) : nameof(Colors.Black);
        public string FunghiBgColor => Alliance == Alliance.Fungi ? nameof(Colors.DarkBlue) : nameof(Colors.Black);
        public string AnimauxBgColor => Alliance == Alliance.Animaux ? nameof(Colors.DarkKhaki) : nameof(Colors.Black);

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