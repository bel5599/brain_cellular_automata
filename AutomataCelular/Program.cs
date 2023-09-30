using System;
using System.Collections.Generic;
using System.Timers;


namespace AutomataCelular
{
    class Program
    {
        private static System.Timers.Timer aTimer;

        public static int time = 0;
        public static int distance = 20;
        public static int limit_of_x = 40;
        public static int limit_of_y = 40;

        public static TumorCell tumor_stem_cell = null;
        public static List<StemCell> tumor_cell_list = new List<StemCell>();
        public static List<StemCell> stem_cell_list = new List<StemCell>();
        public static Dictionary<Cell, Stack<Node>> node_path_dic = new Dictionary<Cell, Stack<Node>>();

        public static Dictionary<Cell, List<Node>> node_path_draw_dic = new Dictionary<Cell, List<Node>>();
        public static Cell actual_cell = null;

        public static Cell[,] matrix = new Cell[limit_of_x, limit_of_y];

        public static Graph graph;

        //public static Timer aTimer;

        List<Node> path_list = new List<Node>();


        static Random rdm = new Random();
        //public static int stem_cells_count = rdm.Next(0, 15);
        public static int stem_cells_count = 15;

        public static int[,] mov = { { -1, -1, -1, 0, 1, 1, 1, 0 }, { -1, 0, 1, 1, 1, 0, -1, -1 } };


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");

            Simulation();
            SetTimer();
            Console.ReadLine();

            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("Terminating the application...");
        }

        public static void Simulation()
        {
            StartCellularLifeInTheBrain();

            GetCellsThatSenseTheTumorSubstance();

            PathFromCellsToTumorCell();

            GenerateGraph();

            StemCellPath();

            
        }

        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(2000);

            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (EmptyPath()) return;
            
            //Console.Clear();
            
            foreach (Cell cell in node_path_dic.Keys)
            {
                //DrawList(node_path_dic);
                if (node_path_dic[cell].Count != 0)
                {
                    Console.WriteLine("Posiciones");
                    Console.WriteLine("{0}", matrix[cell.pos.X, cell.pos.Y]);

                    Node node = node_path_dic[cell].Pop();
                    matrix[cell.pos.X, cell.pos.Y] = null;

                    Console.WriteLine("Se supone que aqui no debe de haber nada");
                    Console.WriteLine("{0}", matrix[node.pos.X, node.pos.Y]);

                    cell.pos = node.pos;
                    matrix[node.pos.X, node.pos.Y] = cell;
                }
            }
            Draw();

