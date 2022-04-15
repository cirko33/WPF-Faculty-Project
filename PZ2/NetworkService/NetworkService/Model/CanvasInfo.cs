using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.Model
{
    public class CanvasInfo : BindableBase
    {
        Struja struja;
        bool taken;
        int x, y;
        ObservableCollection<int> lines;
        public CanvasInfo(int ind)
        {
            Taken = false;
            Struja = new Struja();
            X = 10 + (ind % 4 + 1) * 100 + (ind % 4) * 160;
            Y = 85 + (ind / 4) * 200;
            lines = new ObservableCollection<int>();
        }

        public CanvasInfo(Struja struja, bool taken, int ind) 
        {
            this.Struja = struja;
            this.Taken = taken;
            X = 10 + (ind % 4 + 1) * 100 + (ind % 4) * 160;
            Y = 85 + (ind / 4) * 200;
            lines = new ObservableCollection<int>();
        }

        public Brush Background { 
            get
            {
                if(Struja.Type != null)
                {
                    BitmapImage slika = new BitmapImage();
                    slika.BeginInit();
                    slika.UriSource = new Uri(Environment.CurrentDirectory + "../../../" + Struja.Type.Img_src);
                    slika.EndInit();
                    return new ImageBrush(slika);
                }
                else
                    return Brushes.GhostWhite;
            }
        }
        public string Text { get => Struja.Name != null ? "ID: " + Struja.Id + "Name: " + Struja.Name : ""; }
        public string Foreground { get => Struja.Valued < 0.34 || Struja.Valued > 2.73 ? "Red" : "Blue"; }
        public bool Taken { get => taken;
            set
            {
                if (taken != value)
                { 
                    taken = value;
                    OnPropertyChanged("Taken");
                }
            }
        }
        public Struja Struja { get => struja;
            set
            {
                struja = value;
                OnPropertyChanged("Struja");
                OnPropertyChanged("Foreground");
                OnPropertyChanged("Text");
            }
        }

        public int X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }
        public int Y
        {
            get => y; 
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        public ObservableCollection<int> Lines { get => lines;
            set { Lines = value; OnPropertyChanged("Lines"); } }
    }
}
