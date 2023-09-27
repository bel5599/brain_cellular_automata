using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataCelular
{
    class StemCell: Cell
    {
        public Pos des_pos;
        public StemCell(Pos pos)
        {
            this.pos = pos;
            des_pos = null;
        }
    }
}
