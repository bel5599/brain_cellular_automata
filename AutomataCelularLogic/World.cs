using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public class World
    {
        public Cell[,,] world;
        public Children blood_vessels_tree;
        public List<Pos> blood_vessels;

        public Cell tumor_stem_cell;

        private List<Pos> points;
        private double radius;

        private int stem_cells_count;
        private int astrocytes_count;
        private int neuron_count;

        public Dictionary<Pos, Artery> pos_artery_dict;
        public Dictionary<Pos, Cell> pos_cell_dict;
        public World(int height, int width, int depth, double radius, int stem_cells_count, int astrocytes_count, int neuron_count)
        {
            world = new Cell[height, width, depth];
            
            points = new List<Pos>();
            this.radius = radius;

            this.stem_cells_count = stem_cells_count;
            this.astrocytes_count = astrocytes_count;
            this.neuron_count = neuron_count;

            InicializarListas();
            StartCellularLifeInTheBrain();
        }

        public void InicializarListas()
        {
            pos_artery_dict = new Dictionary<Pos, Artery>();
            pos_cell_dict = new Dictionary<Pos, Cell>();
        }
        public void StartCellularLifeInTheBrain()
        {
            CreateBloodVesselsTree(5);
            LocateTheBloodVesselsOnTheBoard();
            StartTumoraCell();
            CreateCells();
            RellenarHuecos();
        }
        public void StartTumoraCell()
        {
            tumor_stem_cell = new Cell(new Pos(35, 35, 35), CellState.ProliferativeTumoralCell, CellLocationState.MatrixExtracelular);
            tumor_stem_cell.proliferation_age = -1;
            //tumor_stem_cell.neighborhood = Utils.GetMooreNeighbours3D(tumor_stem_cell.pos, space);
            //pos_cell_dict.Add(tumor_stem_cell.pos, tumor_stem_cell);

            world[tumor_stem_cell.pos.X, tumor_stem_cell.pos.Y, tumor_stem_cell.pos.Z] = tumor_stem_cell;

        }

        public void LocateTheBloodVesselsOnTheBoard()
        {
            foreach (Pos pos in blood_vessels)
            {
                Artery artery = new Artery(pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                pos_artery_dict.Add(pos, artery);
                world[pos.X, pos.Y, pos.Z] = artery;
            }
        }

        public void CreateBloodVesselsTree(int limit)
        {
            List<Pos> tree = new List<Pos>();
            Pos root = Utils.GetRandomPosition(0, limit, 0, limit, 0, limit);
            tree.Add(root);

            blood_vessels_tree = CreateBloodVesselsTree(root, 0, 3, new List<Pos>(), tree);
            blood_vessels = tree;
        }

        public Pos RandomAdjPos(Pos pos, List<Pos> marks)
        {
            List<Pos> empty_pos = Utils.EmptyPositions(pos, marks);
            return empty_pos[Utils.rdm.Next(0, empty_pos.Count)];
        }

        public Children CreateBloodVesselsTree(Pos pos, int depth, int max_depth, List<Pos> marks, List<Pos> pos_list)
        {
            Pos new_pos = RandomAdjPos(pos, marks);
            Children c = new Children(new_pos);
            pos_list.Add(new_pos);
            marks.Add(pos);

            if (depth == max_depth)
            {
                c.child_left = c.child_right = null;
                return c;
            }
            else
            {
                int i = Utils.rdm.Next(0, 3);

                if (i == 0)
                    c.child_left = CreateBloodVesselsTree(c.pos, depth+1, max_depth, marks, pos_list);
                else if (i == 1)
                    c.child_right = CreateBloodVesselsTree(c.pos, depth + 1, max_depth, marks, pos_list);
                else
                {
                    c.child_left = CreateBloodVesselsTree(c.pos, depth + 1, max_depth, marks, pos_list);
                    c.child_right = CreateBloodVesselsTree(c.pos, depth + 1, max_depth, marks, pos_list);
                }
                return c;
            }
        }

        public void CreateCells()
        {
            CreateCells(CellState.StemCell, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(stem_cells_count));
            CreateCells(CellState.Astrocyte, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(astrocytes_count));
            CreateCells(CellState.Neuron, CellLocationState.MatrixExtracelular, GeneratePoissonDiskPoints(neuron_count));

        }

        public void CreateCells(CellState cell_state, CellLocationState loca_state, List<Pos> random_positions)
        {
            foreach (Pos pos in random_positions)
            {
                Cell cell = new Cell(pos, cell_state, loca_state);
                cell.proliferation_age = -1;
                world[pos.X, pos.Y, pos.Z] = cell;
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


        public bool IsTooClose(Pos point, List<Pos> points)
        {
            foreach (Pos existingPoint in points)
            {
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
                Pos point = Utils.GenerateRandomPoint(radius, world.GetLength(0), world.GetLength(1), world.GetLength(2));
                while (IsTooClose(point, poissonDiskPoints))
                {
                    point = Utils.GenerateRandomPoint(radius, world.GetLength(0), world.GetLength(1), world.GetLength(2));
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
}
