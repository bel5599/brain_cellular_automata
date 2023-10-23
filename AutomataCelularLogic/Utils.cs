﻿using System;
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
                if (ValidPosition(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]))
                    neighbours.Add(grid[pos.X + array[0], pos.Y + array[1], pos.Z + array[2]]);
            }
            return neighbours;
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

        public static Pos MinDistancePos(List<Cell> pos_list, Pos cell_pos)
        {
            int min = int.MaxValue;
            Pos min_pos = cell_pos;
            for (int i = 0; i < pos_list.Count; i++)
            {
                if (pos_list[i].pos.X == cell_pos.X && pos_list[i].pos.Y == cell_pos.Y && pos_list[i].pos.Z == cell_pos.Z) return pos_list[i].pos;
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


        public static Pos GetRandomPosition(int lower_limit_x, int upper_limit_x, int lower_limit_y, int upper_limit_y, int lower_limit_z, int upper_limit_z)
        {
            //Random rdm = new Random();
            int x = rdm.Next(lower_limit_x, upper_limit_x);
            int y = rdm.Next(lower_limit_y, upper_limit_y);
            int z = rdm.Next(lower_limit_z, upper_limit_z);

            return new Pos(x, y, z);
        }

        public static int EuclideanDistance(Pos pos_tumor_cell, Pos pos_stem_cell)
        {
            int d = (int)Math.Sqrt(Math.Pow(pos_stem_cell.X - pos_tumor_cell.X, 2) + Math.Pow(pos_stem_cell.Y - pos_tumor_cell.Y, 2) + Math.Pow(pos_stem_cell.Z - pos_tumor_cell.Z, 2));

            return d;
        }

        public static Pos GetAdjacentPosition(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, Dictionary<Pos, Artery> pos_artery_dict)
        {
            Pos new_pos;
            for (int i = 0; i < mov_3d.Count; i++)
            {
                int[] array = mov_3d[i];
                new_pos = new Pos(pos.X + array[0], pos.Y + array[1], pos.Z + array[2]);

                if (!pos_cell_dict.ContainsKey(new_pos) && !pos_artery_dict.ContainsKey(new_pos))
                    return new_pos;
            }
            return null;
        }

        public static bool ValidPosition(Pos pos)
        {
            return pos.X >= 0 && pos.X < EnvironmentLogic.limit_of_x && pos.Y >= 0 && pos.Y < EnvironmentLogic.limit_of_y && pos.Z >= 0 && pos.Z < EnvironmentLogic.limit_of_z;
        }
        public static bool ValidPosition(int x, int y, int z)
        {
            return x >= 0 && x < EnvironmentLogic.limit_of_x && y >= 0 && y < EnvironmentLogic.limit_of_y && z >= 0 && z < EnvironmentLogic.limit_of_z;
        }
    }
}
