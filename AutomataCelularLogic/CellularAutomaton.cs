using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{ 
    public class CellularAutomaton
    {
        //List<Cell> cell_space;
        public Cell[,,] space;
        public Tumor tumor;

        public Cell tumor_stem_cell;

        public static int stem_cells_count = 150;
        public static int astrocytes_count = 100;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 100;

        public Probability move_prob;

        public List<Cell> closer_cells_to_vessels;

        public Dictionary<Pos, Cell> pos_cell_dict;
        public Dictionary<Pos, Cell> pos_artery_dict;
        public Dictionary<Pos, Pos> next_stem_position;
        public CellularAutomaton(int x, int y, int z, Probability move_prob, int avascular_carrying_capacity, int vascular_carrying_capacity, int growth_rate, int initial_population)
        {
            space = new Cell[x, y, z];
            this.move_prob = move_prob;
            //this.cell_space = cell_space;
            next_stem_position = new Dictionary<Pos, Pos>();
            pos_cell_dict = new Dictionary<Pos, Cell>();
            pos_artery_dict = new Dictionary<Pos, Cell>();
            closer_cells_to_vessels = new List<Cell>();
            StartCellularLifeInTheBrain();
            CreateTumor(avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);

            //foreach (var item in space)
            //{
            //    if(item.behavior_state == CellState.StemCell)
            //        Console.WriteLine(item.behavior_state);
            //}

            UpdateNeighborhood();

            //foreach (Cell cell in space)
            //{
            //    if (cell.behavior_state == CellState.StemCell)
            //    {
            //        Console.WriteLine("Celula");
            //        Console.WriteLine(cell.behavior_state);
            //        Console.WriteLine(cell.neighborhood.Count);
            //    }
            //}
            //foreach (var item in next_stem_position)
            //{

            //}
            //foreach (var pos in next_stem_position.Keys)
            //{
            //    Cell cell = pos_cell_dict[pos];
            //    if (cell.behavior_state == CellState.StemCell)
            //    {
            //        Console.WriteLine("Celula");
            //        Console.WriteLine(cell.behavior_state);
            //        Console.WriteLine(cell.neighborhood.Count);
            //    }
            //}
        }

        public void StartCellularLifeInTheBrain()
        {
            
            StartTumoraCell();
            
            CreateCells(CellState.StemCell, CellLocationState.MatrixExtracelular, stem_cells_count);
            CreateCells(CellState.Astrocyte, CellLocationState.MatrixExtracelular, astrocytes_count);
            CreateCells(CellState.Neuron, CellLocationState.MatrixExtracelular, neuron_count);

            CreateBloodVessels();

            RellenarHuecos();

        }

        public void UpdateNeighborhood()
        {
            foreach (Cell cell in space)
            {
                cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                if(cell.behavior_state != CellState.nothing)
                    pos_cell_dict.Add(cell.pos, cell);
                if (cell.behavior_state == CellState.StemCell)
                    next_stem_position.Add(cell.pos, null);
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
            tumor_stem_cell = new Cell(new Pos(15, 15, 15), CellState.TumoralCell, CellLocationState.MatrixExtracelular);
            //tumor_stem_cell.neighborhood = Utils.GetMooreNeighbours3D(tumor_stem_cell.pos, space);
            //pos_cell_dict.Add(tumor_stem_cell.pos, tumor_stem_cell);

            space[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y, tumor_stem_cell.pos.Z] = tumor_stem_cell;
            
        }
        public void CreateTumor(int avascular_carrying_capacity, int vascular_carrying_capacity, int growth_rate, int initial_population)
        {
            tumor = new Tumor(tumor_stem_cell, avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
        }

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

                //pos_cell_dict.Add(new_pos, cell);
            }
        }


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
        public void CreateBloodVessels()
        {
            for (int i = 0; i < blood_vessels_count; i++)
            {
                Pos new_pos;
                do
                {
                    new_pos = Utils.GetRandomPosition(0, space.GetLength(0), 0, space.GetLength(1), 0, space.GetLength(2));
                }
                while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

                Cell cell = new Cell(new_pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                space[new_pos.X, new_pos.Y, new_pos.Z] = cell;
            }
            #region comentarios
            //Pos pos1 = Utils.GetRandomPosition(0, space.GetLength(0), 0, space.GetLength(1), 0, space.GetLength(2));
            //Pos pos2 = null;

            //Cell endothelial_cell = new Cell(pos, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
            //endothelial_cell_list.Add(endothelial_cell);
            //artery_list.Add(new Artery(endothelial_cell.pos, null, endothelial_cell));

            //Cell cell = new Cell(pos1, CellState.nothing, CellLocationState.GlialBasalLamina);

            //for (int i = 1; i <= blood_vessels_count; i++)
            //{
            //    pos2 = new Pos(pos1.X + 2, pos1.Y, pos1.Z);
            //    pos1 = new Pos(pos1.X + 1, pos1.Y, pos1.Z);

            //    if (!pos_cell_dict.ContainsKey(pos1) && !pos_cell_dict.ContainsKey(pos2) && !pos_artery_dict.ContainsKey(pos1) && !pos_artery_dict.ContainsKey(pos2))
            //    {
            //        //Cell endothelial_cell = new Cell(pos1, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
            //        //endothelial_cell_list.Add(endothelial_cell);

            //        cell = new Cell(pos2, CellState.nothing, CellLocationState.GlialBasalLamina);
            //        //Artery artery = new Artery(endothelial_cell.pos, pos2, null, endothelial_cell);
            //        //artery_list.Add(artery);
            //        pos_artery_dict.Add(pos1, cell);
            //        pos_artery_dict.Add(pos2, cell);
            //    }
            //    pos1 = pos2;
            //    int r = Utils.rdm.Next(0, 2);
            //    Cell astrocyte;
            //    if (r == 1)
            //    {
            //        //astrocyte = CreateAstrocyteCell();

            //        Pos pos = Utils.GetAdjacentPosition(astrocyte.pos, pos_cell_dict);
            //        if (pos != null)
            //        {
            //            Cell endothelial_cell = new Cell(pos, new EndothelialCellBehavior(), new ClassicProbability(), LocationStatus.EndothelialBasalLamina);
            //            endothelial_cell_list.Add(endothelial_cell);
            //            pos_cell_dict.Add(pos, astrocyte);

            //            artery_list.Add(new Artery(endothelial_cell.pos, astrocyte, endothelial_cell));
            //        }
            //        //VAMOS ASUMIR POR AHORA QUE QUE LA VARIBALE ASTROCITO ES DISTINTO DE NULL Y QUE EXISTE UNA POSICION ADYACENTE
            //    }
            //    else
            //    {
            //        Cell endothelial_cell = CreateEndothelialCell();
            //        artery_list.Add(new Artery(endothelial_cell.pos, null, endothelial_cell));
            //    }
            //}
            #endregion

        }

        public void SearchCloserStemPosition()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (var key_value in next_stem_position)
            {
                Pos pos = key_value.Key;
                //Cell cell = pos_cell_dict[pos];
                //List<Cell> neig = d;
                //Console.WriteLine("{0} {1} {2}",key_value.Key.X, key_value.Key.Y, key_value.Key.Z);

                //Console.WriteLine(neig.Count);

                Pos closer_pos = Utils.MinDistancePos(pos_cell_dict[pos].neighborhood, tumor_stem_cell.pos);
                if (Utils.EuclideanDistance(tumor_stem_cell.pos, closer_pos) >= 5)
                    temp.Add(key_value.Key, closer_pos);
                //else
                //    Console.WriteLine("estoy aqui");
                //if (closer_pos != tumor_stem_cell.pos)

            }
            next_stem_position = temp;
        }

        public void Update()
        {
            tumor.VerhulstEquation();

            ObtainCellsPositionsToDivide();

            SearchCloserStemPosition();

            foreach (Cell cell in space)
            {
                UpdateState(cell);
            }

            ClearCloserCellsToVessels();
            UpdateNextStemPositionDict();
        }

        public void ClearCloserCellsToVessels()
        {
            closer_cells_to_vessels = new List<Cell>();
        }

        public void ObtainCellsPositionsToDivide()
        {
            foreach (var tumor_cell in tumor.cell_list)
            {
                Tuple<Cell, Cell> closer_cell_to_artery = ObtenerVecinoCercanoAVasos(tumor_cell.neighborhood);
                if (closer_cell_to_artery != null)
                    closer_cells_to_vessels.Add(closer_cell_to_artery.Item1);
            }
        }

        public Tuple<Cell, Cell> ObtenerVecinoCercanoAVasos(List<Cell> cell_list)
        {
            int min = int.MaxValue;
            Tuple<Cell, Cell> closer_cell_to_artery = null;
            foreach (var item in cell_list)
            {
                if (item.behavior_state == CellState.nothing)
                {
                    int distance_min = int.MaxValue;
                    Cell artery = null;
                    foreach (var item2 in pos_artery_dict)
                    {
                        int distance = Utils.EuclideanDistance(item.pos, item2.Key);
                        if (distance < distance_min)
                        {
                            distance_min = distance;
                            artery = pos_artery_dict[item2.Key];
                        }
                    }
                    if(min >= distance_min)
                    {
                        min = distance_min;
                        closer_cell_to_artery = new Tuple<Cell, Cell>(item, artery);
                    }
                }
            }
            return closer_cell_to_artery;
        }

        public void UpdateNextStemPositionDict()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (Pos pos in next_stem_position.Keys)
            {
                if(!temp.ContainsKey(next_stem_position[pos]))
                    temp.Add(next_stem_position[pos], null);
            }
            next_stem_position = temp;
        }

        public void UpdateState(Cell cell)
        {
            int tumoral_cells_count = cell.TumoralCellCount();
            if (cell.behavior_state == CellState.Astrocyte || cell.behavior_state == CellState.Neuron)
            {
                if(Utils.rdm.NextDouble() < move_prob.ContamineProbability(cell, cell.neighborhood, tumor.new_cells_count))
                {
                    cell.behavior_state = CellState.TumoralCell;

                    tumor.UpdateNewCellCount();
                }
                //si no se cumple eso entonces sigue con el mismo estado
            }
            else if (cell.behavior_state == CellState.TumoralCell)
            {
                //de cierta manera aqui irian las celulas que se convierte en celulas necroticas y divi
                //if (tumoral_cells_count == 26)
                //    throw new Exception();
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
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                    //pos_cell_dict.Add(cell.pos, cell);
                }
                else if(CloserToTumoralCell(cell))
                {
                    //aqui es donde se supone que hay que analizar las celulas que tiene alrededor de un radio de 5 celdas
                    cell.behavior_state = CellState.TumoralCell;
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
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
                else if(closer_cells_to_vessels.Contains(cell) && Utils.rdm.NextDouble() < move_prob.DivisionProbability(cell, cell.neighborhood, tumor.new_cells_count))
                {
                    cell.behavior_state = CellState.TumoralCell;
                    pos_cell_dict.Add(cell.pos, cell);
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                }
                else if(Utils.rdm.NextDouble() < move_prob.DivisionProbability(cell, cell.neighborhood, tumor.new_cells_count))
                {
                    cell.behavior_state = CellState.TumoralCell;
                    pos_cell_dict.Add(cell.pos, cell);
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                }
            }
        }

        public bool CloserToTumoralCell(Cell cell)
        {
            return Utils.EuclideanDistance(cell.pos, tumor_stem_cell.pos) <= 5;
        }
    }
}
