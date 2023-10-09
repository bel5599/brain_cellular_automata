using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class Artery
    {
        Artery next;
        Artery before;
        List<Cell> astrocyte_list;
        List<Layer> layer_list;
        List<Cell> endothelial_list;
        public Artery()
        {

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
