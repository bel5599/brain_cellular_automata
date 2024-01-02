using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{

    public static class Utils
    {
        public static Random rdm = new Random();

        //VARIABLES DE MOVIMIENTO EN UN ESPACIO 3D
        public static List<int[]> mov_3d = new List<int[]>();

        public static List<Cell> GetMooreNeighbours3D(Pos pos, Cell[,,] grid)
        {
            List<Cell> neighbours = new List<Cell>();
            for (int i = 0; i < mov_3d.Count; i++)
            {
                int[] array= mov_3d[i];
                Pos new_pos = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);
                //new_pos = ValidatingAPositionInAClosedSpace(new_pos, grid.GetLength(0));

                if (ValidPosition(new_pos.X, new_pos.Y, new_pos.Z))
                    neighbours.Add(grid[new_pos.X, new_pos.Y, new_pos.Z]);
            }
            return neighbours;
        }

        public static Pos ValidatingAPositionInAClosedSpace(Pos pos, int limit_of_x)
        {
            int x = pos.X;
            if (x < 0)
            {
                x = limit_of_x + x;
                pos.X = x;
            }
            else if (x >= limit_of_x)
            {
                x = x - limit_of_x;
                pos.X = x;
            }

            return pos;
        }

        public static Pos NullPos()
        {
            return new Pos(-1, -1, -1);
        }

        public static void InitializeVariables()
        {
            mov_3d.Add(new[] { 0, 0, 1 });
            mov_3d.Add(new[] { 0, 0, -1 });

            mov_3d.Add(new[] { 0, 1, 1 });
            mov_3d.Add(new[] { 0, -1, 1 });

            mov_3d.Add(new[] { 0, 1, -1 });
            mov_3d.Add(new[] { 0, -1, -1 });

            mov_3d.Add(new[] { 0, 1, 0 });
            mov_3d.Add(new[] { 0, -1, 0 });

            mov_3d.Add(new[] { 1, 0, 1 });
            mov_3d.Add(new[] { 1, 0, -1 });

            mov_3d.Add(new[] { 1, 1, 1 });
            mov_3d.Add(new[] { 1, -1, 1 });

            mov_3d.Add(new[] { 1, 1, -1 });
            mov_3d.Add(new[] { 1, -1, -1 });

            mov_3d.Add(new[] { 1, 1, 0 });
            mov_3d.Add(new[] { 1, -1, 0 });

            mov_3d.Add(new[] { 1, 0, 0 });

            mov_3d.Add(new[] { -1, 0, 1 });
            mov_3d.Add(new[] { -1, 0, -1 });

            mov_3d.Add(new[] { -1, 1, 1 });
            mov_3d.Add(new[] { -1, -1, 1 });

            mov_3d.Add(new[] { -1, 1, -1 });
            mov_3d.Add(new[] { -1, -1, -1 });

            mov_3d.Add(new[] { -1, 1, 0 });
            mov_3d.Add(new[] { -1, -1, 0 });

            mov_3d.Add(new[] { -1, 0, 0 });


        }

        public static List<Pos> EmptyPositionsInARadius(Pos pos, List<Pos> marks, int limit_of_x, int radius)
        {
            List<Pos> empty_pos = new List<Pos>();

            for (int i = 0; i < mov_3d.Count; i++)
            {
                int[] array = mov_3d[i];
                Pos new_pos = new Pos(pos.X + (array[0]* radius), pos.Y + (array[1]* radius), pos.Z + (array[2]* radius));
                //new_pos = ValidatingAPositionInAClosedSpace(new_pos, limit_of_x);
                if (ValidPosition(new_pos) && !marks.Contains(new_pos))
                    empty_pos.Add(new_pos);
            }
            return empty_pos;
        }

        public static List<Pos> EmptyPositions(Pos pos, List<Pos> marks)
        {
            List<Pos> empty_pos = new List<Pos>();

            for (int i = 0; i < mov_3d.Count; i++)
            {
                int[] array = mov_3d[i];
                Pos new_pos = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);
                //new_pos = ValidatingAPositionInAClosedSpace(new_pos, limit_of_x);
                if (ValidPosition(new_pos) && !marks.Contains(new_pos))
                    empty_pos.Add(new_pos);
            }
            return empty_pos;
        }

        public static List<Cell> NormalCellCount(Pos pos, List<Cell> cell_list)
        {
            List<Cell> empty_pos = new List<Cell>();
            foreach (var item in cell_list)
            {
                if (item.behavior_state == CellState.Neuron || item.behavior_state == CellState.Astrocyte)/* && */
                {
                    empty_pos.Add(item);
                }

            }
            return empty_pos;
        }

        public static List<Cell> EmptyPositions(List<Cell> cell_list, List<Cell> occupied_cells = null)
        {
            List<Cell> empty_pos = new List<Cell>();
            foreach (var item in cell_list)
            {
                if (item.behavior_state == CellState.nothing && item.loca_state == CellLocationState.MatrixExtracelular)/* && */
                {
                    if(occupied_cells != null)
                    {
                        if (!occupied_cells.Contains(item))
                            empty_pos.Add(item);
                    }
                    else
                        empty_pos.Add(item);
                }
                    
            }
            return empty_pos;
        }
        public static Pos MinDistancePos(List<Cell> pos_list, Dictionary<Pos,Pos> existing_cells, Pos cell_pos)
        {
            int min = int.MaxValue;
            Pos min_pos = cell_pos;
            for (int i = 0; i < pos_list.Count; i++)
            {
                if (pos_list[i].pos.X == cell_pos.X && pos_list[i].pos.Y == cell_pos.Y && pos_list[i].pos.Z == cell_pos.Z) return pos_list[i].pos;
                else if(pos_list[i].behavior_state == CellState.nothing && !existing_cells.ContainsKey(pos_list[i].pos))
                {
                    int dist = EuclideanDistance(pos_list[i].pos, cell_pos);
                    if (dist < min)
                    {
                        min = dist;
                        min_pos = pos_list[i].pos;
                    }
                }
            }
            return min_pos;
        }

        //public static bool EquationOfTheLine(int x, int y, Pos pos, int m)
        //{
        //    //return y - pos.Y = m*(x)

        //}

        public static Pos GetRandomPosition(int lower_limit_x, int upper_limit_x, int lower_limit_y, int upper_limit_y, int lower_limit_z, int upper_limit_z)
        {
            //Random rdm = new Random();
            int x = rdm.Next(lower_limit_x, upper_limit_x);
            int y = rdm.Next(lower_limit_y, upper_limit_y);
            int z = rdm.Next(lower_limit_z, upper_limit_z);

            return new Pos(x, y, z);
        }

        public static Pos GetRandomPositionTree(int lower_limit_x, int upper_limit_x, int lower_limit_y, int upper_limit_y, int lower_limit_z, int upper_limit_z)
        {
            //Random rdm = new Random();
            int x = GetIntValue(lower_limit_x, upper_limit_x);
            int y = GetIntValue(lower_limit_y, upper_limit_y);
            int z = GetIntValue(lower_limit_z, upper_limit_z);

            return new Pos(x, y, z);
        }

        public static int GetIntValue(int min, int max)
        {
            if (min == max) return min;

            if (max < min) return max;

            double d = Math.Abs(max - min) + 1;
            double interval = 1d / d;
            double myRandom = rdm.NextDouble();

            var numbers = new List<int>();
            for (int i = min; i <= max; i++) numbers.Add(i);

            double a, b;
            for (int i = 1; i <= d; i++)
            {
                a = (i - 1) * interval;
                b = i * interval;

                if (myRandom.Belongs(a, b)) return numbers[i - 1];
            }

            return 0;
        }

        public static bool Belongs(this double n, double min, double max)
        {
            return n >= min && n <= max;
        }

        public static Pos GenerateRandomPoint(double radius, int ini_height, int height, int ini_width, int width, int ini_depth, int depth)
        {
            int x;
            int y;
            int z;
            do
                x = (int)(rdm.Next(ini_height, height/2) * radius * 2 - radius);
            while (x < 0);
            do
                y = (int)(rdm.Next(ini_width, width/2) * radius * 2 - radius);
            while (y < 0);
            do
                z = (int)(rdm.Next(ini_depth, depth/2) * radius * 2 - radius);
            while (z < 0);

            return new Pos { X = x, Y = y, Z = z };
        }

        public static int EuclideanDistance(Pos pos_tumor_cell, Pos pos_stem_cell)
        {
            int d = (int)Math.Sqrt(Math.Pow(pos_stem_cell.X - pos_tumor_cell.X, 2) + Math.Pow(pos_stem_cell.Y - pos_tumor_cell.Y, 2) + Math.Pow(pos_stem_cell.Z - pos_tumor_cell.Z, 2));

            return d;
        }

        public static Pos GetAdjacentPosition(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, Dictionary<Pos, Cell> pos_artery_dict)
        {
            Pos new_pos;
            for (int i = 0; i < mov_3d.Count; i++)
            {
                int[] array = mov_3d[i];
                new_pos = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);

                if (!pos_cell_dict.ContainsKey(new_pos) && !pos_artery_dict.ContainsKey(new_pos))
                    return new_pos;
            }
            return new Pos(-1, -1, -1);
        }

        public static bool ValidPosition(Pos pos)
        {
            return pos.X >= 0 && pos.X < EnvironmentLogic.limit_of_x && pos.Y >= 0 && pos.Y < EnvironmentLogic.limit_of_y && pos.Z >= 0 && pos.Z < EnvironmentLogic.limit_of_z;
        }
        public static bool ValidPosition(int x, int y, int z)
        {
            //return /*x >= 0 && x < EnvironmentLogic.limit_of_x &&*/ y >= 0 && y < 10 && z >= 0 && z < 10;
            return x >= 0 && x < EnvironmentLogic.limit_of_x && y >= 0 && y < EnvironmentLogic.limit_of_y && z >= 0 && z < EnvironmentLogic.limit_of_z;
        }

        //public static void FormationOfSpheres()
        //{
        //    CellActions action = new CellActions();
        //    //action = (CellActions)rdm.Next(0, 3);
        //    action = CellActions.Division;
        //    return action;
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
    }
}
