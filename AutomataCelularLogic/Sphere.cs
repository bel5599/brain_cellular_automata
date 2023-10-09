using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class Sphere
    {
        public int radio;
        public List<Cell> cell_list;
        public Sphere(int radio, List<Cell> cell_list)
        {
            this.radio = radio;
            this.cell_list = cell_list;
        }
    }
}
