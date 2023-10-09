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

        //public static Pos MinDistance(List<Pos> pos_list, Pos cell_pos)
        //{
        //    int min = int.MaxValue;
        //    Pos min_pos = cell_pos;
        //    for (int i = 0; i < pos_list.Count; i++)
        //    {
        //        if (pos_list[i].X == cell_pos.X && pos_list[i].Y == cell_pos.Y && pos_list[i].Z == cell_pos.Z) return pos_list[i];
        //        {
        //            int dist = EuclideanDistance(pos_list[i], cell_pos);
        //            if (dist < min)
        //            {
        //                min = dist;
        //                min_pos = pos_list[i];
        //            }
        //        }
        //    }
        //    return min_pos;
        //}

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
    }
}
