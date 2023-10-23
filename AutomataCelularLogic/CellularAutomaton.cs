using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{ 
    public class CellularAutomaton
    {
        List<Cell> cell_space;
        public Cell[,,] space;

        public Cell tumor_stem_cell;

        public static int stem_cells_count = 15;
        public static int astrocytes_count = 20;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 20;

        public Dictionary<Pos, Cell> pos_cell_dict; 

        public Dictionary<Pos, Pos> next_stem_position;
        public CellularAutomaton(int x, int y, int z)
        {
            space = new Cell[x, y, z];
            //this.cell_space = cell_space;
            next_stem_position = new Dictionary<Pos, Pos>();
            pos_cell_dict = new Dictionary<Pos, Cell>();
            StartCellularLifeInTheBrain();
            UpdateNeighborhood();
        }

        public void StartCellularLifeInTheBrain()
        {
            
            StartTumoraCell();

            CreateCells(CellState.StemCell, CellLocationState.MatrixExtracelular, stem_cells_count);
            CreateCells(CellState.Astrocyte, CellLocationState.MatrixExtracelular, astrocytes_count);
            CreateCells(CellState.Neuron, CellLocationState.MatrixExtracelular, neuron_count);

            RellenarHuecos();

        }

        public void UpdateNeighborhood()
        {
            foreach (Cell cell in space)
            {
                cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);
            }
        }

        //OJO CAMBIAR NOMBREEEEEE
        public void RellenarHuecos()
        {
            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        if(space[i, j, k] == null)
                        {
                            space[i, j, k] = new Cell(new Pos(i, j, k), CellState.nothing, CellLocationState.MatrixExtracelular);
                        }
                    }
                }
            }
        }

        public void StartTumoraCell()
        {
            //Cell tumor_stem_cell = new Cell(new Pos(15, 15, 15), new TumorCellBehavior(), new ClassicProbability(), LocationStatus.MatrixExtracelular);
            tumor_stem_cell = new Cell(new Pos(15, 15, 15), CellState.TumoralCell, CellLocationState.MatrixExtracelular);
            tumor_stem_cell.neighborhood = Utils.GetMooreNeighbours3D(tumor_stem_cell.pos, space);
            pos_cell_dict.Add(tumor_stem_cell.pos, tumor_stem_cell);

            space[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y, tumor_stem_cell.pos.Z] = tumor_stem_cell;
            
        }
        //public static void CreateTumor()
        //{
        //    tumor = new Tumor(tumor_stem_cell, tumoral_cell_radio, avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
        //}
        //public void CreateStemCells()
        //{
        //    for (int i = 0; i < stem_cells_count; i++)
        //    {
        //        Pos new_pos;
        //        do
        //        {
        //            new_pos = Utils.GetRandomPosition(0, space.GetLength(0), 0, space.GetLength(1), 0, space.GetLength(2));
        //        }
        //        while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

        //        Cell stem_cell = new Cell(new_pos, CellState.StemCell, CellLocationState.MatrixExtracelular);
        //        space[new_pos.X, new_pos.Y, new_pos.Z] = stem_cell;
        //        next_stem_position.Add(new_pos, null);


        //        pos_cell_dict.Add(new_pos, stem_cell);
        //    }
        //}
        public void CreateCells(CellState cell_state, CellLocationState loca_state, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Pos new_pos;
                do
                {
                    new_pos = Utils.GetRandomPosition(0, space.GetLength(0), 0, space.GetLength(1), 0, space.GetLength(2));
                }
                while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

                Cell cell = new Cell(new_pos, cell_state, loca_state);
                //cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                space[new_pos.X, new_pos.Y, new_pos.Z] = cell;

                if(cell_state == CellState.StemCell)
                    next_stem_position.Add(new_pos, null);


                pos_cell_dict.Add(new_pos, cell);
            }
        }
        //public static void CreateNeuronCells()
        //{
        //    for (int i = 0; i < neuron_count; i++)
        //    {
        //        Pos new_pos;
        //        do
        //        {
        //            new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
        //        }
        //        while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

        //        neuron_cell_list.Add(new Cell(new_pos, new NeuronCellBehavior(), new ClassicProbability(), LocationStatus.MatrixExtracelular));
        //        pos_cell_dict.Add(new_pos, neuron_cell_list[neuron_cell_list.Count - 1]);
        //    }
        //}
        //public static void CreateAstrocyteCell()
        //{
        //    for (int i = 0; i < astrocytes_count; i++)
        //    {

        //        Pos new_pos;
        //        do
        //        {
        //            new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
        //        }
        //        while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

        //        Cell cell = new Cell(new_pos, new AstrocyteCellBehavior(), new ClassicProbability(), LocationStatus.MatrixExtracelular);
        //        astrocyte_cell_list.Add(cell);
        //        pos_cell_dict.Add(new_pos, cell);
        //    }
        //    //return cell;
        //}
        //public static Cell CreateEndothelialCell()
        //{
        //    Pos new_pos;
        //    do
        //    {
        //        new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
        //    }
        //    while (pos_cell_dict.ContainsKey(new_pos));

        //    Cell cell = new Cell(new_pos, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
        //    endothelial_cell_list.Add(cell);
        //    pos_cell_dict.Add(new_pos, cell);
        //    return cell;
        //}
        //public static void CreateBloodVessels()
        //{
        //    Pos pos1 = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
        //    Pos pos2 = null;
        //    //Cell endothelial_cell = new Cell(pos, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
        //    //endothelial_cell_list.Add(endothelial_cell);
        //    //artery_list.Add(new Artery(endothelial_cell.pos, null, endothelial_cell));

        //    for (int i = 1; i <= blood_vessels_count; i++)
        //    {
        //        pos2 = new Pos(pos1.X + 2, pos1.Y, pos1.Z);
        //        pos1 = new Pos(pos1.X + 1, pos1.Y, pos1.Z);

        //        if (!pos_cell_dict.ContainsKey(pos1) && !pos_cell_dict.ContainsKey(pos2) && !pos_artery_dict.ContainsKey(pos1) && !pos_artery_dict.ContainsKey(pos2))
        //        {
        //            Cell endothelial_cell = new Cell(pos1, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
        //            endothelial_cell_list.Add(endothelial_cell);

        //            Artery artery = new Artery(endothelial_cell.pos, pos2, null, endothelial_cell);
        //            artery_list.Add(artery);
        //            pos_artery_dict.Add(pos1, artery);
        //            pos_artery_dict.Add(pos2, artery);
        //        }
        //        pos1 = pos2;
        //        //int r = Utils.rdm.Next(0, 2);
        //        //Cell astrocyte;
        //        //if (r == 1)
        //        //{
        //        //    //astrocyte = CreateAstrocyteCell();

        //        //    Pos pos = Utils.GetAdjacentPosition(astrocyte.pos, pos_cell_dict);
        //        //    if (pos != null)
        //        //    {
        //        //        Cell endothelial_cell = new Cell(pos, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
        //        //        endothelial_cell_list.Add(endothelial_cell);
        //        //        pos_cell_dict.Add(pos, astrocyte);

        //        //        artery_list.Add(new Artery(endothelial_cell.pos, astrocyte, endothelial_cell));
        //        //    }
        //        //    //VAMOS ASUMIR POR AHORA QUE QUE LA VARIBALE ASTROCITO ES DISTINTO DE NULL Y QUE EXISTE UNA POSICION ADYACENTE
        //        //}
        //        //else
        //        //{
        //        //    Cell endothelial_cell = CreateEndothelialCell();
        //        //    artery_list.Add(new Artery(endothelial_cell.pos, null, endothelial_cell));
        //        //}
        //    }

        //}

        public void SearchCloserStemPosition()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (var key_value in next_stem_position)
            {
                Pos pos = key_value.Key;
                Pos closer_pos = Utils.MinDistancePos(pos_cell_dict[pos].neighborhood, tumor_stem_cell.pos);
                if(Utils.EuclideanDistance(tumor_stem_cell.pos, closer_pos) >= 5)
                    temp.Add(key_value.Key, closer_pos);
                else
                    Console.WriteLine("estoy aqui");
                //if (closer_pos != tumor_stem_cell.pos)
                    
            }
            next_stem_position = temp;
        }

        public void Update()
        {
            
            SearchCloserStemPosition();

            foreach (Cell cell in space)
            {
                if (cell.behavior_state == CellState.nothing)
                    UpdateState(cell);
                else
                    UpdateState(cell);
            }

            UpdateNextStemPositionDict();
        }

        public void UpdateNextStemPositionDict()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (Pos pos in next_stem_position.Keys)
            {
                temp.Add(next_stem_position[pos], null);
            }
            next_stem_position = temp;
        }

        public void UpdateState(Cell cell)
        {
            int tumoral_cells_count = cell.TumoralCellCount();
            if (cell.behavior_state == CellState.Astrocyte || cell.behavior_state == CellState.Neuron)
            {
                if (tumoral_cells_count >= 10)
                {
                    cell.behavior_state = CellState.TumoralCell;
                }
                //si no se cumple eso entonces sigue con el mismo estado
            }
            else if (cell.behavior_state == CellState.TumoralCell)
            {
                //de cierta manera aqui irian las celulas que se convierte en celulas necroticas y divi
                if (tumoral_cells_count == 26)
                    throw new Exception();
            }
            else if (cell.behavior_state == CellState.StemCell)
            {
                if(next_stem_position.ContainsKey(cell.pos))
                {
                    pos_cell_dict.Remove(cell.pos);
                    cell.behavior_state = CellState.nothing;
                    
                }
                else if (tumoral_cells_count >= 1)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    //pos_cell_dict.Add(cell.pos, cell);
                }
                else if(CloserToTumoralCell(cell))
                {
                    //aqui es donde se supone que hay que analizar las celulas que tiene alrededor de un radio de 5 celdas
                    cell.behavior_state = CellState.TumoralCell;
                    //pos_cell_dict.Add(cell.pos, cell);
                }
            }
            else if (cell.behavior_state == CellState.nothing)
            {
                if(next_stem_position.ContainsValue(cell.pos))
                {
                    cell.behavior_state = CellState.StemCell;
                    pos_cell_dict.Add(cell.pos, cell);
                }
                else if (tumoral_cells_count >= 5)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    pos_cell_dict.Add(cell.pos, cell);
                }
            }
        }

        public bool CloserToTumoralCell(Cell cell)
        {
            return Utils.EuclideanDistance(cell.pos, tumor_stem_cell.pos) <= 5;
        }
    }
}
