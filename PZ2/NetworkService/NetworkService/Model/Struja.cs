using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Struja : ValidationBase
    {
        int id;
        string name;
        double valued;
        TypeStruja type;

        public Struja()
        {

        }
        public Struja(int id, string name, double valued, TypeStruja type)
        {
            this.Id = id;
            this.Name = name;
            this.Valued = valued;
            this.Type = type;
        }

        public Struja(Struja s)
        {
            this.Id = s.Id;
            this.Name = s.Name;
            this.Valued = s.Valued;
            this.Type = s.Type;
        }

        public int Id { get => id; 
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name { get => name; 
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public double Valued { get => valued;
            set
            {
                if (valued != value)
                {
                    valued = value;
                    OnPropertyChanged("Valued");
                }
            }
        }
        public TypeStruja Type { get => type; 
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        protected override void ValidateSelf()
        {
            if (this.Id <= 0)
            {
                this.ValidationErrors["Id"] = "ID must be more then 0 and must be a number";
            }
            else
            {
                foreach (Struja struja in ViewModel.NetworkEntitiesViewModel.Struje)
                {
                    if (struja.Id == this.Id)
                        this.ValidationErrors["Id"] = "Can't have 2 same ID's";
                }
            }

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                this.ValidationErrors["Name"] = "Name is required";
            }

            if(type == null)
            {
                this.ValidationErrors["Type"] = "Type is required";
            }
        }

        public override string ToString()
        {
            return Id + " " + Name + " " + Type.Name;
        }
    }
}
