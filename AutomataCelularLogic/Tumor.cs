using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    //POR AHORA VA A SER UN ENUM PARA LAS ETAPAS Y UN METODO PARA CADA ETAPA, SI ALGO DESPUES SE CAMBIA
    public enum VascularizationMechanism
    {
        InitialGrowth,
        Angiogenesis,
        VascularCooption,
        Transdifferentiation
    }
    public enum TumosStage
    {
        Avascular,
        Vascular
    }

    public class Tumor
    {
        public Cell ini_cell;
        public List<Cell> cell_list;
        public List<Cell> migra_list;
        public VascularizationMechanism vasc_mecha;
        public TumosStage tumor_stage;

        //public int cell_proliferation;
        public int avascular_carrying_capacity;
        public int vascular_carrying_capacity;
        public double growth_rate;
        public int initial_population;
        public int actual_population;

        public int new_cells_count;

        public int radio;

        public int time;

        public Tumor(Cell cell/*, int radio*/, int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            ini_cell = cell;
            cell_list = new List<Cell>();
            migra_list = new List<Cell>();
            tumor_stage = TumosStage.Avascular;
            vasc_mecha = VascularizationMechanism.InitialGrowth;

            //this.radio = radio;
            this.avascular_carrying_capacity = avascular_carrying_capacity;
            this.vascular_carrying_capacity = vascular_carrying_capacity;
            this.growth_rate = growth_rate;
            this.initial_population = initial_population;
            actual_population = this.initial_population;
            this.time = 0;
            new_cells_count = 0;
        }

        //public List<Cell> AddNewTumorCells(Dictionary<Pos, Cell> pos_cell_dict)
        //{
        //    VerhulstEquation();
        //    List<Cell> cell_div_list = new List<Cell>();

        //    while(new_cells > 0)
        //    {
        //        //OBTENER UNA POSICION ALEATORIA EN LA LISTA DE CELULAS
        //        //int i = Utils.rdm.Next(0, cell_list.Count);

        //        //cell_list[i].actual_action = CellActions.Division;
        //        //Pos pos = cell_list[i].Division(pos_cell_dict, radio, ini_cell);

        //        //if (pos != null)
        //        //{
        //        //    Cell cell = new Cell(pos, new TumorCellBehavior(), new ClassicProbability());
        //        //    cell_list.Add(cell);
        //        //    cell_div_list.Add(cell);
        //        //    new_cells--;
        //        //}
        //    }
        //    return cell_div_list;
        //}

        public void VerhulstEquation()
        {
            double loading_capacity;
            actual_population = cell_list.Count + 1;

            if (tumor_stage == TumosStage.Avascular)
                loading_capacity = avascular_carrying_capacity;
            else loading_capacity = vascular_carrying_capacity;

            new_cells_count = (int)VerhulstGrowth(actual_population, growth_rate, loading_capacity, time);
        }
        static double VerhulstGrowth(double actual_population, double growth_rate, double loading_capacity, double time)
        {
            double dP = growth_rate * actual_population * (1 - actual_population / loading_capacity) * time;
            return actual_population + dP;
        }
        //double LogisticGrowth(double P0, double K, double r, double t)
        //{
        //    return (P0 * K * Math.Exp(r * t)) / ((K - P0) + P0 * Math.Exp(r * t));
        //}
        public void UpdateNewCellCount()
        {
            new_cells_count--;
        }
    }
}
