using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace AutomataCelularLogic
{
    public class EnvironmentLogic
    {
        private static System.Timers.Timer aTimer;

        //VARIABLES DEL MODELO DEL AUTOMATA
        //public static int cell_proliferation;
        public static int avascular_carrying_capacity = 6920;
        public static int vascular_carrying_capacity = 50000;
        public static double growth_rate = 2.95 * Math.Pow(10, -2);
        public static int initial_population = 5;

        //VARIABLES QUE TIENE QUE VER CON EL ENTORNO
        public static int time = 0;
        public static int distance = 40;
        public static int limit_of_x = 100;
        public static int limit_of_y = 100;
        public static int limit_of_z = 100;

        //VARIABLES QUE TIENE QUE VER CON LAS CELULAS
        public static List<Cell> tumor_cell_list = new List<Cell>(); //CAMBIARLE EL NOMBRE A LA LISTA
        public static List<Cell> stem_cell_list = new List<Cell>();
        public static List<Cell> astrocyte_cell_list = new List<Cell>();
        public static List<Cell> endothelial_cell_list = new List<Cell>();
        public static List<Cell> neuron_cell_list = new List<Cell>();
        //public static List<Cell> cell_list = new List<Cell>();

        //VARIABLES RELACIONADAS CON LOS VASOS SANGUINEOS
        public static List<Cell> artery_list = new List<Cell>();



        public static Dictionary<Pos, Cell> pos_artery_dict = new Dictionary<Pos, Cell>();

        //LISTA RELACIONADAS CON LAS ACCIONES DE LAS CELULAS EN CADA INSTANTE DE TIEMPO
        public static Dictionary<Pos, Pos> contaminate_dict = new Dictionary<Pos, Pos>();
        public static Dictionary<Pos, Pos> division_dict = new Dictionary<Pos, Pos>();

        //VARIBALES QUE TIENEN QUE VER CON EL TUMOR
        public static int tumoral_cell_radio = 5;
        public static Cell tumor_stem_cell = null;



        public static CellularAutomaton ca;

        //VARIABLES QUE TIENEN QUE VER CON LAS ESFERAS TUMORALES
        public static List<Sphere> sphere_list = new List<Sphere>();
        public static Dictionary<Cell, Sphere> sphere_cell_dict = new Dictionary<Cell, Sphere>();
        //esta lista debe tener las celulas que no estan en ninguna esfera
        public static List<Cell> cells_without_sphere = new List<Cell>();
        public static List<Cell> cells_center_of_the_sphere_list = new List<Cell>();

        //public static Timer aTimer;

        //public static int stem_cells_count = rdm.Next(0, 15);


        public static int astrocytes_count = 20;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 20;

        static void Main(string[] args)
        {

            #region comentarios
            //Cell[,,] example = new Cell[10, 10, 10];
            //Pos pos = Utils.GetRandomPosition(0, 10, 0, 10, 0, 10);
            //example[pos.X, pos.Y, pos.Z] = new Cell(pos, CellState.TumoralStemCell, CellLocationState.MatrixExtracelular);

            //double[,,] oxygen = new double[10, 10, 10];
            //oxygen[pos.X -1, pos.Y, pos.Z] = 1.0;
            //oxygen[pos.X +1, pos.Y, pos.Z] = 1.0;
            //oxygen[pos.X, pos.Y -1, pos.Z] = 1.0;
            //oxygen[pos.X, pos.Y +1, pos.Z] = 1.0;
            //oxygen[pos.X, pos.Y, pos.Z -1] = 1.0;
            //oxygen[pos.X, pos.Y, pos.Z +1] = 1.0;
            //oxygen[pos.X, pos.Y, pos.Z] = 1.0;

            //int i = pos.X;
            //int j = pos.Y;
            //int k = pos.Z;

            //double delta = (oxygen[i - 1, j, k] - 2 * oxygen[i, j, k] + oxygen[i + 1, j, k]) / 1 +
            //               (oxygen[i, j - 1, k] - 2 * oxygen[i, j, k] + oxygen[i, j + 1, k]) / 1 +
            //               (oxygen[i, j, k - 1] - 2 * oxygen[i, j, k] + oxygen[i, j, k + 1]) / 1;

            //Console.WriteLine(delta);

            //double value = oxygen[pos.X, pos.Y, pos.Z];
            //oxygen[pos.X, pos.Y, pos.Z] += 1.0 * delta * value - 1.0 * value * value;




            //double u = 3.34 * Math.Pow(10, -3) * 1.0;
            //Console.WriteLine(u);
            //double s = (2 * Math.PI * 0 * 1 * Math.Pow(10, -3) * (0 - 1.0)) / 1.0;
            //Console.WriteLine(s);

            //oxygen[i, j, k] += 2 * ((1.67 * Math.Pow(10, -7) / 1.0 * (-16) - u + s));

            //Console.WriteLine(oxygen[i, j, k]);
            //Console.ReadLine();
            #endregion

            //double diffussion_coeficient_oxygen = 1.8 * Math.Pow(10, -5);
            //double r_c = 4.5 * Math.Pow(10, -17);

            //double dx = 1.0;
            //double dy = 1.0;
            //double dz = 1.0;

            // Definir el tamaño del tablero
            //int tablero_tamanno = 40;

            //// Definir la tasa media de células neuronales por celda
            ////double tasa_media = 1.0;

            //// Definir la densidad de células
            //double densidad = 0.5; // 0.5 para un tablero denso, 0.1 para un tablero con pocas células

            //// Calcular el número de células
            //int numero_celulas = (int)(tablero_tamanno * tablero_tamanno * densidad);

            //Console.WriteLine($"El número de células en el tablero es: {numero_celulas}");

            // Crear una matriz de tamaño tablero_tamanno x tablero_tamanno
            //int[,,] tablero = new int[tablero_tamanno, tablero_tamanno, tablero_tamanno];

            //// Generar el número de células neuronales iniciales para cada celda en el tablero
            //Random random = new Random();
            //for (int i = 0; i < tablero_tamanno; i++)
            //{
            //    for (int j = 0; j < tablero_tamanno; j++)
            //    {
            //        for (int k = 0; k < tablero_tamanno; k++)
            //        {
            //            // Usar la distribución de Poisson para generar el número de células neuronales iniciales
            //            tablero[i, j, k] = GenerarPoisson(tasa_media, random);
            //        }

            //    }
            //}

            //// Imprimir el tablero
            //for (int i = 0; i < tablero_tamanno; i++)
            //{
            //    for (int j = 0; j < tablero_tamanno; j++)
            //    {
            //        for (int k = 0; k < tablero_tamanno; k++)
            //        {
            //            Console.WriteLine($"Celda ({i}, {j}, {k}): {tablero[i, j, k]} células neuronales");
            //        }

            //    }
            //}

            NetworkHistory.Save();

            //Cell[,,] world = new Cell[limit_of_x, limit_of_y, limit_of_z];
            //Pos pos1 = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
            //Pos pos2 = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);

            //world[pos1.X, pos1.Y, pos1.Z] = new Cell(pos1, CellState.Astrocyte, CellLocationState.MatrixExtracelular);
            //world[pos2.X, pos2.Y, pos2.Z] = new Cell(pos2, CellState.Astrocyte, CellLocationState.MatrixExtracelular);

            //Console.WriteLine("{0} {1} {2}", pos1.X, pos1.Y, pos1.Z);
            //Console.WriteLine("{0} {1} {2}", pos2.X, pos2.Y, pos2.Z);

            //Pos medio = new Pos((pos1.X + pos2.X) / 2, (pos1.Y + pos2.Y) / 2, (pos1.Z + pos2.Z) / 2);

            //int count = 0;
            //while(count++ <50)
            //{
            //    Pos newpos;
            //    do
            //    {
            //        newpos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
            //    } 
            //    while ((newpos.X < medio.X && newpos.X > medio.X) || (newpos.Y < medio.Y && newpos.Y > medio.Y));

            //    Console.WriteLine("{0} {1} {2}", newpos.X, newpos.Y, newpos.Z);
            //    world[newpos.X, newpos.Y, newpos.Z] = new Cell(newpos, CellState.Neuron, CellLocationState.MatrixExtracelular);
            //}


            //Simulation();

            Console.ReadLine();
        }



        static double VerhulstGrowth(double actual_population, double growth_rate, double loading_capacity, double time)
        {
            double dP = growth_rate * actual_population * (1 - actual_population / loading_capacity) * time;
            return actual_population + dP;
        }
        public static void PrintMatrix(double[,,] matrix, Cell[,,] space)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix.GetLength(2); k++)
                    {
                        if(space[i,j,k].behavior_state == CellState.ProliferativeTumoralCell)
                            Console.WriteLine("Posicion: {0} {1} {2}, Concentracion: {3}", i, j, k, matrix[i,j,k]);
                    }
                }
            }
        }

        public static void PrintCells(Cell[,,] matrix, double[,,] oxygen, double[,,] densidad, double[,,] mde, double[,,] tnf)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < matrix.GetLength(2); k++)
                    {
                        CellState cs = matrix[i, j, k].behavior_state;
                        if(cs == CellState.TumoralStemCell || cs == CellState.ProliferativeTumoralCell || cs == CellState.MigratoryTumorCell || cs == CellState.QuiescentTumorCell || cs == CellState.NecroticTumorCell)
                            Console.WriteLine("Posicion: {0} {1} {2}, Tipo: {3}, Oxygen: {4}, Densidad: {5}, MDE: {6} TNF: {7}", i, j, k,
                            cs, oxygen[i, j, k], densidad[i, j, k], mde[i, j, k], tnf[i, j, k]);
                        else
                        Console.WriteLine("Posicion: {0} {1} {2}, Tipo: {3}, Oxygen: {4}, Densidad: {5}, MDE: {6} TNF: {7}", i, j, k,
                            cs, oxygen[i,j,k], densidad[i,j,k], mde[i,j,k], tnf[i,j,k]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void Simulation()
        {
            Utils.InitializeVariables();
            ca = new CellularAutomaton(limit_of_x, limit_of_y, limit_of_z, new ClassicProbability(), avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
            //PrintCells(ca.space, ca.model.oxygen_matrix, ca.model.density_matrix, ca.model.mde, ca.model.angiogenic_reg_conc_matrix);

            //Console.ReadLine();
            int count = 0;
            while (count++ < 500)
            {
                //    //Console.WriteLine("Estoy aqui");
                //    //Console.ReadLine();
                //    //    //    //foreach (var key_value in ca.next_stem_position)
                //    //    //    //{
                //    //    //    //    Pos pos = key_value.Key;
                //    //    //    //    if (key_value.Key != null)
                //    //    //    //    {
                //    //    //    //        Console.WriteLine("Nuevo");
                //    //    //    //        Console.WriteLine(pos.X);
                //    //    //    //        Console.WriteLine(pos.Y);
                //    //    //    //        Console.WriteLine(pos.Z);
                //    //    //    //        Console.WriteLine();
                //    //    //    //        if (key_value.Value != null)
                //    //    //    //        {
                //    //    //    //            Console.WriteLine(key_value.Value.X);
                //    //    //    //            Console.WriteLine(key_value.Value.Y);
                //    //    //    //            Console.WriteLine(key_value.Value.Z);
                //    //    //    //        }
                //    //    //    //    }
                //    //    //    //}

                ca.Update();
                //PrintMatrix(ca.model.oxygen_matrix, ca.space);

                int total_count = 0;

                int prolif = 0;
                int inactiva = 0;
                int necrotica = 0;
                int migratoria = 0;

                foreach (var item in ca.tumor.cell_list)
                {
                    CellState behavior = item.Value.behavior_state;
                    if (behavior == CellState.NecroticTumorCell)
                        necrotica++;
                    else if (behavior == CellState.MigratoryTumorCell)
                        migratoria++;
                    else if (behavior == CellState.ProliferativeTumoralCell)
                        prolif++;
                    else if (behavior == CellState.QuiescentTumorCell)
                        inactiva++;
                }
                total_count = ca.tumor.cell_list.Count;

                Console.WriteLine("Tumorales: {0}, Verhuslt: {1} Proliferativas: {2} Inactivas: {3} Migratorias: {4} Necroticas: {5}", total_count, ca.tumor.new_cells_count,
                                    prolif, inactiva, migratoria, necrotica);

                //PrintCells(ca.space, ca.model.oxygen_matrix, ca.model.density_matrix, ca.model.mde, ca.model.angiogenic_reg_conc_matrix);
            }




            //GetCellsThatSenseTheTumorSubstance();

            //PathFromCellsToTumorCell();

            //StemCellConvertToTumoralCell();

            //UpdateActions();

            //ExecuteActions();

            //UpdateTumorState();

        }






        #region Surgimiento


        //public static void MoveAstrocyteToVessels()
        //{
        //    for (int i = 0; i < astrocyte_cell_list.Count; i++)
        //    {
        //        int r = Utils.rdm.Next(0, 2);
        //        if (r == 1)
        //        {
        //            Cell cell = astrocyte_cell_list[i];
        //            Artery artery = null;

        //            do
        //            {
        //                r = Utils.rdm.Next(0, artery_list.Count);
        //                artery = artery_list[r];
        //            } 
        //            while (artery.astrocyte != null);

        //            cell.des_pos = Utils.GetAdjacentPosition(artery.pos1, pos_cell_dict, pos_artery_dict);
        //            pos_cell_dict.Add(cell.des_pos, cell);
        //        }

        //    }
        //}
        //public static void UpdateAstrocytePosition()
        //{
        //    foreach (Cell cell in astrocyte_cell_list)
        //    {
        //        pos_cell_dict.Remove(cell.pos);
        //        cell.pos = cell.des_pos;
        //        cell.loca_status = LocationStatus.GlialBasalLamina;
        //    }
        //}

        ////cambiar el nombre del metodo
        ////HAY QUE ARREGLAR PARA QUE EXISTAN LAS MISMAS CANTIDAD DE POSICIONES QUE DE CELULAS QUE SE VAN ACERCAR
        //public static void PathFromCellsToTumorCell()
        //{
        //    for (int i = 0; i < tumor_cell_list.Count; i++)
        //    {
        //        int x = tumor_stem_cell.pos.X;
        //        int y = tumor_stem_cell.pos.Y;
        //        int z = tumor_stem_cell.pos.Z;

        //        Pos pos = null;

        //        do
        //        {
        //            pos = Utils.GetRandomPosition(x - tumoral_cell_radio, x + tumoral_cell_radio, y - tumoral_cell_radio, y + tumoral_cell_radio, z - tumoral_cell_radio, z + tumoral_cell_radio);
        //        }
        //        while (ExistentPosition(tumor_cell_list, pos, true));

        //        tumor_cell_list[i].des_pos = pos;
        //        pos_cell_dict.Add(pos, tumor_cell_list[i]);

        //    }

        //}

        //public static void StemCellConvertToTumoralCell()
        //{
        //    foreach (Cell cell in tumor_cell_list)
        //    {
        //        pos_cell_dict.Remove(cell.pos);
        //        cell.pos = cell.des_pos;
        //        cell.cell_behavior = new TumorCellBehavior();
        //        tumor.cell_list.Add(cell);

        //        cells_center_of_the_sphere_list.Add(cell);
        //        cells_without_sphere.Add(cell);
        //        //cell_list.Add(new Cell(cell.des_pos, new TumorCellBehavior()));
        //    }
        //}


        //public static bool ExistentPosition(List<Cell> cell_list, Pos pos, bool des_pos)
        //{
        //    if (des_pos)
        //    {
        //        foreach (Cell cell in cell_list)
        //        {
        //            if (cell.des_pos != null && cell.des_pos.X == pos.X && cell.des_pos.Y == pos.Y && cell.des_pos.Z == pos.Z) return true;
        //        }
        //    }
        //    else
        //    {
        //        foreach (Cell cell in cell_list)
        //        {
        //            if (cell.pos != null && cell.pos.X == pos.X && cell.pos.Y == pos.Y && cell.pos.Z == pos.Z) return true;
        //        }
        //    }
        //    return false;
        //}

        //#endregion

        //#region Grow up
        //public static void ExecuteActions()
        //{
        //    foreach (Cell cell in tumor.cell_list)
        //    {
        //        cell.actual_action.Execute();
        //    }
        //}

        //public static void UpdateTumorState()
        //{
        //    foreach (Cell cell in tumor.cell_list)
        //    {
        //        if ((cell.loca_status == LocationStatus.EndothelialBasalLamina || cell.loca_status == LocationStatus.GlialBasalLamina || cell.loca_status == LocationStatus.VascularBasalLamina ||
        //            cell.loca_status == LocationStatus.SmoothVesselCells_time1) && tumor.vasc_mecha == VascularizationMechanism.InitialGrowth)
        //            tumor.vasc_mecha = VascularizationMechanism.VascularCooption;
        //    }
        //}
        //public static void UpdateActions()
        //{
        //    foreach (Cell cell in tumor.cell_list)
        //    {
        //        Tuple<List<Pos>, List<Cell>> pos_cell_tuple = AvailablePositions(cell);
        //        List<Pos> available_positions = pos_cell_tuple.Item1;
        //        List<Cell> available_cell_list = pos_cell_tuple.Item2;

        //        if (available_positions.Count == 0 && available_cell_list.Count == 0)
        //            cell.actual_action = new NothingAction();
        //        else if (available_positions.Count > 0)
        //        {
        //            //ARREGLAR ESTO
        //            float prob = cell.move_prob.MigrateProbability(cell.pos, tumor);

        //            if (prob >= 0.5)
        //                cell.actual_action = new Migrate();
        //            else
        //            {
        //                float div_prob = cell.move_prob.DivisionProbability(cell.pos, tumoral_cell_radio, distance, tumor.new_cells_count);
        //                float cont_prob = cell.move_prob.ContamineProbability(cell.pos, tumor.new_cells_count);
        //                if (div_prob > cont_prob)
        //                {
        //                    //int r = Utils.rdm.Next(0, available_positions.Count);

        //                    cell.actual_action = new Division(tumor, cell, GetPositionCloserToBloodVessels(available_positions));
        //                    tumor.UpdateNewCellCount();
        //                }
        //                else if(cont_prob > 0)
        //                {
        //                    int r = Utils.rdm.Next(0, available_cell_list.Count);
        //                    cell.actual_action = new Contaminate(tumor, cell, available_cell_list[r]);
        //                    tumor.UpdateNewCellCount();
        //                }
        //            }

        //        }
        //        else
        //        {
        //            int r = Utils.rdm.Next(0, available_positions.Count);

        //            //PODEMOS HALLAR LA PROBABILIDAD PARA CONTAMINAR CIERTA CELULA O SIMPLEMENTE LA CONTAMINA Y YA ESTA
        //            //ESTO

        //            //float prob = cell.move_prob.ContaminateProbability(cell.pos, pos_cell_dict[available_positions[r]]);
        //            //if (prob >= 0.5)
        //            //    cell.actual_action = CellActions.Contaminate;

        //            //AGREGAR PARA CUANDO LA PROBABILIDAD DA MENOS DE 0.5. QUE HACER?

        //            //O ESTO
        //            cell.actual_action = new Contaminate(tumor, cell, available_cell_list[r]);
        //            tumor.UpdateNewCellCount();
        //        }
        //    }

        //    //List<Cell> list = tumor.AddNewTumorCells(pos_cell_dict);
        //    //AddPositionsToTheDictionary();
        //    //cells_without_sphere.AddRange(list);
        //    FormationOfSpheres();

        //}

        //public static Pos GetPositionCloserToBloodVessels(List<Pos> positions_list)
        //{
        //    Pos pos = null;
        //    int min_distance = int.MaxValue;
        //    foreach (Pos item in positions_list)
        //    {
        //        foreach (Artery artery in artery_list)
        //        {
        //            int distance = Utils.EuclideanDistance(item, artery.pos1);
        //            if(distance < min_distance)
        //            {
        //                pos = item;
        //                min_distance = distance;
        //            }
        //        }
        //    }
        //    return pos;
        //}
        //public static bool EmptyPositions(List<Pos> pos_list)
        //{
        //    for (int i = 0; i < pos_list.Count; i++)
        //    {
        //        if (!pos_cell_dict.ContainsKey(pos_list[i])) return true;
        //    }
        //    return false;
        //}

        //public static Tuple<List<Pos>, List<Cell>> AvailablePositions(Cell cell)
        //{

        //    List<Pos> available_positions = new List<Pos>();
        //    List<Cell> cell_list = new List<Cell>();
        //    for (int i = 0; i < Utils.mov_3d.Count; i++)
        //    {
        //        int[] array = Utils.mov_3d[i];
        //        Pos pos = new Pos(cell.pos.X + array[0], cell.pos.Y + array[1], cell.pos.Z + array[2]);
        //        if (Utils.ValidPosition(pos))
        //        {
        //            if (!pos_cell_dict.ContainsKey(pos))
        //                available_positions.Add(pos);
        //            else if(pos_cell_dict.ContainsKey(pos) && !(pos_cell_dict[pos].cell_behavior is TumorCellBehavior))
        //                cell_list.Add(pos_cell_dict[pos]);
        //        }
        //    }
        //    return new Tuple<List<Pos>, List<Cell>>(available_positions, cell_list);
        //}

        //public static void AddPositionsToTheDictionary()
        //{
        //    foreach (Cell cell in tumor.cell_list)
        //    {
        //        if (!pos_cell_dict.ContainsKey(cell.pos))
        //            pos_cell_dict.Add(cell.pos, cell);
        //    }
        //}

        //public static void FormationOfSpheres()
        //{
        //    //int radio_1_count = 0;
        //    //int radio_2_count = 0;

        //    List<Cell> radio_1_cells = new List<Cell>();
        //    List<Cell> radio_2_cells = new List<Cell>();

        //    foreach (Cell cell in cells_center_of_the_sphere_list)
        //    {
        //        int radio_1_count = 0;
        //        int radio_2_count = 0;
        //        if (sphere_cell_dict.ContainsKey(cell))
        //        {
        //            int count = 0;
        //            int count2 = 0;
        //            List<Cell> cell_list = new List<Cell>();
        //            List<Cell> cell_list2 = new List<Cell>();
        //            int radio = sphere_cell_dict[cell].radio;

        //            //Sphere sphere = sphere_cell_dict[cell];

        //            foreach (var key_value in pos_cell_dict)
        //            {
        //                if (Utils.EuclideanDistance(key_value.Key, cell.pos) == (radio + 1))
        //                {
        //                    count++;
        //                    cell_list.Add(key_value.Value);
        //                }
        //                else if (Utils.EuclideanDistance(key_value.Key, cell.pos) == (radio + 2))
        //                {
        //                    count2++;
        //                    cell_list2.Add(key_value.Value);
        //                }
        //            }
        //            if (count2 >= 10)
        //            {
        //                cell_list.AddRange(cell_list2);
        //                sphere_cell_dict[cell].radio = radio + 1;
        //                sphere_cell_dict[cell].cell_list.AddRange(cell_list);
        //                //sphere_cell_dict[cell].cell_list.AddRange(cell_list2);
        //                foreach (Cell item in cell_list)
        //                    cells_without_sphere.Remove(item);
        //            }
        //            else if (count >= 10)
        //            {
        //                sphere_cell_dict[cell].radio = radio + 1;
        //                sphere_cell_dict[cell].cell_list.AddRange(cell_list);
        //                foreach (Cell item in cell_list)
        //                    cells_without_sphere.Remove(item);

        //                //Sphere sphere2 = sphere_cell_dict[cell];
        //                //Console.WriteLine("Vamos a ver si dos esferas son iguales aunque cambie el radio");
        //                //Console.WriteLine(sphere==sphere2);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var key_value in pos_cell_dict)
        //            {
        //                if (Utils.EuclideanDistance(key_value.Key, cell.pos) == 1)
        //                {
        //                    radio_1_count++;
        //                    radio_1_cells.Add(key_value.Value);
        //                }
        //                else if (Utils.EuclideanDistance(key_value.Key, cell.pos) == 2)
        //                {
        //                    radio_2_count++;
        //                    radio_2_cells.Add(key_value.Value);
        //                }
        //            }
        //            if (radio_2_count >= 10)
        //            {
        //                radio_1_cells.AddRange(radio_2_cells);
        //                sphere_cell_dict.Add(cell, new Sphere(2, radio_1_cells));
        //                foreach (Cell item in radio_1_cells)
        //                    cells_without_sphere.Remove(item);
        //            }
        //            else if (radio_1_count >= 10)
        //            {
        //                sphere_cell_dict.Add(cell, new Sphere(2, radio_1_cells));
        //                foreach (Cell item in radio_1_cells)
        //                    cells_without_sphere.Remove(item);
        //            }
        //        }
        //    }
        //}

        ////Esto deberia ser un metodo interno de la clase
        ////public static CellActions CellAction(Cell cell)
        ////{
        ////    CellActions action = new CellActions();
        ////    //action = (CellActions)rdm.Next(0, 3);
        ////    action = CellActions.Division;
        ////    return action;
        ////}


        #endregion
    }
    public struct Point3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
    public class PoissonDiskGenerator
    {
        private static Random random = new Random();
        private static double radius = 1.0;
        private static List<Point3D> points = new List<Point3D>();

        public static Point3D GenerateRandomPoint(int height, int width, int depth)
        {
            int x;
            int y;
            int z;
            do
                x = (int)(random.Next(0, height) * radius * 2 - radius);
            while (x < 0);
            do
                y = (int)(random.Next(0, width) * radius * 2 - radius);
            while (y < 0);
            do
                z = (int)(random.Next(0, depth) * radius * 2 - radius);
            while (z < 0);

            return new Point3D { X = x, Y = y, Z = z };
        }

        public static bool IsTooClose(Point3D point, List<Point3D> points)
        {
            foreach (Point3D existingPoint in points)
            {
                double distance = Math.Sqrt(Math.Pow(point.X - existingPoint.X, 2) + Math.Pow(point.Y - existingPoint.Y, 2) + Math.Pow(point.Z - existingPoint.Z, 2));
                if (distance < radius)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Point3D> GeneratePoissonDiskPoints(int width, int height, int depth)
        {
            List<Point3D> poissonDiskPoints = new List<Point3D>();
            for (int x = 0; x < width; x++)
            {
                //for (int y = 0; y < height; y++)
                //{
                //    for (int z = 0; z < depth; z++)
                //    {
                Point3D point = GenerateRandomPoint(height, width, depth);
                while (IsTooClose(point, poissonDiskPoints))
                {
                    point = GenerateRandomPoint(height, width, depth);
                }
                if (poissonDiskPoints.Contains(point))
                    Console.WriteLine("Ya existe");
                poissonDiskPoints.Add(point);
                //    }
                //}
            }
            return poissonDiskPoints;
        }
    }

}
