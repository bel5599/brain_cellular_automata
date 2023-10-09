using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public abstract class Probability
    {
        public abstract float DivideProbability(Pos pos, List<int[]> mov3d, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance);
    }

    public class ClassicProbability: Probability
    {
        public ClassicProbability()
        {

        }

        public override float DivideProbability(Pos pos, List<int[]> mov3d, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            int free_pos_count = 14;
            for (int i = 0; i < mov3d.Count; i++)
            {
                int[] array = mov3d[i];
                int x = pos.X;
                int y = pos.Y;
                int z = pos.Z;
                if (pos_cell_dict.ContainsKey(new Pos(x + array[0], y + array[1], z + array[2])))
                    free_pos_count--;
            }

            return free_pos_count/14 * distance/radio;
        }
    }
    public class MediumProbability : Probability
    {
        public override float DivideProbability(Pos pos, List<int[]> mov3d, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            throw new NotImplementedException();
        }
    }
    public class HardProbability : Probability
    {
        public override float DivideProbability(Pos pos, List<int[]> mov3d, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            throw new NotImplementedException();
        }
    }
}
