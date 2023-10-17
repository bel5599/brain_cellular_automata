using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    //public enum CellActions
    //{
    //    Division,
    //    Contaminate,
    //    Migrate,
    //    Nothing
    //}
    public enum LocationStatus
    {
        SmoothVesselCells_time1,
        SmoothVesselCells_time2,
        SmoothVesselCells_time3,
        GlialBasalLamina,
        VascularBasalLamina,
        EndothelialBasalLamina,
        Lumen,
        MatrixExtracelular
    }
    public class Cell
    {
        public Pos pos;
        public Pos des_pos;
        public Action actual_action;
        public Behavior cell_behavior;
        public Probability move_prob;
        public LocationStatus loca_status;
        public Cell(Pos pos, Behavior cell_behavior, Probability move_prob, LocationStatus loca_status)
        {
            this.pos = pos;
            des_pos = null;
            actual_action = new NothingAction();
            this.cell_behavior = cell_behavior;
            this.move_prob = move_prob;
            this.loca_status = loca_status;
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
                        float prob = move_prob.DivisionProbability(pos, tumoral_cell_radio, Utils.EuclideanDistance(tumor_stem_cell.pos, pos), EnvironmentLogic.tumor.new_cells_count);
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
