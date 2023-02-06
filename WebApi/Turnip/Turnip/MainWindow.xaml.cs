using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace Turnip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        private readonly ILogger<MainWindow> logger;

        public MainWindow(ILogger<MainWindow> logger)
        {
            this.logger = logger;
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            InitializeComponent();

            btnQuit.Click += OnQuit;
            btnResign.Click += OnResign;
        }

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

            var imageUri = ResourceAccessor.Get("content/graphics/cards/cover.300x400.png");
            var bitmap = new BitmapImage(imageUri);
            card4.Source = bitmap;
        }

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            logger.LogInformation("Stopping application.");
            Application.Current.Shutdown();
        }
    }
}