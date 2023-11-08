using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class Artery: Cell
    {
        //    public Pos pos1;
        //    public Pos pos2;

        //    public Artery next;
        //    public Artery before;

        //    public Cell astrocyte;
        //    public List<Layer> layer_list;
        //    public Cell endothelial_cell;
        //    public Artery(Pos pos1, Pos pos2, Cell astrocyte, Cell endothelial)
        //    {
        //        if (astrocyte != null)
        //            this.astrocyte = astrocyte;
        //        else
        //            this.astrocyte = null;

        //        endothelial_cell = endothelial;

        //        //this.before = before;
        //        this.pos1 = pos1;
        //        this.pos2 = pos2;
        //        //this.astrocyte = astrocyte;
        //        layer_list = new List<Layer>();
        //        layer_list.Add(new GlialBasalLamina());
        //        layer_list.Add(new VascularBasalLamina());
        //        layer_list.Add(new SmoothVesselCells(5));
        //        layer_list.Add(new EndothelialBasalLamina());
        //    }
        //}
        //public abstract class Layer
        //{
        //    Probability prob;
        //}
        //public class GlialBasalLamina: Layer
        //{
        //    public GlialBasalLamina()
        //    {

        //    }
        //}

        //public class VascularBasalLamina: Layer
        //{

        //}
        //public class EndothelialBasalLamina: Layer
        //{

        //}
        //public class SmoothVesselCells: Layer
        //{
        //    int size;
        //    public SmoothVesselCells(int size)
        //    {
        //        this.size = size;
        //    }
        public Artery(Pos pos, CellState behavior_state, CellLocationState loca_state) : base(pos, behavior_state, loca_state)
        {
        }
    }

    public class Arteriole : Cell
    {
        public Arteriole(Pos pos, CellState behavior_state, CellLocationState loca_state) : base(pos, behavior_state, loca_state)
        {
        }
    }

    public class Capillary : Cell
    {
        public Capillary(Pos pos, CellState behavior_state, CellLocationState loca_state) : base(pos, behavior_state, loca_state)
        {
        }
    }
}
