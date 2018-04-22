using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Homework4
{
    partial class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // Changes start button text
        private String _start = "";
        public String Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        private Boolean _startenabled;
        public Boolean StartEnabled
        {
            get { return _startenabled; }
            set
            {
                _startenabled = value;
                OnPropertyChanged("StartEnabled");
            }
        }

        // Changes status box header
        private String _header = "";
        public String StatusHeader
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged("StatusHeader");
            }
        }

        // Changes status box text
        private String _status = "";
        public String Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        // Changes status box color
        private Brush _statColor;
        public Brush StatusColor
        {
            get { return _statColor; }
            set
            {
                _statColor = value;
                OnPropertyChanged("StatusColor");
            }
        }

        // Changes main window color
        private Brush _mainbackground;
        public Brush MainBackground
        {
            get { return _mainbackground; }
            set
            {
                _mainbackground = value;
                OnPropertyChanged("MainBackground");
            }
        }

        [Serializable]
        struct TinyTile
        {
            public string name, l;
            public string b, bg;
            public bool e;
            public int num;

            // Note: Can't serialize a brush, so convert brush to string and back when needed

            public TinyTile(Tile t, int n)
            {
                name = t.tName;
                l = t.tLabel;
                b = new BrushConverter().ConvertToString(t.tBrush);
                bg = new BrushConverter().ConvertToString(t.tBackground);
                e = t.tEnabled;
                num = n;
            }

            public TinyTile(int n)
            {
                name = null;
                l = null;
                b = null;
                bg = null;
                e = false;
                num = n;
            }

            public Tile getTile()
            {
                return new Tile()
                {
                    tName = name,
                    tLabel = l,
                    tBrush = (Brush)new BrushConverter().ConvertFromString(b),
                    tBackground = (Brush)new BrushConverter().ConvertFromString(bg),
                    tEnabled = e
                };
            }

        }

    }
}
