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
        public static int avascular_carrying_capacity = 500;
        public static int vascular_carrying_capacity = 1000;
        public static double growth_rate = 1.2 * Math.Pow(10, -2);
        public static int initial_population = 1;

        //VARIABLES QUE TIENE QUE VER CON EL ENTORNO
        public static int time = 0;
        public static int distance = 40;
        public static int limit_of_x = 40;
        public static int limit_of_y = 40;
        public static int limit_of_z = 40;

        //VARIABLES QUE TIENE QUE VER CON LAS CELULAS
        public static List<Cell> tumor_cell_list = new List<Cell>(); //CAMBIARLE EL NOMBRE A LA LISTA
        public static List<Cell> stem_cell_list = new List<Cell>();
        public static List<Cell> astrocyte_cell_list = new List<Cell>();
        public static List<Cell> endothelial_cell_list = new List<Cell>();
        //public static List<Cell> cell_list = new List<Cell>();

        //VARIABLES RELACIONADAS CON LOS VASOS SANGUINEOS
        public static List<Artery> artery_list = new List<Artery>();
 
        public static Dictionary<Pos, Cell> pos_cell_dict = new Dictionary<Pos, Cell>();

        //LISTA RELACIONADAS CON LAS ACCIONES DE LAS CELULAS EN CADA INSTANTE DE TIEMPO
        public static Dictionary<Pos, Pos> contaminate_dict = new Dictionary<Pos, Pos>();
        public static Dictionary<Pos, Pos> division_dict = new Dictionary<Pos, Pos>();

        //VARIBALES QUE TIENEN QUE VER CON EL TUMOR
        public static int tumoral_cell_radio = 5;
        public static Cell tumor_stem_cell = null;

        public static Tumor tumor;
       
        //VARIABLES QUE TIENEN QUE VER CON LAS ESFERAS TUMORALES
        public static List<Sphere> sphere_list = new List<Sphere>();
        public static Dictionary<Cell, Sphere> sphere_cell_dict = new Dictionary<Cell, Sphere>();
        //esta lista debe tener las celulas que no estan en ninguna esfera
        public static List<Cell> cells_without_sphere = new List<Cell>();
        public static List<Cell> cells_center_of_the_sphere_list = new List<Cell>();

        //public static Timer aTimer;

        //public static int stem_cells_count = rdm.Next(0, 15);

        public static int stem_cells_count = 15;
        public static int astrocytes_count = 20;
        public static int blood_vessels_count = 10;

        static void Main(string[] args)
        {
            //console.writeline("hello world");

            //Console.WriteLine("Hello World");

            Simulation();

            Console.WriteLine("Esferas");
            foreach (var item in sphere_cell_dict)
            {
                Console.WriteLine(item.Value.radio);
            }
            Console.ReadLine();

        }

        public static void Simulation()
        {
            //InitializeVariables();

            StartCellularLifeInTheBrain();

            GetCellsThatSenseTheTumorSubstance();

            PathFromCellsToTumorCell();

            StemCellConvertToTumoralCell();

            int time = 0;
            while (time != 10)
            {
                time++;
                UpdateActions();
                //Console.WriteLine("Lista");
                Console.WriteLine(cells_without_sphere.Count);

            }
            Console.WriteLine("Termine");
            
        }


        public static void StartCellularLifeInTheBrain()
        {
            //Crear la primera celula tumoral y cambiar la posicion del tumor
            StartTumoraCell();

            CreateTumor();

            //Crear las celulas madres pluripotencial
            CreateStemCells();

            CreateBloodVessels();

            UpdateActions();

            ExecuteActions();

            //HACER!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Crear las celulas astrocitos, pericitos, etc
        }

        public static void StartTumoraCell()
        {
            tumor_stem_cell = new Cell(new Pos(15, 15, 15), new TumorCellBehavior(), new ClassicProbability());
        }

        public static void CreateTumor()
        {
            tumor = new Tumor(tumor_stem_cell, tumoral_cell_radio, avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
        }

        public static void CreateStemCells()
        {
            for (int i = 0; i < stem_cells_count; i++)
            {
                Pos new_pos;
                do
                {
                    new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
                }
                while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

                stem_cell_list.Add(new Cell(new_pos, new StemCellBehavior(), new ClassicProbability()));
                pos_cell_dict.Add(new_pos, stem_cell_list[stem_cell_list.Count - 1]);
            }
        }
        public static Cell CreateAstrocyteCell()
        {
            //for (int i = 0; i < astrocytes_count; i++)
            //{

            Pos new_pos;
            do
            {
                new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
            }
            while (pos_cell_dict.ContainsKey(new_pos)); //ARREGLAR ESTO

            Cell cell = new Cell(new_pos, new AstrocyteCellBehavior(), new ClassicProbability());
            astrocyte_cell_list.Add(cell);
            pos_cell_dict.Add(new_pos, cell);
            return cell;
        }

        public static Cell CreateEndothelialCell()
        {
            Pos new_pos;
            do
            {
                new_pos = Utils.GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
            }
            while (pos_cell_dict.ContainsKey(new_pos));

            Cell cell = new Cell(new_pos, new EndothelialCellBehavior(), new ClassicProbability());
            endothelial_cell_list.Add(cell);
            pos_cell_dict.Add(new_pos, cell);
            return cell;
        }

        
        public static void CreateBloodVessels()
        {
            for (int i = 0; i < blood_vessels_count; i++)
            {
                int r = Utils.rdm.Next(0, 2);
                Cell astrocyte;
                if (r == 1)
                {
                    astrocyte = CreateAstrocyteCell();

                    Pos pos = Utils.GetAdjacentPosition(astrocyte.pos, pos_cell_dict);
                    if (pos != null)
                    {
                        Cell endothelial_cell = new Cell(pos, new EndothelialCellBehavior(), new ClassicProbability());
                        endothelial_cell_list.Add(endothelial_cell);
                        pos_cell_dict.Add(pos, astrocyte);

                        artery_list.Add(new Artery(endothelial_cell.pos, astrocyte, endothelial_cell));
                    }
                    //VAMOS ASUMIR POR AHORA QUE QUE LA VARIBALE ASTROCITO ES DISTINTO DE NULL Y QUE EXISTE UNA POSICION ADYACENTE
                }
                else
                {
                    Cell endothelial_cell = CreateEndothelialCell();
                    artery_list.Add(new Artery(endothelial_cell.pos, null, endothelial_cell));
                }
            }
            
        }

        #region Surgimiento

        public static void GetCellsThatSenseTheTumorSubstance()
        {
            for (int i = 0; i < stem_cell_list.Count; i++)
            {
                Cell cell = stem_cell_list[i];
                if (Utils.EuclideanDistance(tumor_stem_cell.pos, cell.pos) <= distance)
                {
                    tumor_cell_list.Add(cell);
                }
            }
        }

        //cambiar el nombre del metodo
        //HAY QUE ARREGLAR PARA QUE EXISTAN LAS MISMAS CANTIDAD DE POSICIONES QUE DE CELULAS QUE SE VAN ACERCAR
        public static void PathFromCellsToTumorCell()
        {
            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                int x = tumor_stem_cell.pos.X;
                int y = tumor_stem_cell.pos.Y;
                int z = tumor_stem_cell.pos.Z;

                Pos pos = null;

                do
                {
                    pos = Utils.GetRandomPosition(x - tumoral_cell_radio, x + tumoral_cell_radio, y - tumoral_cell_radio, y + tumoral_cell_radio, z - tumoral_cell_radio, z + tumoral_cell_radio);
                }
                while (ExistentPosition(tumor_cell_list, pos, true));

                tumor_cell_list[i].des_pos = pos;
                pos_cell_dict.Add(pos, tumor_cell_list[i]);

            }

        }

        public static void StemCellConvertToTumoralCell()
        {
            foreach (Cell cell in tumor_cell_list)
            {
                pos_cell_dict.Remove(cell.pos);
                cell.pos = cell.des_pos;
                cell.cell_behavior = new TumorCellBehavior();
                tumor.cell_list.Add(cell);

                cells_center_of_the_sphere_list.Add(cell);
                cells_without_sphere.Add(cell);
                //cell_list.Add(new Cell(cell.des_pos, new TumorCellBehavior()));
            }
        }


        public static bool ExistentPosition(List<Cell> cell_list, Pos pos, bool des_pos)
        {
            if (des_pos)
            {
                foreach (Cell cell in cell_list)
                {
                    if (cell.des_pos != null && cell.des_pos.X == pos.X && cell.des_pos.Y == pos.Y && cell.des_pos.Z == pos.Z) return true;
                }
            }
            else
            {
                foreach (Cell cell in cell_list)
                {
                    if (cell.pos != null && cell.pos.X == pos.X && cell.pos.Y == pos.Y && cell.pos.Z == pos.Z) return true;
                }
            }
            return false;
        }

        #endregion

        #region Grow up
        public static void ExecuteActions()
        {
            foreach (Cell cell in tumor.cell_list)
            {
                cell.actual_action.Execute();
            }
        }

        public static void UpdateActions()
        {
            foreach (Cell cell in tumor.cell_list)
            {
                Tuple<List<Pos>, List<Cell>> pos_cell_tuple = AvailablePositions(cell);
                List<Pos> available_positions = pos_cell_tuple.Item1;
                List<Cell> available_cell_list = pos_cell_tuple.Item2;

                if (available_positions.Count == 0 && available_cell_list.Count == 0)
                    cell.actual_action = new NothingAction();
                else if (available_positions.Count > 0)
                {
                    //ARREGLAR ESTO
                    float prob = cell.move_prob.MigrateProbability(cell.pos, tumor);

                    if (prob >= 0.5)
                        cell.actual_action = new Migrate();
                    else
                    {
                        if (cell.move_prob.DivisionProbability(cell.pos, pos_cell_dict, tumoral_cell_radio, distance) > cell.move_prob.ContamineProbability(cell.pos))
                        {
                            int r = Utils.rdm.Next(0, available_positions.Count);

                            cell.actual_action = new Division(tumor, cell, available_positions[r]);
                        }
                        else
                        {
                            int r = Utils.rdm.Next(0, available_cell_list.Count);
                            cell.actual_action = new Contaminate(tumor, cell, available_cell_list[r]);
                        }
                    }

                }
                else
                {
                    int r = Utils.rdm.Next(0, available_positions.Count);

                    //PODEMOS HALLAR LA PROBABILIDAD PARA CONTAMINAR CIERTA CELULA O SIMPLEMENTE LA CONTAMINA Y YA ESTA
                    //ESTO

                    //float prob = cell.move_prob.ContaminateProbability(cell.pos, pos_cell_dict[available_positions[r]]);
                    //if (prob >= 0.5)
                    //    cell.actual_action = CellActions.Contaminate;

                    //AGREGAR PARA CUANDO LA PROBABILIDAD DA MENOS DE 0.5. QUE HACER?

                    //O ESTO
                    cell.actual_action = new Contaminate(tumor, cell, available_cell_list[r]);
                }
            }
                
            //List<Cell> list = tumor.AddNewTumorCells(pos_cell_dict);
            //AddPositionsToTheDictionary();
            //cells_without_sphere.AddRange(list);
            FormationOfSpheres();

        }
        public static bool EmptyPositions(List<Pos> pos_list)
        {
            for (int i = 0; i < pos_list.Count; i++)
            {
                if (!pos_cell_dict.ContainsKey(pos_list[i])) return true;
            }
            return false;
        }

        public static Tuple<List<Pos>, List<Cell>> AvailablePositions(Cell cell)
        {

            List<Pos> available_positions = new List<Pos>();
            List<Cell> cell_list = new List<Cell>();
            for (int i = 0; i < Utils.mov_3d.Count; i++)
            {
                int[] array = Utils.mov_3d[i];
                Pos pos = new Pos(cell.pos.X + array[0], cell.pos.Y + array[1], cell.pos.Z + array[2]);
                if (Utils.ValidPosition(pos))
                {
                    if (!pos_cell_dict.ContainsKey(pos))
                        available_positions.Add(pos);
                    else if(pos_cell_dict.ContainsKey(pos) && !(pos_cell_dict[pos].cell_behavior is TumorCellBehavior))
                        cell_list.Add(pos_cell_dict[pos]);
                }
            }
            return new Tuple<List<Pos>, List<Cell>>(available_positions, cell_list);
        }

        public static void AddPositionsToTheDictionary()
        {
            foreach (Cell cell in tumor.cell_list)
            {
                if (!pos_cell_dict.ContainsKey(cell.pos))
                    pos_cell_dict.Add(cell.pos, cell);
            }
        }

        public static void FormationOfSpheres()
        {
            //int radio_1_count = 0;
            //int radio_2_count = 0;

            List<Cell> radio_1_cells = new List<Cell>();
            List<Cell> radio_2_cells = new List<Cell>();

            foreach (Cell cell in cells_center_of_the_sphere_list)
            {
                int radio_1_count = 0;
                int radio_2_count = 0;
                if (sphere_cell_dict.ContainsKey(cell))
                {
                    int count = 0;
                    int count2 = 0;
                    List<Cell> cell_list = new List<Cell>();
                    List<Cell> cell_list2 = new List<Cell>();
                    int radio = sphere_cell_dict[cell].radio;

                    //Sphere sphere = sphere_cell_dict[cell];

                    foreach (var key_value in pos_cell_dict)
                    {
                        if (Utils.EuclideanDistance(key_value.Key, cell.pos) == (radio + 1))
                        {
                            count++;
                            cell_list.Add(key_value.Value);
                        }
                        else if (Utils.EuclideanDistance(key_value.Key, cell.pos) == (radio + 2))
                        {
                            count2++;
                            cell_list2.Add(key_value.Value);
                        }
                    }
                    if (count2 >= 10)
                    {
                        cell_list.AddRange(cell_list2);
                        sphere_cell_dict[cell].radio = radio + 1;
                        sphere_cell_dict[cell].cell_list.AddRange(cell_list);
                        //sphere_cell_dict[cell].cell_list.AddRange(cell_list2);
                        foreach (Cell item in cell_list)
                            cells_without_sphere.Remove(item);
                    }
                    else if (count >= 10)
                    {
                        sphere_cell_dict[cell].radio = radio + 1;
                        sphere_cell_dict[cell].cell_list.AddRange(cell_list);
                        foreach (Cell item in cell_list)
                            cells_without_sphere.Remove(item);

                        //Sphere sphere2 = sphere_cell_dict[cell];
                        //Console.WriteLine("Vamos a ver si dos esferas son iguales aunque cambie el radio");
                        //Console.WriteLine(sphere==sphere2);
                    }
                }
                else
                {
                    foreach (var key_value in pos_cell_dict)
                    {
                        if (Utils.EuclideanDistance(key_value.Key, cell.pos) == 1)
                        {
                            radio_1_count++;
                            radio_1_cells.Add(key_value.Value);
                        }
                        else if (Utils.EuclideanDistance(key_value.Key, cell.pos) == 2)
                        {
                            radio_2_count++;
                            radio_2_cells.Add(key_value.Value);
                        }
                    }
                    if (radio_2_count >= 10)
                    {
                        radio_1_cells.AddRange(radio_2_cells);
                        sphere_cell_dict.Add(cell, new Sphere(2, radio_1_cells));
                        foreach (Cell item in radio_1_cells)
                            cells_without_sphere.Remove(item);
                    }
                    else if (radio_1_count >= 10)
                    {
                        sphere_cell_dict.Add(cell, new Sphere(2, radio_1_cells));
                        foreach (Cell item in radio_1_cells)
                            cells_without_sphere.Remove(item);
                    }
                }
            }
        }

        //Esto deberia ser un metodo interno de la clase
        public static CellActions CellAction(Cell cell)
        {
            CellActions action = new CellActions();
            //action = (CellActions)rdm.Next(0, 3);
            action = CellActions.Division;
            return action;
        }

        public static void CellMigrate(Cell cell)
        {
            
        }

        public static void CellContaminate(Cell cell)
        {

        }

        #endregion
    }

}
