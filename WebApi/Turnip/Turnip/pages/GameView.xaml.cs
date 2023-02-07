using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Turnip.Models;
using Turnip.Services;

namespace Turnip.pages
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Page
    {
        private readonly ILogger<MainWindow> logger;

        public GameView(ILogger<MainWindow> logger)
        {
            this.logger = logger;

            InitializeComponent();

            btnQuit.Click += OnQuit;
            btnResign.Click += OnResign;

            // Navigation
            btnUp.Click += MoveUp;
            btnDown.Click += MoveDn;
            btnLeft.Click += MoveLt;
            btnRight.Click += MoveRt;
            btnHome.Click += MoveCenter;

            DataContext = new GameModel();
            Game.SetSelectTileStatus(true);
        }

        private GameModel Game => (GameModel)DataContext;

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OnResign(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Resigning game.");

            var bitmap = ResourceAccessor.GetImage("/content/graphics/tiles/animaltilebasic.png");
        }

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Stopping application.");
            Application.Current.Shutdown();
        }

        private void MoveCenter(object sender, RoutedEventArgs e)
        { Game.TilePosition.Update(3, 4); }

        private void MoveUp(object sender, RoutedEventArgs e)
        { _ = TryMoveCursor(-1, 0); }

        private void MoveDn(object sender, RoutedEventArgs e)
        { _ = TryMoveCursor(1, 0); }

        private void MoveLt(object sender, RoutedEventArgs e)
        { _ = TryMoveCursor(0, -1); }

        private void MoveRt(object sender, RoutedEventArgs e)
        { _ = TryMoveCursor(0, 1); }

        private bool TryMoveCursor(int moveY, int moveX)
        {
            var oldY = Game.TilePosition.Row;
            var oldX = Game.TilePosition.Column;
            var newY = Game.TilePosition.Row + moveY;
            var newX = Game.TilePosition.Column + moveX;
            var img = FindTile(newY, newX);
            if (img != null)
            {
                if (Game.TilePosition.Update(newY, newX))
                {
                    img.Opacity = 0.50;
                    var oldTile = FindTile(oldY, oldX);
                    if (oldTile != null) oldTile.Opacity = 1.0;
                }
            }
            return false;
        }

        private Image? FindTile(int row, int column)
        {
            return this.FindName($"R{row:D2}{column:D2}") as Image;
        }
    }
}