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

        public static int time = 0;
        public static int distance = 40;
        public static int limit_of_x = 40;
        public static int limit_of_y = 40;
        public static int limit_of_z = 40;

        public static int tumoral_cell_radio = 5;

        public static Cell tumor_stem_cell = null;
        public static List<Cell> tumor_cell_list = new List<Cell>();

        public static Dictionary<Pos, Cell> pos_cell_dict = new Dictionary<Pos, Cell>();

        //public static List<Cell> cell_list = new List<Cell>();

        public static List<Sphere> sphere_list = new List<Sphere>();

        public static Dictionary<Cell, Sphere> sphere_cell_dict = new Dictionary<Cell, Sphere>();

        //esta lista debe tener las celulas que no estan en ninguna esfera
        public static List<Cell> cells_without_sphere = new List<Cell>();

        public static List<Cell> cells_center_of_the_sphere_list = new List<Cell>();

        public static List<Cell> stem_cell_list = new List<Cell>();
        //public static Dictionary<Cell, Stack<Node>> node_path_dic = new Dictionary<Cell, Stack<Node>>();

        //VARIABLES PARA VER EL ERROR
        //public static Dictionary<Cell, List<Node>> node_path_draw_dic = new Dictionary<Cell, List<Node>>();
        public static Cell actual_cell = null;

        //public static Cell[,] matrix = new Cell[limit_of_x, limit_of_y];

        //public static Graph graph;

        //public static Timer aTimer;

        //List<Node> path_list = new List<Node>();


        static Random rdm = new Random();
        //public static int stem_cells_count = rdm.Next(0, 15);
        public static int stem_cells_count = 15;

        //public static int[,] mov = { { -1, -1, -1, 0, 1, 1, 1, 0 }, { -1, 0, 1, 1, 1, 0, -1, -1 } };
        public static List<int[]> mov_3d = new List<int[]>();


        static void Main(string[] args)
        {
            //console.writeline("hello world");

            //Console.WriteLine("Hello World");

            Simulation();



            //for (int i = 0; i < tumor_cell_list.Count; i++)
            //{
            //    Console.WriteLine("{0} {1} {2}", tumor_cell_list[i].pos.X, tumor_cell_list[i].pos.Y, tumor_cell_list[i].pos.Z);
            //}
            //Console.WriteLine("Finito");
            Console.WriteLine("Esferas");
            foreach (var item in sphere_cell_dict)
            {
                Console.WriteLine(item.Value.radio);
            }
            Console.ReadLine();

            //for (int i = 0; i < cells_without_sphere.Count; i++)
            //{
            //    Console.WriteLine("{0} {1} {2}", cells_without_sphere[i].pos.X, cells_without_sphere[i].pos.Y, cells_without_sphere[i].pos.Z);
            //}



            //settimer();
            //console.readline();

            //atimer.stop();
            //atimer.dispose();

            //console.writeline("terminating the application...");
        }

        public static void InitializeVariables()
        {
            mov_3d.Add(new[] { 0, 0, 1 });
            mov_3d.Add(new[] { 0, 0, -1 });
            mov_3d.Add(new[] { 0, 1, 0 });
            mov_3d.Add(new[] { 0, -1, 0 });
            mov_3d.Add(new[] { 0, 1, 1 });
            mov_3d.Add(new[] { 0, -1, -1 });
            mov_3d.Add(new[] { 1, 0, 0 });
            mov_3d.Add(new[] { -1, 0, 0 });
            mov_3d.Add(new[] { 1, 0, 1 });
            mov_3d.Add(new[] { -1, 0, -1 });
            mov_3d.Add(new[] { 1, 1, 0 });
            mov_3d.Add(new[] { -1, -1, 0 });
            mov_3d.Add(new[] { 1, 1, 1 });
            mov_3d.Add(new[] { -1, -1, -1 });
        }

        public static void Simulation()
        {
            InitializeVariables();

            StartCellularLifeInTheBrain();

            GetCellsThatSenseTheTumorSubstance();

            PathFromCellsToTumorCell();

            StemCellConvertToTumoralCell();

            int time = 0;
            while (time != 10)
            {
                time++;
                CellMove();
                //Console.WriteLine("Lista");
                Console.WriteLine(cells_without_sphere.Count);

            }
            Console.WriteLine("Termine");
            
            //Console.ReadLine();

            //GenerateGraph();

            //StemCellPath();


        }

        //private static void SetTimer()
        //{
        //    aTimer = new System.Timers.Timer(2000);

        //    aTimer.Elapsed += OnTimedEvent;
        //    aTimer.AutoReset = true;
        //    aTimer.Enabled = true;
        //}

        //public static void OnTimedEvent(Object source, ElapsedEventArgs e)
        //{
        //    if (EmptyPath()) return;

        //    //Console.Clear();

        //    //Console.WriteLine("Empieza de nuevo");
        //    //Drawwww();

        //    List<Cell> cell_without_path = new List<Cell>();


        //    foreach (Cell cell in node_path_dic.Keys)
        //    {

        //        if (node_path_dic[cell].Count != 0)
        //        {
        //            //Console.WriteLine("Posiciones");
        //            //Console.WriteLine("{0}", matrix[cell.pos.X, cell.pos.Y]);

        //            Node n = node_path_dic[cell].Peek();
        //            if (matrix[n.pos.X, n.pos.Y] == null)
        //            {
        //                Node node = node_path_dic[cell].Pop();
        //                //matrix[cell.pos.X, cell.pos.Y] = null;

        //                //Console.WriteLine("Se supone que aqui no debe de haber nada");
        //                //Console.WriteLine("{0}", matrix[node.pos.X, node.pos.Y]);

        //                cell.pos = node.pos;
        //                //matrix[node.pos.X, node.pos.Y] = cell;
        //            }
        //            else
        //                cell_without_path.Add(cell);
        //        }
        //    }
        //    //Draw();
        //    Clear_Stack(cell_without_path);

        //    Console.WriteLine(e.SignalTime);
        //    Console.WriteLine(tumor_cell_list.Count);
        //}

        //public static void Clear_Stack(List<Cell> cell_list)
        //{
        //    foreach (Cell item in cell_list)
        //    {
        //        WhithOutPath(item, node_path_dic[item].Peek());
        //    }
        //}

        //public static void WhithOutPath(Cell cell, Node node)
        //{
        //    foreach (Cell item in node_path_dic.Keys)
        //    {
        //        if (node_path_dic[item].Count == 0 && item.pos.X == node.pos.X && item.pos.Y == node.pos.Y)
        //        {
        //            Console.WriteLine("Posicion ocupada {0} {1}", node.pos.X, node.pos.Y);
        //            node_path_dic[cell] = new Stack<Node>();
        //            return;
        //        }
        //    }
        //}

        //public static void Drawwww()
        //{
        //    foreach (Cell item in node_path_draw_dic.Keys)
        //    {
        //        foreach (Node node in node_path_draw_dic[item])
        //        {
        //            Console.WriteLine("Node {0}, {1}", node.pos.X, node.pos.Y);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //public static bool EmptyPath()
        //{
        //    foreach (var item in node_path_dic.Values)
        //    {
        //        if (item.Count != 0) return false;
        //    }
        //    return true;
        //}

        //public static void Draw()
        //{
        //    for (int i = 0; i < matrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < matrix.GetLength(1); j++)
        //        {
        //            if (matrix[i, j] == null)
        //                Console.Write("  |");
        //            else if (matrix[i, j] is StemCell)
        //                Console.Write("CC |");
        //            else
        //                Console.Write("CT |");
        //        }
        //        Console.WriteLine();
        //        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        //    }
        //}
        //public static void StemCellPath()
        //{
        //    for (int i = 0; i < tumor_cell_list.Count; i++)
        //    {
        //        node_path_draw_dic.Add(tumor_cell_list[i], new List<Node>());
        //        actual_cell = tumor_cell_list[i];

        //        node_path_dic.Add(tumor_cell_list[i], Path(tumor_cell_list[i].pos, tumor_cell_list[i].des_pos));
        //    }


        //}

        public static void StartCellularLifeInTheBrain()
        {
            //Crear la primera celula tumoral y cambiar la posicion del tumor
            tumor_stem_cell = new Cell(new Pos(15, 15, 15), new TumorCellBehavior(), new ClassicProbability());

            //matrix[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y] = tumor_stem_cell;

            //Crear las celulas madres pluripotencial
            for (int i = 0; i < stem_cells_count; i++)
            {
                Pos new_pos;
                do
                {
                    new_pos = GetRandomPosition(0, limit_of_x, 0, limit_of_y, 0, limit_of_z);
                }
                while (ExistentPosition(new_pos));

                stem_cell_list.Add(new Cell(new_pos, new StemCellBehavior(), new ClassicProbability()));
                pos_cell_dict.Add(new_pos, stem_cell_list[stem_cell_list.Count - 1]);
            }

            //HACER!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Crear las celulas astrocitos, pericitos, etc
        }

        #region Surgimiento

        public static void GetCellsThatSenseTheTumorSubstance()
        {
            for (int i = 0; i < stem_cell_list.Count; i++)
            {
                Cell cell = stem_cell_list[i];
                if (EuclideanDistance(tumor_stem_cell.pos, cell.pos) <= distance)
                {
                    tumor_cell_list.Add(cell);
                }
            }
        }

        //cambiar el nombre del metodo
        //HAY QUE ARREGLAR PARA QUE EXISTAN LAS MISMAS CANTIDAD DE POSICIONES QUE DE CELULAS QUE SE VAN ACERCAR
        public static void PathFromCellsToTumorCell()
        {
            //tumor_cell_list
            //Dictionary<int, List<Pos>> pos_dict = GetCloserPositionToTheTumorCell(tumor_stem_cell.pos, tumor_cell_list.Count);
            //int index = 0;

            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                int x = tumor_stem_cell.pos.X;
                int y = tumor_stem_cell.pos.Y;
                int z = tumor_stem_cell.pos.Z;

                Pos pos = null;

                do
                {
                    pos = GetRandomPosition(x - tumoral_cell_radio, x + tumoral_cell_radio, y - tumoral_cell_radio, y + tumoral_cell_radio, z - tumoral_cell_radio, z + tumoral_cell_radio);
                }
                while (ExistentPosition(tumor_cell_list, pos, true));

                tumor_cell_list[i].des_pos = pos;
                pos_cell_dict.Add(pos, tumor_cell_list[i]);

                //while (index < pos_dict.Count && pos_dict[index].Count == 0)
                //{
                //    index++;
                //}

                //Pos actual_pos = MinDistance(pos_dict[index], tumor_cell_list[i].pos);
                //tumor_cell_list[i].des_pos = actual_pos;
                //pos_dict[index].Remove(actual_pos);
                //matrix[tumor_cell_list[i].pos.X, tumor_cell_list[i].pos.Y] = tumor_cell_list[i];
            }

        }

        public static void StemCellConvertToTumoralCell()
        {
            foreach (Cell cell in tumor_cell_list)
            {
                pos_cell_dict.Remove(cell.pos);
                cell.pos = cell.des_pos;
                cell.cell_behavior = new TumorCellBehavior();
                cells_center_of_the_sphere_list.Add(cell);
                cells_without_sphere.Add(cell);
                //cell_list.Add(new Cell(cell.des_pos, new TumorCellBehavior()));
            }
        }

        //HAY QUE GUARDAR EN ALGUN LADO SI ESA POSICION ESTA SIENDO UTILIZADA
        

        //HAY QUE BUSCAR OTRA FORMA DE CREAR LAS ARISTAS
        //public static void GenerateGraph()
        //{
        //    graph = new Graph();
        //    int count = 0;
        //    for (int i = 0; i < matrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < matrix.GetLength(1); j++)
        //        {
        //            if (matrix[i, j] != null)
        //                graph.AddNode(new Node(count, new Pos(i, j), "celula"));
        //            else
        //                graph.AddNode(new Node(count, new Pos(i, j), "vacio"));
        //            count++;
        //        }
        //    }

        //    GenerateEdges();
        //}

        //ARREGLAR ESTO, DEBE SER MAS EFICIENTE
        //public static void GenerateEdges()
        //{
        //    for (int i = 0; i < matrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < matrix.GetLength(1); j++)
        //        {
        //            object c = matrix[i, j];
        //            for (int k = 0; k < mov.GetLength(1); k++)
        //            {
        //                if(ValidPosition(i + mov[0,k], j + mov[1, k]))
        //                {
        //                    graph.AddEdge(new Edge(graph.FindNode(i, j), graph.FindNode(i + mov[0, k], j + mov[1, k])));
        //                }
        //            }
        //        }
        //    }
        //}
        //HAY QUE BUSCAR LA POSICION MAS CERCANA A LA POSICION DE LA CELULA QUE SE VA ACERCAR

        //public static Stack<Node> Path(Pos init_pos, Pos des_pos)
        //{
        //    BFS(graph, graph.FindNode(init_pos.X, init_pos.Y));
        //    Node node = graph.FindNode(des_pos.X, des_pos.Y);

        //    Stack<Node> path = new Stack<Node>();


        //    while(node.pos.X != init_pos.X && node.pos.Y != init_pos.Y)
        //    {
        //        path.Push(node);

        //        //LINEA PARA VER EL ERROR
        //        node_path_draw_dic[actual_cell].Add(node);

        //        //path_list.Add(node);

        //        node = node.pi;
        //    }
        //    return path;
        //}


        //public static void BFS(Graph g, Node s)
        //{
        //    foreach (var node in g.nodes)
        //    {
        //        node.color = "white";
        //        node.distance = int.MaxValue;
        //        node.pi = null;
        //    }
        //    s.color = "gray";
        //    s.distance = 0;
        //    s.pi = null;

        //    Queue<Node> queue = new Queue<Node>();
        //    queue.Enqueue(s);

        //    while(queue.Count != 0)
        //    {
        //        Node u = queue.Dequeue();
        //        if (matrix[u.pos.X, u.pos.Y] == null || u == s)
        //        {
        //            foreach (Node v in g.ady_list[u.name])
        //            {
        //                if (v.color == "white")
        //                {
        //                    v.color = "gray";
        //                    v.distance = u.distance + 1;
        //                    v.pi = u;
        //                    queue.Enqueue(v);
        //                }
        //            }
        //            u.color = "black";
        //        }

        //    }
        //}


        //ARREGLAR PARA OBTENER POSICIONES CON NUMEROS FLOTANTES
        //public static Dictionary<int, List<Pos>> GetCloserPositionToTheTumorCell(Pos cell_tumor_position, int cells_count)
        //{
        //    Pos actual_pos = cell_tumor_position;
        //    //List<Pos> pos_list = new List<Pos>();
        //    Dictionary<int, List<Pos>> pos_dict = new Dictionary<int, List<Pos>>();
        //    pos_dict.Add(0, new List<Pos>());

        //    int index = 0;
        //    int count = 0;

        //    while (count < cells_count)
        //    {
        //        for (int i = 0; i < mov_3d.Count; i++)
        //        {
        //            var array = mov_3d[i];
        //            //actual_pos = new Pos(actual_pos.X + array[0], actual_pos.Y + array[1], actual_pos.Z + array[2]);
        //            if (!ExistentPos(pos_dict, actual_pos.X + array[0], actual_pos.Y + array[1], actual_pos.Z + array[2]))
        //            {
        //                pos_dict[index].Add(new Pos(actual_pos.X + array[0], actual_pos.Y + array[1], actual_pos.Z + array[2]));
        //                count++;
        //                //VERFICAR SI LA POSICION NO ESTA OCUPADA


        //            }
        //        }
        //        actual_pos = GetPos(pos_dict, index);
        //        index++;
        //        pos_dict.Add(index, new List<Pos>());
        //    }
        //    return pos_dict;
        //}

        //public static void DrawList(Dictionary<int, List<Pos>> dict)
        //{
        //    foreach (int key in dict.Keys)
        //    {
        //        for (int j = 0; j < dict[key].Count; j++)
        //        {
        //            Console.Write("{0}, {1}; ", dict[key][j].X, dict[key][j].Y, dict[key][j].Z);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //CAMBIARLE EL NOMBRE AL METODO

        //Cambiar el nombre al metodo
        public static bool ExistentPosition(Pos pos)
        {
            foreach (var cell in stem_cell_list)
            {
                if (cell.pos.X == pos.X && cell.pos.Y == pos.Y && cell.pos.Z == pos.Z)
                    return true;
            }
            return false;
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

        //public static bool ExistentPosition(List<Cell> cell_list, Pos pos)
        //{
        //    foreach (Cell cell in cell_list)
        //    {
        //        if (cell.pos != null && cell.pos.X == pos.X && cell.pos.Y == pos.Y && cell.pos.Z == pos.Z) return true;
        //    }
        //    return false;
        //}
        public static Pos MinDistance(List<Pos> pos_list, Pos cell_pos)
        {
            int min = int.MaxValue;
            Pos min_pos = cell_pos;
            for (int i = 0; i < pos_list.Count; i++)
            {
                if (pos_list[i].X == cell_pos.X && pos_list[i].Y == cell_pos.Y && pos_list[i].Z == cell_pos.Z) return pos_list[i];
                {
                    int dist = EuclideanDistance(pos_list[i], cell_pos);
                    if (dist < min)
                    {
                        min = dist;
                        min_pos = pos_list[i];
                    }
                }
            }
            return min_pos;
        }
        public static int EuclideanDistance(Pos pos_tumor_cell, Pos pos_stem_cell)
        {
            int d = (int)Math.Sqrt(Math.Pow(pos_stem_cell.X - pos_tumor_cell.X, 2) + Math.Pow(pos_stem_cell.Y - pos_tumor_cell.Y, 2) + Math.Pow(pos_stem_cell.Z - pos_tumor_cell.Z, 2));

            return d;
        }
        //public static Pos GetPos(Dictionary<int, List<Pos>> pos_dict, int index)
        //{
        //    int count = 0;

        //    //Console.WriteLine("Index");
        //    //Console.WriteLine(index);

        //    while (index >= pos_dict[count].Count)
        //    {
        //        index -= pos_dict[count].Count;
        //        count++;
        //    }
        //    //Console.WriteLine(index);
        //    return pos_dict[count][index];
        //}
        //public static bool ExistentPos(Dictionary<int, List<Pos>> pos_dict, int x, int y, int z)
        //{
        //    foreach (List<Pos> item in pos_dict.Values)
        //    {
        //        foreach (Pos pos in item)
        //            if (pos.X == x && pos.Y == y && pos.Z == z) return true;
        //    }

        //    return tumor_stem_cell.pos.X == x && tumor_stem_cell.pos.Y == y && tumor_stem_cell.pos.Z == z;
        //}

        //public static bool ValidPosition(int x, int y)
        //{
        //    return x >= 0 && y >= 0 && x < matrix.GetLength(0) && y < matrix.GetLength(1);
        //}
        public static bool ValidPosition(Pos pos)
        {
            return false;
        }
        public static Pos GetRandomPosition(int lower_limit_x, int upper_limit_x, int lower_limit_y, int upper_limit_y, int lower_limit_z, int upper_limit_z)
        {
            //Random rdm = new Random();
            int x = rdm.Next(lower_limit_x, upper_limit_x);
            int y = rdm.Next(lower_limit_y, upper_limit_y);
            int z = rdm.Next(lower_limit_z, upper_limit_z);

            return new Pos(x, y, z);
        }

        #endregion

        #region Grow up

        public static void CellMove()
        {
            List<Cell> cell_div_list = new List<Cell>();

            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                tumor_cell_list[i].actual_action = CellAction(tumor_cell_list[i]);
                if(tumor_cell_list[i].actual_action == CellActions.division)
                {
                    Cell cell = CellDivision(tumor_cell_list[i]);
                    if (cell != null)
                    {
                        cell_div_list.Add(cell);
                        pos_cell_dict.Add(cell.pos, cell);
                    }
                }

            }
            Console.WriteLine("Nueva lista");
            for (int i = 0; i < cell_div_list.Count; i++)
            {
                Console.WriteLine("{0} {1} {2}", cell_div_list[i].pos.X, cell_div_list[i].pos.Y, cell_div_list[i].pos.Z);
            }
            tumor_cell_list.AddRange(cell_div_list);
            cells_without_sphere.AddRange(cell_div_list);
            FormationOfSpheres();
            
            
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
                        if (EuclideanDistance(key_value.Key, cell.pos) == (radio + 1))
                        {
                            count++;
                            cell_list.Add(key_value.Value);
                        }
                        else if (EuclideanDistance(key_value.Key, cell.pos) == (radio + 2))
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
                        if (EuclideanDistance(key_value.Key, cell.pos) == 1)
                        {
                            radio_1_count++;
                            radio_1_cells.Add(key_value.Value);
                        }
                        else if (EuclideanDistance(key_value.Key, cell.pos) == 2)
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
            action = CellActions.division;
            return action;
        }

        public static Cell CellDivision(Cell cell)
        {
            bool new_pos = false;

            //VERIFICAR QUE AUNQUE SEA EXISTE ALGUNA POSICION ADYACENTE VACIA

            while (!new_pos)
            {
                int p = rdm.Next(0, mov_3d.Count);

                var array = mov_3d[p];
                Pos pos = new Pos(cell.pos.X + array[0], cell.pos.Y + array[1], cell.pos.Z + array[2]);

                //!ExistentPosition(tumor_cell_list, pos, false)
                if (!pos_cell_dict.ContainsKey(pos))
                {
                    new_pos = true;
                    float prob = cell.move_prob.DivideProbability(cell.pos, mov_3d, pos_cell_dict, tumoral_cell_radio, EuclideanDistance(tumor_stem_cell.pos, cell.pos));
                    if(prob >= 0.5)
                    {
                        return new Cell(pos, new TumorCellBehavior(), new ClassicProbability());
                    }
                    return null;
                    
                    //Aplicar una probabilidad para dividirse a una posicion x
                }
            }
            return null;
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
