using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Printing.Interop;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;
using Turnip.Models;
using Turnip.pages;
using Turnip.Services;

namespace Turnip
{
    public static class PageNames
    {
        public const string Splash = nameof(pages.Splash);
        public const string LeaderBoard = nameof(pages.LeaderBoard);
        public const string Scores = nameof(pages.Scores);
        public const string StartGame = nameof(pages.StartGame);
        public const string GameView = nameof(pages.GameView);
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private readonly ServiceProvider container;
        private readonly ILogger<MainWindow>? logger;
        private readonly INavigationService? navigationService;

        public MainWindow(ServiceProvider container)
        {
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.container = container;
            this.logger = container.GetService<ILogger<MainWindow>>();
            this.navigationService = container.GetService<INavigationService>();

            if (navigationService != null) navigationService.Navigate += OnNavigate;
            InitializeComponent();
        }

        private void WindowLoaded(object sender, EventArgs e)
        {
            SetPage("splash");
        }

        private void OnNavigate(object? sender, EventArgs e)
        {
            var pageInfo = e as PageNavigationEventArgs;
            if (pageInfo == null)
            {
                logger?.LogWarning("Invalid arguments for navigation.");
                return;
            }

            var page = GetPage(pageInfo.Page);
            if (page != null)
            {
                this.MainFrame.Content = page;
            }
            else
            {
                logger?.LogError($"Could not set page with name: {pageInfo.Page}");
            }
        }

        public void SetPage(string name)
        {
            var page = GetPage(name);
            if (page != null)
            {
                this.MainFrame.Content = page;
            }
            else
            {
                logger?.LogError($"Could not set page with name: {name}");
            }
        }

        private Page? GetPage(string pageName) =>
         (pageName.ToUpperInvariant()) switch
         {
             "SPLASH" => container.GetService<Splash>(),
             "LEADERBOARD" => container.GetService<LeaderBoard>(),
             "SCORES" => container.GetService<Scores>(),
             "STARTGAME" => container.GetService<StartGame>(),
             "GAMEVIEW" => container.GetService<GameView>(),
             _ => null
         };
    }
}