using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Homework4
{
    public partial class SocketSetupWindow : Window, INotifyPropertyChanged
    {
        public int localport, remoteport;
        public string LocalPort
        {
            get { return localport == 0 ? "" : localport.ToString(); }
            set
            {
                if (value == "")
                    localport = 0;
                else
                    localport = int.Parse(value);
                OnPropertyChanged("LocalPort");
            }
        }
        public string RemotePort
        {
            get { return remoteport == 0 ? "" : remoteport.ToString(); }
            set
            {
                if (value == "")
                    remoteport = 0;
                else 
                    remoteport = int.Parse(value);
                OnPropertyChanged("RemotePort");
            }
        }

        public string localip, remoteip;
        public string LocalIP
        {
            get { return localip; }
            set
            {
                localip = value;
                OnPropertyChanged("LocalIP");
            }
        }
        public string RemoteIP
        {
            get { return remoteip; }
            set
            {
                remoteip = value;
                OnPropertyChanged("RemoteIP");
            }
        }

        public SocketSetupWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.DataContext = this;

            LocalPort = "";
            LocalIP = "";
            RemotePort = "";
            RemoteIP = "";

        }

        private void setDefaults1(object sender, RoutedEventArgs e)
        {
            LocalPort = "5151";
            LocalIP = "127.0.0.1";
            RemotePort = "5152";
            RemoteIP = "127.0.0.1";
        }

        private void setDefaults2(object sender, RoutedEventArgs e)
        {
            LocalPort = "5152";
            LocalIP = "127.0.0.1";
            RemotePort = "5151";
            RemoteIP = "127.0.0.1";
        }

        private void saveSettings(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
