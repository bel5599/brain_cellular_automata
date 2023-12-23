using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public struct Pos
    {
        int x;
        int y;
        int z;
        public Pos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public int Z
        {
            get { return z; }
            set { z = value; }
        }

        public bool NullPos()
        {
            return x == -1;
        }
        //public override bool Equals(object obj)
        //{
        //    if ((obj == null) || !this.GetType().Equals(obj.GetType())) return false;
        //    Pos pos = (Pos)obj;
        //    return x == pos.x && y == pos.y && z == pos.z;
        //}
    }
}
