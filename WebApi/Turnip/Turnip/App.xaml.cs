using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Turnip.pages;
using Turnip.Services;

namespace Turnip
{
    public interface IMainWindow
    {
        void Show();
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider container;
        private readonly ILogger<Application> logger;

        public App()
        {
            logger = NullLogger<Application>.Instance;
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var options = new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true };
            container = serviceCollection.BuildServiceProvider(options);
        }

        private void ConfigureServices(IServiceCollection container)
        {
            container.AddLogging(
                o =>
                {
                    o.AddConsole();
                }
            );
            container.AddScoped<GameView>();
            container.AddScoped<Splash>();
            container.AddScoped<Scores>();
            container.AddScoped<StartGame>();
            container.AddScoped<LeaderBoard>();
            container.AddSingleton<INavigationService, PageNavigationService>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow(container);
            if (mainWindow == null)
            {
                logger.LogCritical("No main window");
                Current.Shutdown();
            }
            else
            {
                Application.Current.MainWindow = mainWindow;
                Application.Current.MainWindow.Show();
            }
        }
    }
}