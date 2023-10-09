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

        public Pos Division(Dictionary<Pos, Cell> pos_cell_dict, int tumoral_cell_radio, Cell tumor_stem_cell)
        {
            if (cell_behavior is TumorCellBehavior)
            {
                bool new_pos = false;

                while (!new_pos)
                {
                    int p = Utils.rdm.Next(0, Utils.mov_3d.Count);

                    var array = Utils.mov_3d[p];
                    Pos new_position = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);

                    //!ExistentPosition(tumor_cell_list, pos, false)
                    if (!pos_cell_dict.ContainsKey(new_position))
                    {
                        new_pos = true;
                        float prob = move_prob.DivisionProbability(pos, Utils.mov_3d, pos_cell_dict, tumoral_cell_radio, Utils.EuclideanDistance(tumor_stem_cell.pos, pos));
                        if (prob >= 0.5)
                        {
                            return pos;
                        }
                        return null;

                        //Aplicar una probabilidad para dividirse a una posicion x
                    }
                }
            }
            return null;
        }
    }
}
