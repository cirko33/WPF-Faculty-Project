using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace NetworkService.Model
{
    public class Measured : BindableBase
    {
        string name;
        ObservableCollection<Entity> entities;

        public Measured(string name, ObservableCollection<Entity> entities)
        {
            this.Name = name;
            this.Entities = entities;
        }

        public Measured()
        {
            name = "";
            entities = new ObservableCollection<Entity>();
        }
        public ObservableCollection<Entity> Entities
        {
            get => entities;
            set
            {
                entities = value;
                OnPropertyChanged("Entities");
            }
        }
        public string Name { get => name;
            set
            {
                name = value;
                OnPropertyChanged("Entities");
            }
        }
    }
}
