using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Homework4
{
    class Tile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // Tile number
        private string _name;
        public string tName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("tName");
            }
        }

        // Tile text
        private string _label;
        public string tLabel
        {
            get { return _label; }
            set
            {
                _label = value;
                OnPropertyChanged("tLabel");
            }
        }

        // Tile text color
        private Brush _brush;
        public Brush tBrush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                OnPropertyChanged("tBrush");
            }
        }

        // Tile background color
        private Brush _background;
        public Brush tBackground
        {
            get { return _background; }
            set
            {
                _background = value;
                OnPropertyChanged("tBackground");
            }
        }

        // Tile enabled status
        private bool _enabled;
        public bool tEnabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged("tEnabled");
            }
        }

    }
}
