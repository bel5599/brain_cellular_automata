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
    //public enum LocationStatus
    //{
    //    SmoothVesselCells_time1,
    //    SmoothVesselCells_time2,
    //    SmoothVesselCells_time3,
    //    GlialBasalLamina,
    //    VascularBasalLamina,
    //    EndothelialBasalLamina,
    //    Lumen,
    //    MatrixExtracelular
    //}
    public enum CellLocationState
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

    public enum CellState
    {
        Astrocyte,
        StemCell,
        Neuron,
        ProliferativeTumoralCell,
        NecroticTumorCell,
        QuiescentTumorCell,
        MigratoryTumorCell,
        nothing
        //TumoralAstrocyte,
        //TumoralStemCell,
        //TumoralNeuron
    }

    public class Cell
    {
        public Pos pos;

        //public double density;

        public CellState behavior_state;
        public CellLocationState loca_state;
        public List<Cell> neighborhood;
        public double proliferation_age;
        public Cell(Pos pos, CellState behavior_state, CellLocationState loca_state)
        {
            //if (loca_state == CellLocationState.MatrixExtracelular && behavior_state == CellState.nothing)
            //    density = Utils.rdm.NextDouble();
            //else
            //    density = 0;

            this.pos = pos;
            this.behavior_state = behavior_state;
            this.loca_state = loca_state;
            proliferation_age = 0;
        }

        //public void UpdateState()
        //{
        //    int tumoral_cells_count = TumoralCellCount();
        //    if (behavior_state == CellState.Astrocyte || behavior_state == CellState.Neuron)
        //    {
        //        if (tumoral_cells_count >= 10)
        //        {
        //            behavior_state = CellState.TumoralCell;
        //        }
        //        //si no se cumple eso entonces sigue con el mismo estado
        //    }
        //    else if (behavior_state == CellState.TumoralCell)
        //    {
        //        //de cierta manera aqui irian las celulas que se convierte en celulas necroticas y divi
        //        if (tumoral_cells_count == 26)
        //            throw new Exception();
        //    }
        //    else if (behavior_state == CellState.StemCell)
        //    {
        //        if (tumoral_cells_count >= 1)
        //        {
        //            behavior_state = CellState.TumoralCell;
        //        }
        //    }
        //    else if (behavior_state == CellState.nothing)
        //    {
        //        if (tumoral_cells_count >= 5)
        //        {
        //            behavior_state = CellState.TumoralCell;
        //        }
        //    }
        //}

        public int TumoralCellCount()
        {
            int count = 0;
            foreach (Cell cell in neighborhood)
            {
                if (cell.behavior_state == CellState.ProliferativeTumoralCell)
                    count++;
            }
            return count;
        }
    }


    //public class Cell
    //{

    //    //public Pos pos;
    //    //public Pos des_pos;
    //    //public Action actual_action;
    //    //public Probability move_prob;
    //    //public LocationStatus loca_status;
    //    public Cell(Pos pos, Probability move_prob, LocationStatus loca_status)
    //    {
    //        this.pos = pos;
    //        des_pos = null;
    //        actual_action = new NothingAction();
    //        this.move_prob = move_prob;
    //        this.loca_status = loca_status;
    //    }

    //    public Pos Division(Dictionary<Pos, Cell> pos_cell_dict, int tumoral_cell_radio, Cell tumor_stem_cell)
    //    {
    //        if (cell_behavior is TumorCellBehavior)
    //        {
    //            bool new_pos = false;

    //            while (!new_pos)
    //            {
    //                int p = Utils.rdm.Next(0, Utils.mov_3d.Count);

    //                var array = Utils.mov_3d[p];
    //                Pos new_position = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);

    //                //!ExistentPosition(tumor_cell_list, pos, false)
    //                if (!pos_cell_dict.ContainsKey(new_position))
    //                {
    //                    new_pos = true;
    //                    float prob = move_prob.DivisionProbability(pos, tumoral_cell_radio, Utils.EuclideanDistance(tumor_stem_cell.pos, pos), EnvironmentLogic.tumor.new_cells_count);
    //                    if (prob >= 0.5)
    //                    {
    //                        return pos;
    //                    }
    //                    return null;

    //                    //Aplicar una probabilidad para dividirse a una posicion x
    //                }
    //            }
    //        }
    //        return null;
    //    }

    //}
}
