using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AutomataCelularLogic
{
    public enum StrahlerOrder
    {
        StrahlerOrder_1,
        StrahlerOrder_2,
        StrahlerOrder_3
    }
    public class World
    {
        public Cell[,,] world;
        public Children blood_vessels_tree;
        public List<Pos> blood_vessels;
        public Dictionary<Pos, Children> pos_children;

        public Cell tumor_stem_cell;

        public List<EdgeTree> edges;

        private double radius;

        //PARAMETROS RELACIONADOS CON LOS ORDEN DE STRAHLER EN LOS VASOS SANGUINEOS
        public Dictionary<Tuple<Pos, Pos>, StrahlerOrder> edge_order_dict;

        private int stem_cells_count;
        private int astrocytes_count;
        private int neuron_count;

        //public List<Cell> stem_cell_list;
        public Dictionary<Pos, Cell> stem_cell_list;
        //public List<Cell> astrocyte_list;
        public Dictionary<Pos, Cell> astrocyte_list;
        //public List<Cell> neuron_list;
        public Dictionary<Pos, Cell> neuron_list;

        public Dictionary<Pos, BloodVessel> pos_artery_dict;
        public Dictionary<Pos, Cell> pos_cell_dict;
        public World(int height, int width, int depth, double radius, int stem_cells_count, int astrocytes_count, int neuron_count)
        {
            world = new Cell[height, width, depth];
            
            this.radius = radius;

            this.stem_cells_count = stem_cells_count;
            this.astrocytes_count = astrocytes_count;
            this.neuron_count = neuron_count;

            AmountNormalCells();

            InicializarListas();
            StartCellularLifeInTheBrain();

            //Edges();
        }

        public void InicializarListas()
        {
            pos_artery_dict = new Dictionary<Pos, BloodVessel>();
            pos_cell_dict = new Dictionary<Pos, Cell>();

            edge_order_dict = new Dictionary<Tuple<Pos, Pos>, StrahlerOrder>();
            pos_children = new Dictionary<Pos, Children>();

            edges = new List<EdgeTree>();

            astrocyte_list = new Dictionary<Pos, Cell>();
            neuron_list = new Dictionary<Pos, Cell>();
            stem_cell_list = new Dictionary<Pos, Cell>();
        }
        public void StartCellularLifeInTheBrain()
        {
            Stopwatch crono = new Stopwatch();
            crono.Start();
            CreateBloodVesselsTree(world.GetLength(0));
            crono.Stop();
            Console.WriteLine("CreateBloodVesselsTree: {0} segundos", crono.ElapsedMilliseconds/1000);

            crono.Start();
            LocateTheBloodVesselsOnTheBoard();
            crono.Stop();
            Console.WriteLine("LocateTheBloodVesselsOnTheBoard: {0} segundos", crono.ElapsedMilliseconds/1000);

            StartTumoraCell();

            crono.Start();
            CreateCells();
            crono.Stop();
            Console.WriteLine("CreateCells: {0} segundos", crono.ElapsedMilliseconds/1000);

            crono.Start();
            RellenarHuecos();
            crono.Stop();
            Console.WriteLine("RellenarHuecos: {0} segundos", crono.ElapsedMilliseconds/1000);
        }
        public void StartTumoraCell()
        {
            Pos pos;
            do
            {
                pos = Utils.GetRandomPosition(0, world.GetLength(0), 0, world.GetLength(1), 0, world.GetLength(2));
            }
            while (pos_artery_dict.ContainsKey(pos));

            tumor_stem_cell = new Cell(pos, CellState.TumoralStemCell, CellLocationState.MatrixExtracelular);
            tumor_stem_cell.proliferation_age = -1;
            //tumor_stem_cell.neighborhood = Utils.GetMooreNeighbours3D(tumor_stem_cell.pos, space);
            //pos_cell_dict.Add(tumor_stem_cell.pos, tumor_stem_cell);

            world[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y, tumor_stem_cell.pos.Z] = tumor_stem_cell;

        }

        public void LocateTheBloodVesselsOnTheBoard()
        {
            foreach (Pos pos in blood_vessels)
            {
                if (!pos_artery_dict.ContainsKey(pos))
                {
                    BloodVessel blood_vessel = new BloodVessel(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                    pos_artery_dict.Add(pos, blood_vessel);
                    world[pos.X, pos.Y, pos.Z] = blood_vessel;
                }
            }
        }

        public void CreateBloodVesselsTree(int limit)
        {
            List<Pos> tree = new List<Pos>();
            Pos root_pos = Utils.GetRandomPosition(0, limit, 0, limit, 0, limit);
            tree.Add(root_pos);

            Children root = new Children(root_pos);
            pos_children.Add(root_pos, root);

            blood_vessels_tree = CreateBloodVesselsTree(root, root_pos, 0, 15, new List<Pos>(), tree);
            blood_vessels = tree;
        }

        public bool PointOnLine(Pos A, Pos B, Pos P)
        {
            // Calcula el vector director de la recta
            double dx = B.X - A.X;
            double dy = B.Y - A.Y;
            double dz = B.Z - A.Z;

            // Normaliza el vector director
            double length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            dx /= length;
            dy /= length;
            dz /= length;

            // Calcula el vector desde A hasta P
            double px = P.X - A.X;
            double py = P.Y - A.Y;
            double pz = P.Z - A.Z;

            // Calcula el producto escalar de AP y el vector unitario de la recta
            double projection = dx * px + dy * py + dz * pz;

            // Calcula la distancia desde A hasta P
            double distance = Math.Sqrt(px * px + py * py + pz * pz);

            // Comprueba si P está en la recta
            return Math.Abs(projection - distance) < 0.00001;
        }

        public Pos RandomPos(Pos pos, List<Pos> marks)
        {
            List<Pos> empty_pos = Utils.EmptyPositionsInARadius(pos, marks, world.GetLength(0), 15);
            //for (int i = 0; i < empty_pos.Count; i++)
            //{
            //    Console.WriteLine(Utils.EuclideanDistance(pos, empty_pos[i]));
            //}
            if (empty_pos.Count > 0)
                return empty_pos[Utils.rdm.Next(0, empty_pos.Count)];
            return new Pos(-1, -1, -1);
        }

        

        public Pos RandomAdjPos(Pos pos, List<Pos> marks)
        {
            List<Pos> empty_pos = Utils.EmptyPositions(pos, marks, world.GetLength(0));
            //for (int i = 0; i < empty_pos.Count; i++)
            //{
            //    Console.WriteLine(Utils.EuclideanDistance(pos, empty_pos[i]));
            //}
            if(empty_pos.Count > 0)
                return empty_pos[Utils.rdm.Next(0, empty_pos.Count)];
            return new Pos(-1,-1,-1);
        }

        public Children CreateBloodVesselsTree(Children root, Pos pos, int depth, int max_depth, List<Pos> marks, List<Pos> pos_list)
        {
            Pos new_pos = RandomAdjPos(pos, marks);
            if (new_pos.X != -1 && new_pos.Y != -1 && new_pos.Z != -1)
            {
                Children c = new Children(new_pos);
                pos_children.Add(new_pos, c);
                pos_list.Add(new_pos);
                marks.Add(pos);

                if (depth == 0)
                {
                    var r = Utils.rdm.Next(0, 2);
                    if (r == 0)
                    {
                        root.child_left = c;
                        root.child_right = null;
                    }
                    else
                    {
                        root.child_right = c;
                        root.child_left = null;
                    }

                    edge_order_dict.Add(new Tuple<Pos, Pos>(pos, new_pos), StrahlerOrder.StrahlerOrder_3);
                }

                if (depth == max_depth)
                {
                    c.child_left = c.child_right = null;

                    marks.Add(new_pos);

                    edge_order_dict.Add(new Tuple<Pos, Pos>(pos, new_pos), StrahlerOrder.StrahlerOrder_1);
                    return c;
                }
                else
                {
                    if (depth != 0)
                        edge_order_dict.Add(new Tuple<Pos, Pos>(pos, new_pos), StrahlerOrder.StrahlerOrder_2);

                    int i = Utils.rdm.Next(0, 3);

                    if (i == 0)
                        c.child_left = CreateBloodVesselsTree(root, c.pos, depth + 1, max_depth, marks, pos_list);
                    else if (i == 1)
                        c.child_right = CreateBloodVesselsTree(root, c.pos, depth + 1, max_depth, marks, pos_list);
                    else
                    {
                        c.child_left = CreateBloodVesselsTree(root, c.pos, depth + 1, max_depth, marks, pos_list);
                        c.child_right = CreateBloodVesselsTree(root, c.pos, depth + 1, max_depth, marks, pos_list);
                    }

                    if (depth == 0)
                        return root;
                    else
                        return c;
                }
            }
            else
            {
                marks.Add(pos);
                return null;
            }
        }

        public void AmountNormalCells()
        {
            int count =(int)((world.GetLength(0) * world.GetLength(1) * world.GetLength(2)) * 0.0005);
            int count_1 = count / 3;

            stem_cells_count = 20;
            astrocytes_count = count_1;
            neuron_count = count_1;
        }

        public void CreateCells()
        {
            CreateCells(CellState.StemCell, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(stem_cells_count));
            CreateCells(CellState.Astrocyte, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(astrocytes_count));
            CreateCells(CellState.Neuron, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(neuron_count));

        }

        public void CreateCellsByRadius(int radius)
        {
            int astrocytes_count_per_segment = astrocytes_count / edge_order_dict.Count;
            int neuron_count_per_segment = neuron_count / edge_order_dict.Count;
            foreach (var item in edge_order_dict)
            {
                Pos pos1 = item.Key.Item1;
                Pos pos2 = item.Key.Item2;
                //Pos medio = new Pos((pos1.X + pos2.X) / 2, (pos1.Y + pos2.Y) / 2, (pos1.Z + pos2.Z) / 2);


                for (int i = 0; i < astrocytes_count_per_segment; i++)
                {
                    List<Pos> astrocyte = GeneratePoissonDiskPoints(astrocytes_count_per_segment, pos1.X,pos1.Y, pos1.Z,20,20,20);
                }
            }
        }

        public List<Pos> GeneratePoissonDiskPoints(int amount, int ini_x, int limit_of_x, int ini_y, int limit_of_y, int ini_z, int limit_of_z)
        {
            List<Pos> poissonDiskPoints = new List<Pos>();

            for (int i = 0; i < amount; i++)
            {
                Pos point = Utils.GenerateRandomPoint(radius, ini_x, limit_of_x, ini_y, limit_of_y, ini_z, limit_of_z);
                while (IsTooClose(point, poissonDiskPoints) || pos_artery_dict.ContainsKey(point))
                {
                    point = Utils.GenerateRandomPoint(radius, ini_x, limit_of_x, ini_y, limit_of_y, ini_z, limit_of_z);
                }
                poissonDiskPoints.Add(point);
            }

            return poissonDiskPoints;
        }

        public void CreateCells(CellState cell_state, CellLocationState loca_state, List<Pos> random_positions)
        {
            foreach (Pos pos in random_positions)
            {
                Cell cell = new Cell(pos, cell_state, loca_state);
                cell.proliferation_age = -1;
                world[pos.X, pos.Y, pos.Z] = cell;

                if (cell_state == CellState.Astrocyte)
                    astrocyte_list.Add(cell.pos, cell);
                else if (cell_state == CellState.Neuron)
                    neuron_list.Add(cell.pos, cell);
                else
                    stem_cell_list.Add(cell.pos, cell);
            }
        }

        public void RellenarHuecos()
        {
            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    for (int k = 0; k < world.GetLength(2); k++)
                    {
                        if (world[i, j, k] == null)
                            world[i, j, k] = new Cell(new Pos(i, j, k), CellState.nothing, CellLocationState.MatrixExtracelular);
                    }
                }
            }
        }
        

        //public void CreateBloodVesselsTreeNoRecursivo(Pos pos, int depth, int max_depth, List<Pos> marks, List<Pos> pos_list)
        //{
        //    Children actual = new Children(pos);

        //    while(depth < max_depth)
        //    {
        //        Pos new_pos = RandomAdjPos(pos, marks);

        //    }
        //}

        public void Edges()
        {
            Children temp = blood_vessels_tree;
            List<Children> children_list = new List<Children>();

            

            children_list.Add(temp);

            int i = 0;

            while(i < children_list.Count)
            {
                temp = children_list[i];

                if (temp.child_left != null)
                {
                    edges.Add(new EdgeTree(temp.pos, temp.child_left.pos));
                    children_list.Add(temp.child_left);
                }
                if (temp.child_right != null)
                {
                    edges.Add(new EdgeTree(temp.pos, temp.child_right.pos));
                    children_list.Add(temp.child_right);
                }

                i++;
            }
        }


        public bool IsTooClose(Pos point, List<Pos> points)
        {
            foreach (Pos existingPoint in points)
            {
                //aqui hay que poner la distancia euclidiana
                double distance = Math.Sqrt(Math.Pow(point.X - existingPoint.X, 2) + Math.Pow(point.Y - existingPoint.Y, 2) + Math.Pow(point.Z - existingPoint.Z, 2));
                if (distance < radius)
                    return true;
            }
            return false;
        }

        public List<Pos> GeneratePoissonDiskPoints(int amount)
        {
            List<Pos> poissonDiskPoints = new List<Pos>();

            for (int i = 0; i < amount; i++)
            {
                Pos point = Utils.GenerateRandomPoint(radius, 0, world.GetLength(0), 0, world.GetLength(1), 0, world.GetLength(2));
                while (IsTooClose(point, poissonDiskPoints) || pos_artery_dict.ContainsKey(point))
                {
                    point = Utils.GenerateRandomPoint(radius, 0, world.GetLength(0), 0, world.GetLength(1), 0, world.GetLength(2));
                }
                poissonDiskPoints.Add(point);
            }

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        for (int z = 0; z < depth; z++)
            //        {
            //            Pos point = Utils.GenerateRandomPoint(radius);
            //            while (IsTooClose(point, poissonDiskPoints))
            //            {
            //                point = Utils.GenerateRandomPoint(radius);
            //            }
            //            poissonDiskPoints.Add(point);
            //        }
            //    }
            //}
            return poissonDiskPoints;
        }



        //public class PoissonDiskGenerator
        //{
        //    private static Random random = new Random();
        //    private static double radius = 1.0;
        //    private static List<Point3D> points = new List<Point3D>();

        //    public static Point3D GenerateRandomPoint()
        //    {
        //        double x = random.NextDouble() * radius * 2 - radius;
        //        double y = random.NextDouble() * radius * 2 - radius;
        //        double z = random.NextDouble() * radius * 2 - radius;
        //        return new Point3D { X = x, Y = y, Z = z };
        //    }

        //    public static bool IsTooClose(Point3D point, List<Point3D> points)
        //    {
        //        foreach (Point3D existingPoint in points)
        //        {
        //            double distance = Math.Sqrt(Math.Pow(point.X - existingPoint.X, 2) + Math.Pow(point.Y - existingPoint.Y, 2) + Math.Pow(point.Z - existingPoint.Z, 2));
        //            if (distance < radius)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }

        //    public static List<Point3D> GeneratePoissonDiskPoints(int width, int height, int depth)
        //    {
        //        List<Point3D> poissonDiskPoints = new List<Point3D>();
        //        for (int x = 0; x < width; x++)
        //        {
        //            for (int y = 0; y < height; y++)
        //            {
        //                for (int z = 0; z < depth; z++)
        //                {
        //                    Point3D point = GenerateRandomPoint();
        //                    while (IsTooClose(point, poissonDiskPoints))
        //                    {
        //                        point = GenerateRandomPoint();
        //                    }
        //                    poissonDiskPoints.Add(point);
        //                }
        //            }
        //        }
        //        return poissonDiskPoints;
        //    }
        //}

    }

    public class Children 
    {
        public Pos pos;
        public Children child_left;
        public Children child_right;
        public Children(Pos pos)
        {
            this.pos = pos;
        }
    }

    public class EdgeTree
    {
        public Pos pos1;
        public Pos pos2;
        public EdgeTree(Pos pos1, Pos pos2)
        {
            this.pos1 = pos1;
            this.pos2 = pos2;
        }
    }
}
