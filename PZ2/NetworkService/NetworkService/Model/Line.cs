using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Line : BindableBase
    {
        int x1, x2, y1, y2, id;
        static int i;
        public Line(int x1, int x2, int y1, int y2)
        {
            this.X1 = x1;
            this.X2 = x2;
            this.Y1 = y1;
            this.Y2 = y2;
            this.Id = i++;
        }

        public int X1 { get => x1; set { x1 = value; OnPropertyChanged("X1"); } }
        public int X2 { get => x2; set { x2 = value; OnPropertyChanged("X2"); } }
        public int Y1 { get => y1; set { y1 = value; OnPropertyChanged("Y1"); } }
        public int Y2 { get => y2; set { y2 = value; OnPropertyChanged("Y2"); } }
        public int Id { get => id; set { id = value; OnPropertyChanged("Id"); } }
    }
}
