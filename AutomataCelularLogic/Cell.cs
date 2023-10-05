using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public enum CellActions
    {
        division,
        contaminate,
        nothing
    }
    public class Cell
    {
        public Pos pos;
        public Pos des_pos;
        public CellActions actual_action;
        public Behavior cell_behavior;
        public Probability move_prob;
        public Cell(Pos pos, Behavior cell_behavior, Probability move_prob)
        {
            this.pos = pos;
            des_pos = null;
            actual_action = new CellActions();
            this.cell_behavior = cell_behavior;
            this.move_prob = move_prob;
        }

    }
}
