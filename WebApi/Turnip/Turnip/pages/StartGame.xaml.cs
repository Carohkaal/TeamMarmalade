using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for StartGame.xaml
    /// </summary>
    public partial class StartGame : Page
    {
        private readonly ILogger<StartGame> logger;
        private readonly INavigationService navigationService;

        public StartGame(ILogger<StartGame> logger, INavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            DataContext = new GameModel();
            InitializeComponent();

            MainGrid.Background = new ImageBrush
            {
                ImageSource = ResourceAccessor.GetImage("/content/graphics/startscreen01.png")
            };
            Game.ResetId();
            Game.NickName = ResourceAccessor.GetRandomNickName();
            Game.Alliance = Alliance.Plants;

            btnStart.Click += OnStart;
            btnPlants.Click += (object sender, RoutedEventArgs e) => { Game.Alliance = Alliance.Plants; };
            btnFunghi.Click += (object sender, RoutedEventArgs e) => { Game.Alliance = Alliance.Fungi; };
            btnAnimaux.Click += (object sender, RoutedEventArgs e) => { Game.Alliance = Alliance.Animaux; };
            btnRecycleGameId.Click += OnResetId;
            btnRecycleNickname.Click += OnResetNickName;
        }

        public GameModel Game => (GameModel)DataContext;

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Stopping application.");
            Application.Current.Shutdown();
        }

        private void OnResetNickName(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Reset nick name.");
            Game.NickName = ResourceAccessor.GetRandomNickName();
        }

        private void OnResetId(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Reset game id.");
            Game.ResetId();
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Game initialized, waiting for other players.");
            navigationService.NavigateTo(PageNames.GameView);
        }
    }
}