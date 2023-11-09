using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{ 
    public class CellularAutomaton
    {
        //variables relacionadas directamente con el automata 
        public Probability move_prob;
        public Cell[,,] space;
        public Tumor tumor;
        public Cell tumor_stem_cell;

        public Cell[] random_space;

        //informacion de la cantidad de celulas
        public static int stem_cells_count = 100;
        public static int astrocytes_count = 50;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 50;

        public int cant_cell = 50;

        //estructuras
        public Dictionary<Pos, Cell> pos_cell_dict;

        //public Dictionary<Pos, Cell> pos_vessel_dict;

        public Dictionary<Pos, Artery> pos_artery_dict;

        //public Dictionary<Pos, Arteriole> pos_arteriole_dict;
        //public Dictionary<Pos, Capillary> pos_capillary_dict;
        //public Dictionary<Cell, Tuple<List<Arteriole>, Capillary>> artery_arteriole_dict;

        public Dictionary<Pos, Pos> next_stem_position;
        public Dictionary<Pos, Pos> next_migratory_position;


        public List<Cell> closer_cells_to_vessels;


        public List<Cell> vasos_cooptados;

        public List<Cell> new_artery_list;
        //public List<Cell> new_arteriole_list;
        //public List<Cell> new_capillary_list;

        Dictionary<Cell, Cell> vasos_en_crecimiento_dict;
        public List<Tuple<Cell, Cell>> vasos_en_crecimiento;
        public List<Cell> posible_migratory_cells;
        public List<Cell> migratory_cells;

        public CellularAutomaton(int x, int y, int z, Probability move_prob, int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            //space = new Cell[x, y, z];
            random_space = new Cell[x * y * z];

            this.move_prob = move_prob;
            InicializarListas();

            //StartCellularLifeInTheBrain();
            World world = new World(x, y, z, 1, stem_cells_count, astrocytes_count, neuron_count);
            space = world.space;
            tumor_stem_cell = world.tumor_stem_cell;
            CreateTumor(avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);

            UpdateNeighborhood();
            AleatorizarCelulas();
        }

        void InicializarListas()
        {
            next_stem_position = new Dictionary<Pos, Pos>();
            pos_cell_dict = new Dictionary<Pos, Cell>();

            //pos_vessel_dict = new Dictionary<Pos, Cell>();

            pos_artery_dict = new Dictionary<Pos, Artery>();
            //artery_arteriole_dict = new Dictionary<Cell, Tuple<List<Arteriole>, Capillary>>();
            //pos_arteriole_dict = new Dictionary<Pos, Arteriole>();
            //pos_capillary_dict = new Dictionary<Pos, Capillary>();

            vasos_en_crecimiento = new List<Tuple<Cell, Cell>>();
            vasos_en_crecimiento_dict = new Dictionary<Cell, Cell>();

            migratory_cells = new List<Cell>();
            posible_migratory_cells = new List<Cell>();
            next_migratory_position = new Dictionary<Pos, Pos>();

            vasos_cooptados = new List<Cell>();
            new_artery_list = new List<Cell>();
            //new_arteriole_list = new List<Cell>();
            //new_capillary_list = new List<Cell>();

            closer_cells_to_vessels = new List<Cell>();
        }
        public void AleatorizarCelulas()
        {
            int i = 0;
            int count = random_space.Length;
            Dictionary<Cell, int> dict = new Dictionary<Cell, int>();
            while(count > 0)
            {
                int x = Utils.rdm.Next(0, space.GetLength(0));
                int y = Utils.rdm.Next(0, space.GetLength(1));
                int z = Utils.rdm.Next(0, space.GetLength(2));
                if(!dict.ContainsKey(space[x,y,z]))
                {
                    random_space[i] = space[x, y, z];
                    dict.Add(random_space[i], i);
                    i++;
                    count--;
                }
            }
        }

        public void UpdateNeighborhood()
        {
            foreach (Cell cell in space)
            {
                cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                if(cell.behavior_state != CellState.nothing)
                    pos_cell_dict.Add(cell.pos, cell);
                if (cell.behavior_state == CellState.StemCell)
                    next_stem_position.Add(cell.pos, Utils.NullPos());
            }
        }

        //OJO CAMBIAR NOMBREEEEEE

        #region Creation
        public void CreateTumor(int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            tumor = new Tumor(tumor_stem_cell, avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
        }

        #endregion


        public void SearchCloserStemPosition()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (var key_value in next_stem_position)
            {
                Pos pos = key_value.Key;
                //Cell cell = pos_cell_dict[pos];
                //List<Cell> neig = cell.neighborhood;
                Pos closer_pos = Utils.MinDistancePos(pos_cell_dict[pos].neighborhood, tumor_stem_cell.pos);
                int distance = Utils.EuclideanDistance(tumor_stem_cell.pos, closer_pos);
                if (space[closer_pos.X, closer_pos.Y, closer_pos.Z].behavior_state == CellState.nothing && space[closer_pos.X, closer_pos.Y, closer_pos.Z].loca_state == CellLocationState.MatrixExtracelular)
                {
                    if (distance >= 5)
                        temp.Add(key_value.Key, closer_pos);
                    else if (distance != 0 && Utils.rdm.Next(0, 2) == 1)
                        temp.Add(key_value.Key, closer_pos);
                }
            }
            next_stem_position = temp;
        }

        public void ClearCloserCellsToVessels()
        {
            closer_cells_to_vessels = new List<Cell>();
        }



        #region ObtenerPosicionesCercanaALosVasosParaContaminar
        public void ObtainCellsPositionsToDivide()
        {
            foreach (var tumor_cell in tumor.cell_list)
            {
                Tuple<Cell, Cell> closer_cell_to_artery = ObtenerVecinoCercanoAVasos(tumor_cell.neighborhood);
                if (closer_cell_to_artery != null)
                    closer_cells_to_vessels.Add(closer_cell_to_artery.Item1);
            }
        }

        //ARREGLAR AQUI PARA TENER EN CUENTA LAS ARTERIOLAS TAMBIEN
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
                        if (!vasos_cooptados.Contains(item2.Value))
                        {
                            int distance = Utils.EuclideanDistance(item.pos, item2.Key);
                            if (distance < distance_min)
                            {
                                distance_min = distance;
                                artery = pos_artery_dict[item2.Key];
                            }
                        }
                    }
                    if (min >= distance_min)
                    {
                        min = distance_min;
                        closer_cell_to_artery = new Tuple<Cell, Cell>(item, artery);
                    }
                }
            }
            //Console.WriteLine("{0} {1} {2}", closer_cell_to_artery.Item1.pos.X, closer_cell_to_artery.Item1.pos.Y, closer_cell_to_artery.Item1.pos.Z);
            //Console.WriteLine("{0} {1} {2}", closer_cell_to_artery.Item2.pos.X, closer_cell_to_artery.Item2.pos.Y, closer_cell_to_artery.Item2.pos.Z);
            return closer_cell_to_artery;
        }
        #endregion

        #region MigratoryCells
        //buscar posiciones alrededor del tumor y hallar la probabilidad para convertilas en migratorias

        public void UpdateMigratoryCells()
        {
                GetMigratoryCells();

            if(next_migratory_position.Count > 0)
            {
                NextMove();
            }
        }
        public void GetMigratoryCells()
        {
            foreach (var item in tumor.cell_list)
            {
                if (Utils.rdm.NextDouble() < move_prob.MigrateProbability(item, tumor))
                    posible_migratory_cells.Add(item);
            }
        }

        public void NextMove()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (var item in next_migratory_position)
            {
                //aqui se quiere obtener las casillas vacias vecinas
                List<Cell> free_neighbord = Utils.EmptyPositions(pos_cell_dict[item.Key].neighborhood);
                if (free_neighbord.Count > 0)
                {
                    temp.Add(item.Key, free_neighbord[Utils.rdm.Next(0, free_neighbord.Count)].pos);
                }
            }
            next_migratory_position = temp;
        }

        //La idea de las celulas migratorias es moverse a lugares donde exista alta concentracion de nutrientes
        //por tanto lo mas probable es que entre al lumen de los vasos sanguineos
        #endregion

        public bool CloserToTumoralCell(Cell cell)
        {
            return Utils.EuclideanDistance(cell.pos, tumor_stem_cell.pos) <= 5;
        }

        #region CooptionVessels
        //metodo que comprueba si hay vasos cooptados
        public void CooptionVessels()
        {
            //List<Cell> vasos_cooptados_temp = new List<Cell>();
            foreach (var item in pos_artery_dict.Values)
            {
                int count = 0;
                foreach (var cell in item.neighborhood)
                {
                    if (cell.behavior_state == CellState.TumoralCell)
                        count++;
                }
                if (count >= 10)
                {
                    //vasos_cooptados_temp.Add(item);
                    vasos_cooptados.Add(item);
                }
            }
            //return vasos_cooptados_temp;

        }

        public void AnalisisDelEstadoDelTumor()
        {
            //List<Cell> vasos_cooptados = CooptionVessels();

            //hay que tener en cuenta la asfixia
            if(vasos_cooptados.Count >= 3)
            {
                UpdateTumorState();
                //cambiar la direccion en la que busca nutrientes
            }

            //if(vasos_cooptados.Count >= (pos_capillary_dict.Count + pos_arteriole_dict.Count))
            //{
            //    //Hacer Algo
            //    throw new Exception();
            //}
        }
        #endregion


        #region Angiogenesis

        public void Angiogenesis()
        {
            CrearCaminoParaAlcanzarAlTumor();

            ContinuarCrecimiento();
        }

        public void CrearCaminoParaAlcanzarAlTumor()
        {
            List<Tuple<Cell, Cell>> tumorCells_bloodVessels = NewVesselsFormation();

            int i = Utils.rdm.Next(0, tumorCells_bloodVessels.Count);

            Tuple<Cell, Cell> tumorCell_bloodVessel = tumorCells_bloodVessels[i];
            Cell vecino_mas_cercano = VecinoMasCercano(tumorCell_bloodVessel);

            if (tumorCell_bloodVessel.Item2 is Artery)
            {
                new_artery_list.Add(vecino_mas_cercano);
                //Arteriole arteriole = new Arteriole(vecino_mas_cercano.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //pos_arteriole_dict.Add(vecino_mas_cercano.pos, arteriole);
                //space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;

                vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
                //vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(tumorCell_bloodVessel.Item1, arteriole));

                //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
            }
            //else if(tumorCell_bloodVessel.Item2 is Cell)
            //{
            //    new_artery_list.Add(vecino_mas_cercano);
            //    vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
            //}
            //else if(tumorCell_bloodVessel.Item2 is Capillary)
            //{
            //    new_capillary_list.Add(vecino_mas_cercano);
            //    vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
            //}
        }

        //Aqui lo que estoy haciendo es buscando todas las casillas vacias alrededor del tumor para hacer crecer un nuevo vaso hasta esa casilla;
        public List<Tuple<Cell, Cell>> NewVesselsFormation()
        {
            int min = int.MaxValue;
            Cell blood_vessel = null;
            List<Cell> cells = BuscarPosicionesAlBordeDelTumor();

            List<Cell> blood_vessels = new List<Cell>();
            blood_vessels.AddRange(pos_artery_dict.Values);
            //blood_vessels.AddRange(pos_arteriole_dict.Values);
            //blood_vessels.AddRange(pos_capillary_dict.Values);

            List<Tuple<Cell, Cell>> tumorCell_bloodVessel = new List<Tuple<Cell, Cell>>();

            foreach (var item in cells)
            {
                foreach (var item2 in blood_vessels)
                {
                    if (!vasos_cooptados.Contains(item2))
                    {
                        int distance = Utils.EuclideanDistance(item.pos, item2.pos);
                        if (distance < min)
                        {
                            min = distance;
                            blood_vessel = item2;
                        }
                    }

                }
                tumorCell_bloodVessel.Add(new Tuple<Cell, Cell>(item, blood_vessel));
            }
            return tumorCell_bloodVessel;


        }

        public List<Cell> BuscarPosicionesAlBordeDelTumor()
        {
            List<Cell> emptyPosition = new List<Cell>();
            foreach (var item in tumor.cell_list)
            {
                foreach (var item1 in item.neighborhood)
                {
                    if (item1.behavior_state == CellState.nothing)
                        emptyPosition.Add(item1);
                }
            }
            return emptyPosition;
        }

        //hay que ver porque una vez que se esta formando un vaso no puede aparecer en la creacion


        public Cell VecinoMasCercano(Tuple<Cell, Cell> tumorCell_bloodVessel)
        {
            int min = int.MaxValue;
            Cell vecino_mas_cercano = null;
            foreach (var item in tumorCell_bloodVessel.Item2.neighborhood)
            {
                if (item.behavior_state == CellState.nothing && item.loca_state == CellLocationState.MatrixExtracelular)
                {
                    int distance = Utils.EuclideanDistance(tumorCell_bloodVessel.Item1.pos, item.pos);
                    if (distance < min)
                    {
                        min = distance;
                        vecino_mas_cercano = item;
                    }
                }
                //Hay que analizar si cuando se actualiza el automata se cierra el camino porque aparece una celula tumoral en el medio
            }
            return vecino_mas_cercano;
        }

        //una vez se crean los vasos este metodo es el update del crecimiento
        public void ContinuarCrecimiento()
        {
            Dictionary<Cell, Cell> temp = new Dictionary<Cell, Cell>();
            foreach (var item in vasos_en_crecimiento)
            {
                var vecino = VecinoMasCercano(item);
                //if (item.Item2 is Arteriole)
                //{
                //    new_artery_list.Add(vecino);
                //    //Arteriole arteriole = new Arteriole(vecino.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //    //pos_arteriole_dict.Add(vecino.pos, arteriole);
                //    //space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;

                //    temp.Add(item.Item1, vecino);
                //    //vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(item.Item1, arteriole));

                //    //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
                //}
                if (item.Item2 is Artery)
                {
                    new_artery_list.Add(vecino);
                    temp.Add(item.Item1, vecino);
                }
                //else if (item.Item2 is Capillary)
                //{
                //    new_capillary_list.Add(vecino);
                //    temp.Add(item.Item1, vecino);
                //}
            }
            vasos_en_crecimiento_dict = temp;
            
        }
        
        #endregion


        #region Update
        public void Update()
        {
            tumor.time++;

            tumor.VerhulstEquation();

            if (tumor.cell_list.Count == 100)
                UpdateTumorState();

            CooptionVessels();

            AnalisisDelEstadoDelTumor();

            if(tumor.tumor_stage == TumosStage.Vascular)
            {
                UpdateMigratoryCells();
            }

            if(tumor.vasc_mecha == VascularizationMechanism.Angiogenesis)
            {
                Angiogenesis();
            }

            ObtainCellsPositionsToDivide();

            SearchCloserStemPosition();

            foreach (Cell cell in random_space)
                UpdateState(cell);

            ClearCloserCellsToVessels();
            UpdateNextStemPositionDict();
        }

        public void UpdateTumorState()
        {
            if (tumor.vasc_mecha == VascularizationMechanism.InitialGrowth)
            {
                tumor.vasc_mecha = VascularizationMechanism.VascularCooption;
            }
            else if (tumor.vasc_mecha == VascularizationMechanism.VascularCooption)
            {
                tumor.vasc_mecha = VascularizationMechanism.Angiogenesis;
                tumor.tumor_stage = TumosStage.Vascular;
            }
            else if (tumor.vasc_mecha == VascularizationMechanism.Angiogenesis)
            {
                tumor.vasc_mecha = VascularizationMechanism.Transdifferentiation;
            }
        }

        public void UpdateNextStemPositionDict()
        {
            Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            foreach (Pos pos in next_stem_position.Keys)
            {
                if (!temp.ContainsKey(next_stem_position[pos]))
                    temp.Add(next_stem_position[pos], Utils.NullPos());
            }
            next_stem_position = temp;
        }

        public void UpdateState(Cell cell)
        {
            int tumoral_cells_count = cell.TumoralCellCount();
            if (cell.behavior_state == CellState.Astrocyte || cell.behavior_state == CellState.Neuron)
            {
                double prob = move_prob.ContamineProbability(cell, cell.neighborhood, tumor);
                if (/*Utils.rdm.NextDouble()*/ 0.09 < prob)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                }
                //si no se cumple eso entonces sigue con el mismo estado
            }
            else if (cell.behavior_state == CellState.TumoralCell)
            {
                if(posible_migratory_cells.Contains(cell))
                {
                    cell.behavior_state = CellState.Migratory;
                    next_migratory_position.Add(cell.pos, Utils.NullPos());
                    migratory_cells.Add(cell);
                }
                //tener en cuenta aqui tambien lo de la migracion de las celulas tumorales

                //de cierta manera aqui irian las celulas que se convierte en celulas necroticas y divi
                //if (tumoral_cells_count == 26)
                //    throw new Exception();
            }
            else if(cell.behavior_state == CellState.Migratory)
            {
                if(next_migratory_position.ContainsKey(cell.pos) && next_migratory_position[cell.pos].NullPos())
                {
                    pos_cell_dict.Remove(cell.pos);
                    tumor.cell_list.Remove(cell);
                    cell.behavior_state = CellState.nothing;
                }
            }
            else if (cell.behavior_state == CellState.StemCell)
            {
                if (next_stem_position.ContainsKey(cell.pos))
                {
                    pos_cell_dict.Remove(cell.pos);
                    cell.behavior_state = CellState.nothing;

                }
                else if (tumoral_cells_count >= 1 && Utils.rdm.Next(0, 2) == 1)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                    //pos_cell_dict.Add(cell.pos, cell);
                }
                else if (CloserToTumoralCell(cell) && Utils.rdm.Next(0, 2) == 1)
                {
                    //aqui es donde se supone que hay que analizar las celulas que tiene alrededor de un radio de 5 celdas
                    cell.behavior_state = CellState.TumoralCell;
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                    //pos_cell_dict.Add(cell.pos, cell);
                }
            }
            else if (cell.behavior_state == CellState.nothing && cell.loca_state == CellLocationState.MatrixExtracelular)
            {
                double prob = move_prob.DivisionProbability(cell, cell.neighborhood, tumor);
                if (next_stem_position.ContainsValue(cell.pos))
                {
                    cell.behavior_state = CellState.StemCell;
                    pos_cell_dict.Add(cell.pos, cell);
                }
                else if (closer_cells_to_vessels.Contains(cell) && Utils.rdm.NextDouble() < prob)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    pos_cell_dict.Add(cell.pos, cell);
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                }
                else if (Utils.rdm.NextDouble() < prob)
                {
                    cell.behavior_state = CellState.TumoralCell;
                    pos_cell_dict.Add(cell.pos, cell);
                    tumor.cell_list.Add(cell);

                    tumor.UpdateNewCellCount();
                }
                else if(next_migratory_position.ContainsValue(cell.pos))
                {
                    cell.behavior_state = CellState.Migratory;
                    migratory_cells.Add(cell);
                    pos_cell_dict.Add(cell.pos, cell);
                }
                else if(new_artery_list.Contains(cell))
                {
                    pos_cell_dict.Remove(cell.pos);
                    cell = new Cell(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                    pos_artery_dict.Add(cell.pos, (Artery)cell);
                    
                    space[cell.pos.X, cell.pos.Y, cell.pos.Z] = cell;
                }
                //else if(new_arteriole_list.Contains(cell))
                //{
                //    pos_cell_dict.Remove(cell.pos);
                //    cell = new Arteriole(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);

                //    pos_arteriole_dict.Add(cell.pos, (Arteriole)cell);
                //    space[cell.pos.X, cell.pos.Y, cell.pos.Z] = cell;
                //}
                //else if(new_capillary_list.Contains(cell))
                //{
                //    pos_cell_dict.Remove(cell.pos);
                //    cell = new Capillary(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //    pos_capillary_dict.Add(cell.pos, (Capillary)cell);
                    
                //    space[cell.pos.X, cell.pos.Y, cell.pos.Z] = cell;
                //}
            }
        }
        #endregion

        
    }
}
