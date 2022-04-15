using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class Entity : BindableBase
    {
        double measured;
        DateTime date;

        public Entity()
        {
            measured = 0;
        }

        public Entity(double measured, DateTime date)
        {
            this.Measured = measured;
            this.Date = date;
        }

        public double Measured { get => measured;
            set 
            { 
                measured = value;
                OnPropertyChanged("Measured");
                OnPropertyChanged("Color");
                OnPropertyChanged("DisplayMeasure");
                OnPropertyChanged("DrawMeasured");
                OnPropertyChanged("DisplayDate");
            }
        }
        public DateTime Date { get => date;
            set
            {
                date = value;
                OnPropertyChanged("Date");
                OnPropertyChanged("DisplayDate");
            }
        }

        public string DisplayDate { get => Measured == 0 ? "" : Date.ToString("t");  }
        public string DisplayMeasure { get => Measured == 0 ? "" : Math.Round(Measured, 2).ToString(); }
        public double DrawMeasured { get => 380 - (50 * Measured); }
        public Brush Color { 
            get 
            {
                if (Measured == 0)
                    return Brushes.Transparent;
                else if (Measured < 0.34 || Measured > 2.73)
                    return Brushes.Red;
                else
                    return Brushes.Blue;
            }
        }
    }
}
