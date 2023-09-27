using System;
using System.Collections.Generic;
using System.Timers;


namespace AutomataCelular
{
    class Program
    {
        public static int time = 0;
        public static int distance = 5;
        public static TumorCell tumor_stem_cell = null;
        public static List<StemCell> tumor_cell_list = new List<StemCell>();
        public static List<StemCell> stem_cell_list = new List<StemCell>();
        public static Dictionary<Cell, Stack<Node>> node_path_dic = new Dictionary<Cell, Stack<Node>>();

        public static Cell[,] matrix = new Cell[50, 50];

        public static Graph graph;

        public static System.Timers.Timer aTimer;

        static Random rdm = new Random();
        public static int stem_cells_count = rdm.Next(0, 20);

        public static int[,] mov = { { -1, -1, -1, 0, 1, 1, 1, 0 }, { -1, 0, 1, 1, 1, 0, -1, -1 } };


        public static int limit_of_x = 50;
        public static int limit_of_y = 50;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static void Simulation()
        {
            StartCellularLifeInTheBrain();

            GetCellsThatSenseTheTumorSubstance();

            PathFromCellsToTumorCell();

            GenerateGraph();

            StemCellPath();
        }

        public static void SetTimer()
        {
            aTimer = new System.Timers.Timer(2000);

            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            foreach (Cell cell in node_path_dic.Keys)
            {
                Node node = node_path_dic[cell].Pop();
                cell.pos = node.pos;

            }
            Console.WriteLine(e.SignalTime);
        }

        public static void Draw()
        {

        }
        public static void StemCellPath()
        {
            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                node_path_dic.Add(tumor_cell_list[i], Path(tumor_cell_list[i].pos, tumor_cell_list[i].des_pos));
            }
        }

        public static void StartCellularLifeInTheBrain()
        {
            //Crear la primera celula tumoral y cambiar la posicion del tumor
            tumor_stem_cell = new TumorCell(new Pos(0,0));

            matrix[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y] = tumor_stem_cell;

            //Crear las celulas madres pluripotencial
            for (int i = 0; i < stem_cells_count; i++)
            {
                stem_cell_list.Add(new StemCell(GetRandomPosition()));
            }

            //HACER!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Crear las celulas astrocitos, pericitos, etc
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
            List<Pos> pos_list = GetCloserPositionToTheTumorCell(tumor_stem_cell.pos);

            for (int i = 0; i < tumor_cell_list.Count; i++)
            {
                tumor_cell_list[i].des_pos = pos_list[i];
                matrix[tumor_cell_list[i].pos.X, tumor_cell_list[i].pos.Y] = tumor_cell_list[i];
            }

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

            while(node.pos != init_pos)
            {
                path.Push(node);
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


        //ARREGLAR PARA OBTENER POSICIONES CON NUMEROS FLOTANTES
        public static List<Pos> GetCloserPositionToTheTumorCell(Pos cell_tumor_position)
        {
            

            List<Pos> pos_list = new List<Pos>();
            for (int i = 0; i < mov.GetLength(1); i++)
            {
                pos_list.Add(new Pos(cell_tumor_position.X + mov[0, i], cell_tumor_position.Y + mov[1, i]));
            }

            return pos_list;
        }

        public static bool ValidPosition(int x, int y)
        {
            return true;
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
            int x = rdm.Next(0 - limit_of_x, limit_of_x);
            int y = rdm.Next(0 - limit_of_y, limit_of_y);

            return new Pos(x, y);
        }
    }
}
