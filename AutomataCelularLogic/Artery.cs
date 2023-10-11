using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class Artery
    {
        public Pos pos;

        public Artery next;
        public Artery before;

        public Cell astrocyte;
        public List<Layer> layer_list;
        public Cell endothelial_cell;
        public Artery(Pos pos, Cell astrocyte, Cell endothelial)
        {
            if (astrocyte != null)
                this.astrocyte = astrocyte;
            else
                this.astrocyte = null;

            endothelial_cell = endothelial;

            //this.before = before;
            this.pos = pos;
            //this.astrocyte = astrocyte;

            layer_list.Add(new GlialBasalLamina());
            layer_list.Add(new VascularBasalLamina());
            layer_list.Add(new SmoothVesselCells(5));
            layer_list.Add(new EndothelialBasalLamina());
        }
    }
    public abstract class Layer
    {
        Probability prob;
    }
    public class GlialBasalLamina: Layer
    {
        public GlialBasalLamina()
        {

        }
    }

    public class VascularBasalLamina: Layer
    {

    }
    public class EndothelialBasalLamina: Layer
    {

    }
    public class SmoothVesselCells: Layer
    {
        int size;
        public SmoothVesselCells(int size)
        {
            this.size = size;
        }
    }
}
