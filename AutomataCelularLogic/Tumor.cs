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
        public VascularizationMechanism vasc_mecha;
        public TumosStage tumor_stage;

        //public int cell_proliferation;
        public int avascular_carrying_capacity;
        public int vascular_carrying_capacity;
        public double growth_rate;
        public int initial_population;
        public int actual_population;

        public int radio;

        public int time;

        public Tumor(Cell cell, int radio, int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            ini_cell = cell;
            cell_list = new List<Cell>();
            tumor_stage = TumosStage.Avascular;
            vasc_mecha = VascularizationMechanism.InitialGrowth;

            this.radio = radio;
            this.avascular_carrying_capacity = avascular_carrying_capacity;
            this.vascular_carrying_capacity = vascular_carrying_capacity;
            this.growth_rate = growth_rate;
            this.initial_population = initial_population;
            actual_population = this.initial_population;
            this.time = 0;
        }

        public List<Cell> AddNewTumorCells(Dictionary<Pos, Cell> pos_cell_dict)
        {
            int new_cells = VerhulstEquation();
            List<Cell> cell_div_list = new List<Cell>();

            while(new_cells > 0)
            {
                //OBTENER UNA POSICION ALEATORIA EN LA LISTA DE CELULAS
                int i = Utils.rdm.Next(0, cell_list.Count);

                cell_list[i].actual_action = CellActions.division;
                Pos pos = cell_list[i].Division(pos_cell_dict, radio, ini_cell);

                if (pos != null)
                {
                    Cell cell = new Cell(pos, new TumorCellBehavior(), new ClassicProbability());
                    cell_list.Add(cell);
                    cell_div_list.Add(cell);
                    new_cells--;
                }
            }
            return cell_div_list;
        }

        public int VerhulstEquation()
        {
            double loading_capacity;

            if (tumor_stage == TumosStage.Avascular)
                loading_capacity = avascular_carrying_capacity;
            else loading_capacity = vascular_carrying_capacity;

            return (int)(growth_rate * actual_population * (1 - actual_population / loading_capacity));
        }
    }
}
