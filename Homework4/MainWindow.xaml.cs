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
//using SocketSetup;

namespace Homework4
{
    public partial class MainWindow : Window
    {
        private Model mod;

        public MainWindow()
        {
            InitializeComponent();
            mod = new Model();

            this.ResizeMode = ResizeMode.NoResize;
            this.DataContext = mod;

            IControl.ItemsSource = mod.TileCollection;
        }

        // Handles tile presses
        private void tileHandler(object sender, RoutedEventArgs e)
        {
            var b = e.OriginalSource as FrameworkElement;
            if (b != null)
            {
                var pressed = b.DataContext as Tile;
                mod.localTilePressed(int.Parse(pressed.tName));
            }
        }

        // Handles play button presses
        private void startHandler(object sender, RoutedEventArgs e)
        {
            mod.startPressed();
        }

        private void setupHandler(object sender, RoutedEventArgs e)
        {
            SocketSetupWindow ssw = new SocketSetupWindow();
            ssw.ShowDialog();

            mod.setNetworkSettings(ssw.localport, ssw.localip, ssw.remoteport, ssw.remoteip);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mod.cleanup();
        }
    }
}