            Console.WriteLine(e.SignalTime);
            Console.WriteLine(tumor_cell_list.Count);
        }

        public static bool EmptyPath()
        {
            foreach (var item in node_path_dic.Values)
            {
                if (item.Count != 0) return false;
            }
            return true;
        }

        public static void Draw()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == null)
                        Console.Write("  |");
                    else if (matrix[i, j] is StemCell)
                        Console.Write("CC |");
                    else
                        Console.Write("CT |");
                }
                Console.WriteLine();
                Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
            }
        }
        public static void StemCellPath()
        {
            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                node_path_draw_dic.Add(tumor_cell_list[i], new List<Node>());
                actual_cell = tumor_cell_list[i];

                node_path_dic.Add(tumor_cell_list[i], Path(tumor_cell_list[i].pos, tumor_cell_list[i].des_pos));
            }


        }

        //public static void DrawPath()
        //{
        //    for (int i = 0; i < length; i++)
        //    {

        //    }
        //}

        public static void StartCellularLifeInTheBrain()
        {
            //Crear la primera celula tumoral y cambiar la posicion del tumor
            tumor_stem_cell = new TumorCell(new Pos(15,15));

            matrix[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y] = tumor_stem_cell;

            //Crear las celulas madres pluripotencial
            for (int i = 0; i < stem_cells_count; i++)
            {
                Pos new_pos;
                do
                {
                    new_pos = GetRandomPosition();
                }
                while (ExistentPosition(new_pos));
                
                stem_cell_list.Add(new StemCell(new_pos));
            }

            //HACER!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Crear las celulas astrocitos, pericitos, etc
        }

        //Cambiar el nombre al metodo
        public static bool ExistentPosition(Pos pos)
        {
            foreach (var cell in stem_cell_list)
            {
                if (cell.pos.X == pos.X && cell.pos.Y == pos.Y)
                    return true;
            }
            return false;
        }

        public static void GetCellsThatSenseTheTumorSubstance()
        {
            for (int i = 0; i < stem_cell_list.Count; i++)
            {
                StemCell cell = stem_cell_list[i];
                if(EuclideanDistance(tumor_stem_cell.pos, cell.pos) <= distance)
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
            Dictionary<int, List<Pos>> pos_dict = GetCloserPositionToTheTumorCell(tumor_stem_cell.pos, tumor_cell_list.Count);
            int index = 0;

            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                while(index < pos_dict.Count && pos_dict[index].Count == 0)
                {
                    index++;
                }

                Pos actual_pos = MinDistance(pos_dict[index], tumor_cell_list[i].pos);
                tumor_cell_list[i].des_pos = actual_pos;
                pos_dict[index].Remove(actual_pos);
                matrix[tumor_cell_list[i].pos.X, tumor_cell_list[i].pos.Y] = tumor_cell_list[i];
            }
        }

        public static Pos MinDistance(List<Pos> pos_list, Pos cell_pos)
        {
            int min = int.MaxValue;
            Pos min_pos = cell_pos;
            for (int i = 0; i < pos_list.Count; i++)
            {
                if (pos_list[i].X == cell_pos.X && pos_list[i].Y == cell_pos.Y) return pos_list[i];

                if (matrix[pos_list[i].X, pos_list[i].Y] == null)
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

        //HAY QUE BUSCAR OTRA FORMA DE CREAR LAS ARISTAS
        public static void GenerateGraph()
        {
            graph = new Graph();
            int count = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] != null)
                        graph.AddNode(new Node(count, new Pos(i, j), "celula"));
                    else
                        graph.AddNode(new Node(count, new Pos(i, j), "vacio"));
                    count++;
                }
            }
            
            GenerateEdges();
        }

        //ARREGLAR ESTO, DEBE SER MAS EFICIENTE
        public static void GenerateEdges()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    object c = matrix[i, j];
                    for (int k = 0; k < mov.GetLength(1); k++)
                    {
                        if(ValidPosition(i + mov[0,k], j + mov[1, k]))
                        {
                            graph.AddEdge(new Edge(graph.FindNode(i, j), graph.FindNode(i + mov[0, k], j + mov[1, k])));
                        }
                    }
                }
            }
        }
        //HAY QUE BUSCAR LA POSICION MAS CERCANA A LA POSICION DE LA CELULA QUE SE VA ACERCAR

        public static Stack<Node> Path(Pos init_pos, Pos des_pos)
        {
            BFS(graph, graph.FindNode(init_pos.X, init_pos.Y));
            Node node = graph.FindNode(des_pos.X, des_pos.Y);

            Stack<Node> path = new Stack<Node>();


            while(node.pos.X != init_pos.X && node.pos.Y != init_pos.Y)
            {
                path.Push(node);

                node_path_draw_dic[actual_cell].Add(node);

                //path_list.Add(node);

                node = node.pi;
            }
            return path;
        }


        public static void BFS(Graph g, Node s)
        {
            foreach (var node in g.nodes)
            {
                node.color = "white";
                node.distance = int.MaxValue;
                node.pi = null;
            }
            s.color = "gray";
            s.distance = 0;
            s.pi = null;

            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(s);

            while(queue.Count != 0)
            {
                Node u = queue.Dequeue();
                if (matrix[u.pos.X, u.pos.Y] == null || u == s)
                {
                    foreach (Node v in g.ady_list[u.name])
                    {
                        if (v.color == "white")
                        {
                            v.color = "gray";
                            v.distance = u.distance + 1;
                            v.pi = u;
                            queue.Enqueue(v);
                        }
                    }
                    u.color = "black";
                }
                
            }
        }


        //ARREGLAR PARA OBTENER POSICIONES CON NUMEROS FLOTANTES
        public static Dictionary<int, List<Pos>> GetCloserPositionToTheTumorCell(Pos cell_tumor_position, int cells_count)
        {
            Pos actual_pos = cell_tumor_position;
            //List<Pos> pos_list = new List<Pos>();
            Dictionary<int, List<Pos>> pos_dict = new Dictionary<int, List<Pos>>();
            pos_dict.Add(0, new List<Pos>());

            int index = 0;
            int count = 0;

            while(count < cells_count)
            {
                //count = 0;
                for (int i = 0; i < mov.GetLength(1); i++)
                {
                    if (!ExistentPos(pos_dict, actual_pos.X + mov[0, i], actual_pos.Y + mov[1, i]))
                    {
                        pos_dict[index].Add(new Pos(actual_pos.X + mov[0, i], actual_pos.Y + mov[1, i]));
                        count++;
                    }
                }
                
                //actual_pos = pos_dict[index][count];
                actual_pos = GetPos(pos_dict, index);
                index++;
                pos_dict.Add(index, new List<Pos>());

                DrawList(pos_dict);
            }
            

            return pos_dict;
        }

        public static void DrawList(Dictionary<int, List<Pos>> dict)
        {
            foreach (int key in dict.Keys)
            {
                for (int j = 0; j < dict[key].Count; j++)
                {
                    Console.Write("{0}, {1}; ", dict[key][j].X, dict[key][j].Y);
                }
                Console.WriteLine();
            }
        }

        //CAMBIARLE EL NOMBRE AL METODO
        public static Pos GetPos(Dictionary<int, List<Pos>> pos_dict, int index)
        {
            int count = 0;

            Console.WriteLine("Index");
            Console.WriteLine(index);

            while(index >= pos_dict[count].Count)
            {
                index -= pos_dict[count].Count;
                count++;
            }
            Console.WriteLine(index);
            return pos_dict[count][index];
        }

        public static bool ExistentPos(Dictionary<int, List<Pos>> pos_dict, int x, int y)
        {
            foreach (List<Pos> item in pos_dict.Values)
            {
                foreach (Pos pos in item)
                    if (pos.X == x && pos.Y == y) return true;
            }

            return tumor_stem_cell.pos.X == x && tumor_stem_cell.pos.Y == y;
        }

        public static bool ValidPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < matrix.GetLength(0) && y < matrix.GetLength(1);
        }
        public static bool ValidPosition(Pos pos)
        {
            return false;
        }

        public static int EuclideanDistance(Pos pos_tumor_cell, Pos pos_stem_cell)
        {
            int d = (int)Math.Sqrt(Math.Pow(pos_stem_cell.X - pos_tumor_cell.X, 2) + Math.Pow(pos_stem_cell.Y - pos_tumor_cell.Y, 2));

            return d;
        }

        public static Pos GetRandomPosition()
        {
            //Random rdm = new Random();
            int x = rdm.Next(0 , limit_of_x);
            int y = rdm.Next(0 , limit_of_y);

            return new Pos(x, y);
        }
    }
}
