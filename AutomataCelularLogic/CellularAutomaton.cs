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
        public Dictionary<Pos, Artery> pos_artery_dict;
        public Dictionary<Pos, Arteriole> pos_arteriole_dict;
        public Dictionary<Pos, Capillary> pos_capillary_dict;
        public Dictionary<Artery, Tuple<List<Arteriole>, Capillary>> artery_arteriole_dict;
        public Dictionary<Pos, Pos> next_stem_position;
        public List<Cell> closer_cells_to_vessels;

        public List<Cell> vasos_cooptados;

        public List<Tuple<Cell, Cell>> vasos_en_crecimiento;
        public List<Cell> migratory_cells;

        public CellularAutomaton(int x, int y, int z, Probability move_prob, int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            space = new Cell[x, y, z];
            this.move_prob = move_prob;
            //this.cell_space = cell_space;

            next_stem_position = new Dictionary<Pos, Pos>();
            pos_cell_dict = new Dictionary<Pos, Cell>();
            pos_artery_dict = new Dictionary<Pos, Artery>();
            artery_arteriole_dict = new Dictionary<Artery, Tuple<List<Arteriole>, Capillary>>();
            pos_arteriole_dict = new Dictionary<Pos, Arteriole>();
            pos_capillary_dict = new Dictionary<Pos, Capillary>();
            vasos_en_crecimiento = new List<Tuple<Cell, Cell>>();
            migratory_cells = new List<Cell>();
            random_space = new Cell[x * y * z];

            vasos_cooptados = new List<Cell>();

            closer_cells_to_vessels = new List<Cell>();
            StartCellularLifeInTheBrain();
            CreateTumor(avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);

            UpdateNeighborhood();
            AleatorizarCelulas();
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
                    next_stem_position.Add(cell.pos, null);
            }
        }

        //OJO CAMBIAR NOMBREEEEEE

        #region Creation
        public void StartCellularLifeInTheBrain()
        {
            CreateBloodVessels();

            #region comentarios
            //Console.WriteLine("Arterias");
            //foreach (var item in pos_artery_dict)
            //{
            //    Console.WriteLine("Arteria");
            //    Console.WriteLine("{0} {1} {2}", item.Key.X, item.Key.Y, item.Key.Z);

            //    Console.WriteLine("Arteriolas");
            //    if (artery_arteriole_dict.ContainsKey(item.Value))
            //    {
            //        foreach (var item2 in artery_arteriole_dict[item.Value].Item1)
            //        {
            //            Console.WriteLine("{0} {1} {2}", item2.pos.X, item2.pos.Y, item2.pos.Z);
            //        }

            //        Console.WriteLine("Capillar");
            //        var capillar = artery_arteriole_dict[item.Value].Item2;
            //        Console.WriteLine("{0} {1} {2}", capillar.pos.X, capillar.pos.Y, capillar.pos.Z);
            //    }
            //    Console.WriteLine();
            //}
            #endregion

            StartTumoraCell();

            CreateCells(CellState.StemCell, CellLocationState.MatrixExtracelular, stem_cells_count);
            CreateCells(CellState.Astrocyte, CellLocationState.MatrixExtracelular, astrocytes_count);
            CreateCells(CellState.Neuron, CellLocationState.MatrixExtracelular, neuron_count);

            RellenarHuecos();

        }

        public void StartTumoraCell()
        {
            tumor_stem_cell = new Cell(new Pos(20, 20, 20), CellState.TumoralCell, CellLocationState.MatrixExtracelular);
            //tumor_stem_cell.neighborhood = Utils.GetMooreNeighbours3D(tumor_stem_cell.pos, space);
            //pos_cell_dict.Add(tumor_stem_cell.pos, tumor_stem_cell);

            space[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y, tumor_stem_cell.pos.Z] = tumor_stem_cell;

        }

        public void CreateTumor(int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
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
                while (pos_cell_dict.ContainsKey(new_pos) || pos_artery_dict.ContainsKey(new_pos) || pos_arteriole_dict.ContainsKey(new_pos) || pos_capillary_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

                Cell cell = new Cell(new_pos, cell_state, loca_state);
                //cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                space[new_pos.X, new_pos.Y, new_pos.Z] = cell;

                //pos_cell_dict.Add(new_pos, cell);
            }
        }

        public void CreateBloodVessels()
        {
            CreateArtery();
            AddArterioleToArtery();
        }

        public void CreateArtery()
        {
            Pos pos = new Pos(space.GetLength(0) / 2, space.GetLength(1) / 2, 0);
            Artery artery = new Artery(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
            pos_artery_dict.Add(pos, artery);
            space[pos.X, pos.Y, pos.Z] = artery;

            int x = pos.X;
            int y1 = pos.Y - 1;
            int y2 = pos.Y + 1;
            int z = pos.Z + 1;


            while (Utils.ValidPosition(x, y1, z) && Utils.ValidPosition(x, y2, z))
            {
                if (Utils.ValidPosition(x, y1, z) && !pos_artery_dict.ContainsKey(new Pos(x, y1, z)))
                {
                    pos = new Pos(x, y1, z);
                    artery = new Artery(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                    pos_artery_dict.Add(pos, artery);
                    space[pos.X, pos.Y, pos.Z] = artery;

                    y1--;
                }
                if (Utils.ValidPosition(x, y2, z) && !pos_artery_dict.ContainsKey(new Pos(x, y2, z)))
                {
                    pos = new Pos(x, y2, z);
                    artery = new Artery(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                    pos_artery_dict.Add(pos, artery);
                    space[pos.X, pos.Y, pos.Z] = artery;

                    y2++;
                }
                z++;

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

        public void AddArterioleToArtery()
        {
            foreach (var item in pos_artery_dict.Values)
            { 
                if(Utils.rdm.Next(0,2) == 1)
                {
                    CreateArteriole(item);
                }
            }
        }

        public void CreateArteriole(Artery artery)
        {
            int[,] array_mov = new int[,] { { 0, 1, 1 }, { 0, -1, 1 }, { 0, 0, 1 } };


            if (Utils.rdm.Next(0, 2) == 1)
            {
                Pos pos = null;
                Pos pos1 = null;

                do
                {
                    int i = Utils.rdm.Next(0, array_mov.GetLength(1));

                    pos = new Pos(artery.pos.X, artery.pos.Y + array_mov[i, 1], artery.pos.Z + array_mov[i, 2]);
                    i = Utils.rdm.Next(0, array_mov.GetLength(1));
                    pos1 = new Pos(pos.X, pos.Y + array_mov[i, 1], pos.Z + array_mov[i, 2]);
                }
                while (!Utils.ValidPosition(pos) || !Utils.ValidPosition(pos1) || pos_artery_dict.ContainsKey(pos) || pos_artery_dict.ContainsKey(pos1));

                Arteriole arteriole = new Arteriole(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                Arteriole arteriole1 = new Arteriole(pos1, CellState.nothing, CellLocationState.GlialBasalLamina);

                List<Arteriole> arteriole_list = new List<Arteriole>();

                arteriole_list.Add(arteriole);
                arteriole_list.Add(arteriole1);
                artery_arteriole_dict.Add(artery, new Tuple<List<Arteriole>, Capillary>(arteriole_list, null));

                pos_arteriole_dict.Add(arteriole.pos, arteriole);
                pos_arteriole_dict.Add(arteriole1.pos, arteriole1);

                space[pos.X, pos.Y, pos.Z] = arteriole;
                space[arteriole1.pos.X, arteriole1.pos.Y, arteriole1.pos.Z] = arteriole1;

                CreateCapilar(arteriole1, artery);
            }
            else
            {
                Pos pos = null;
                Pos pos1 = null;
                Pos pos2 = null;

                do
                {
                    int i = Utils.rdm.Next(0, array_mov.GetLength(1));

                    pos = new Pos(artery.pos.X, artery.pos.Y + array_mov[i, 1], artery.pos.Z + array_mov[i, 2]);
                    i = Utils.rdm.Next(0, array_mov.GetLength(1));
                    pos1 = new Pos(pos.X, pos.Y + array_mov[i, 1], pos.Z + array_mov[i, 2]);
                    i = Utils.rdm.Next(0, array_mov.GetLength(1));
                    pos2 = new Pos(pos.X, pos.Y + array_mov[i, 1], pos.Z + array_mov[i, 2]);
                }
                while (!Utils.ValidPosition(pos) || !Utils.ValidPosition(pos1) || !Utils.ValidPosition(pos2) || pos_artery_dict.ContainsKey(pos) || pos_artery_dict.ContainsKey(pos1) || pos_artery_dict.ContainsKey(pos2));

                Arteriole arteriole = new Arteriole(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                Arteriole arteriole1 = new Arteriole(pos1, CellState.nothing, CellLocationState.GlialBasalLamina);
                Arteriole arteriole2 = new Arteriole(pos2, CellState.nothing, CellLocationState.GlialBasalLamina);

                List<Arteriole> arteriole_list = new List<Arteriole>();

                arteriole_list.Add(arteriole);
                arteriole_list.Add(arteriole1);
                arteriole_list.Add(arteriole2);
                artery_arteriole_dict.Add(artery, new Tuple<List<Arteriole>, Capillary>(arteriole_list, null));

                pos_arteriole_dict.Add(arteriole.pos, arteriole);
                pos_arteriole_dict.Add(arteriole1.pos, arteriole1);
                pos_arteriole_dict.Add(arteriole2.pos, arteriole2);

                space[pos.X, pos.Y, pos.Z] = arteriole;
                space[arteriole1.pos.X, arteriole1.pos.Y, arteriole1.pos.Z] = arteriole1;
                space[arteriole2.pos.X, arteriole2.pos.Y, arteriole2.pos.Z] = arteriole2;

                CreateCapilar(arteriole2, artery);
            }
        }

        public void CreateCapilar(Arteriole arteriole, Artery artery)
        {
            int[,] array_mov = new int[,] { { 0, 1, 1 }, { 0, -1, 1 }, { 0, 0, 1 } };

            Pos pos = null;
            do
            {
                int i = Utils.rdm.Next(0, array_mov.GetLength(1));
                pos = new Pos(arteriole.pos.X + array_mov[i, 0], arteriole.pos.Y + array_mov[i, 1], arteriole.pos.Z + array_mov[i, 2]);
            }
            while (!Utils.ValidPosition(pos) || pos_arteriole_dict.ContainsKey(pos) || pos_artery_dict.ContainsKey(pos) || pos_capillary_dict.ContainsKey(pos));

            Capillary capillary = new Capillary(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
            artery_arteriole_dict[artery] = new Tuple<List<Arteriole>, Capillary>(artery_arteriole_dict[artery].Item1, capillary);
            pos_capillary_dict.Add(pos, capillary);
        }

        public void RellenarHuecos()
        {
            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        if (space[i, j, k] == null)
                        {
                            space[i, j, k] = new Cell(new Pos(i, j, k), CellState.nothing, CellLocationState.MatrixExtracelular);
                        }
                    }
                }
            }
        }
        #endregion


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
                    foreach (var item2 in pos_capillary_dict)
                    {
                        if (!vasos_cooptados.Contains(item2.Value))
                        {
                            int distance = Utils.EuclideanDistance(item.pos, item2.Key);
                            if (distance < distance_min)
                            {
                                distance_min = distance;
                                artery = pos_capillary_dict[item2.Key];
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
        public void GetMigratoryCells()
        {
            List<Cell> possible_migratory_cells = new List<Cell>();

            foreach (var item in tumor.cell_list)
            {
                if (GetFronterCells(item))
                    possible_migratory_cells.Add(item);
            }

            //List<Cell> migratory_cells = new List<Cell>();
            foreach (var item in possible_migratory_cells)
            {
                if (Utils.rdm.NextDouble() < move_prob.MigrateProbability(item.pos, tumor))
                {
                    migratory_cells.Add(item);
                    item.behavior_state = CellState.Migratory;
                }
            }
        }

        public bool GetFronterCells(Cell cell)
        {
            int count = 0;
            foreach (var item in cell.neighborhood)
            {
                if (item.behavior_state == CellState.nothing && item.loca_state == CellLocationState.MatrixExtracelular)
                    count++;
            }
            return count >= 5;
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
            foreach (var item in pos_capillary_dict.Values)
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

        //Aqui lo que estoy haciendo es buscando todas las casillas vacias alrededor del tumor para hacer crecer un nuevo vaso hasta esa casilla;
        public List<Tuple<Cell, Cell>> NewVesselsFormation()
        {
            int min = int.MaxValue;
            Cell blood_vessel = null;
            List<Cell> cells = BuscarPosicionesAlBordeDelTumor();

            List<Cell> blood_vessels = new List<Cell>();
            blood_vessels.AddRange(pos_artery_dict.Values);
            blood_vessels.AddRange(pos_arteriole_dict.Values);
            blood_vessels.AddRange(pos_capillary_dict.Values);

            List<Tuple<Cell, Cell>> tumorCell_bloodVessel = new List<Tuple<Cell, Cell>>();

            foreach (var item in cells)
            {
                foreach (var item2 in blood_vessels)
                {
                    int distance = Utils.EuclideanDistance(item.pos, item2.pos);
                    if (distance < min)
                    {
                        min = distance;
                        blood_vessel = item2;
                    }

                }
                tumorCell_bloodVessel.Add(new Tuple<Cell, Cell>(item, blood_vessel));
            }
            return tumorCell_bloodVessel;


        }

        public void CrearCaminoParaAlcanzarAlTumor()
        {
            List<Tuple<Cell, Cell>> tumorCells_bloodVessels = NewVesselsFormation();

            int i = Utils.rdm.Next(0, tumorCells_bloodVessels.Count);

            Tuple<Cell, Cell> tumorCell_bloodVessel = tumorCells_bloodVessels[i];
            Cell vecino_mas_cercano = VecinoMasCercano(tumorCell_bloodVessel);

            if (tumorCell_bloodVessel.Item2 is Arteriole)
            {
                Arteriole arteriole = new Arteriole(vecino_mas_cercano.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                pos_arteriole_dict.Add(vecino_mas_cercano.pos, arteriole);
                space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;
                vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(tumorCell_bloodVessel.Item1, arteriole));
                //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
            }
        }

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

        public void ContinuarCrecimiento()
        {
            foreach (var item in vasos_en_crecimiento)
            {
                var vecino = VecinoMasCercano(item);
                if (item.Item2 is Arteriole)
                {
                    Arteriole arteriole = new Arteriole(vecino.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                    pos_arteriole_dict.Add(vecino.pos, arteriole);
                    space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;
                    vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(item.Item1, arteriole));
                    //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
                }
            }
            
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

            //UpdateTumorState();

            //Console.WriteLine("Lista de celulas tumorales");
            //foreach (var item in tumor.cell_list)
            //{
            //    Console.WriteLine("Nueva celula tumoral");
            //    Console.WriteLine("{0} {1} {2}", item.pos.X, item.pos.Y, item.pos.Z);
            //    Console.WriteLine(item.behavior_state);
            //    Console.WriteLine(item.loca_state);
            //}
            //Console.WriteLine("--------------------------------");

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
                    temp.Add(next_stem_position[pos], null);
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
                //tener en cuenta aqui tambien lo de la migracion de las celulas tumorales

                //de cierta manera aqui irian las celulas que se convierte en celulas necroticas y divi
                //if (tumoral_cells_count == 26)
                //    throw new Exception();
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
            }
        }
        #endregion

        
    }
}
