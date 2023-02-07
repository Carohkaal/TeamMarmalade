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
using Turnip.Services;

namespace Turnip.pages
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Page
    {
        private readonly ILogger<Splash> logger;
        private readonly INavigationService navigationService;

        public Splash(ILogger<Splash> logger, INavigationService navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            InitializeComponent();

            btnExit.Click += OnQuit;
            btnStart.Click += OnStart;
        }

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Stopping application.");
            Application.Current.Shutdown();
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("starting new game application.");
            navigationService.NavigateTo(PageNames.StartGame);
        }
    }
}