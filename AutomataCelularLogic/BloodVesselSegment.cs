using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public enum RadioClassification
    {
        ImmatureVessel,
        MatureVessel
    }
    public class BloodVesselSegment
    {
        public BloodVessel blood_vessel1;
        public BloodVessel blood_vessel2;

        public double radius;//R
        public double mean_length;
        public double pressure_collapse;//Pc
        public double pressure_collapse_min;//Pc_min

        public double intravascular_pressure;//Pv
        public double interstitial_pressure;//Pi
        public double hydraulic_permeability;//Lp

        public RadioClassification radio_clasf;
        //public double aver_osmotic_reflec_coeff;//ot
        
        //public double pressure_of_plasma;//pi v
        //public double 

        public StrahlerOrder order;

        public BloodVesselSegment(BloodVessel blood_vessel1, BloodVessel blood_vessel2, StrahlerOrder order, double mean_diameter, double mean_length, double pressure_collapse)
        {
            this.blood_vessel1 = blood_vessel1;
            this.blood_vessel2 = blood_vessel2;
            this.order = order;

            this.radius = Math.Sqrt(mean_diameter);
            this.mean_length = mean_length;
            this.pressure_collapse = pressure_collapse;
            this.pressure_collapse_min = pressure_collapse;

            radio_clasf = RadioClassification.MatureVessel;
            
        }
    }
}
