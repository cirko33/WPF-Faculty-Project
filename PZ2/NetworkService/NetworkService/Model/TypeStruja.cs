using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class TypeStruja 
    {
        string name;
        string img_src;

        public TypeStruja(string type)
        {
            name = type;
            img_src = (type == "Interval Meter") ? "/Resources/Images/intervalmeter.jpg" :
                "/Resources/Images/smartmeter.jpg";
        }

        public string Name { get => name; set => name = value; }
        public string Img_src { get => img_src; set => img_src = value; }
    }
}
