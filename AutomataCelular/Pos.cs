using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataCelular
{
    class Pos
    {
        int x;
        int y;
        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set {  y = value; }
        }
    }
}
