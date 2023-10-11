using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public abstract class Probability
    {
        public abstract float DivisionProbability(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance);
        public abstract float MigrateProbability(Pos pos, Tumor tumor);
        public abstract float ContaminateProbability(Pos pos, Cell cell);
        public abstract float ContamineProbability(Pos pos);
    }

    public class ClassicProbability: Probability
    {
        public ClassicProbability()
        {

        }

        public override float DivisionProbability(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            int free_pos_count = 14;
            for (int i = 0; i < Utils.mov_3d.Count; i++)
            {
                int[] array = Utils.mov_3d[i];
                int x = pos.X;
                int y = pos.Y;
                int z = pos.Z;
                if (pos_cell_dict.ContainsKey(new Pos(x + array[0], y + array[1], z + array[2])))
                    free_pos_count--;
            }

            return free_pos_count/14 * distance/radio;
        }

        public override float MigrateProbability(Pos pos, Tumor tumor)
        {
            if (tumor.tumor_stage == TumosStage.Vascular)
            {
                return 0f;
            }
            return 0f;
        }

        public override float ContaminateProbability(Pos pos, Cell cell)
        {
            return 0f;
        }
        
        public override float ContamineProbability(Pos pos)
        {
            return 0f;
        }


        public float ProbabilityOfContaminatingAstrocyte()
        {
            return 0f;
        }
    }
    public class MediumProbability : Probability
    {
        public override float ContaminateProbability(Pos pos, Cell cell)
        {
            throw new NotImplementedException();
        }

        public override float ContamineProbability(Pos pos)
        {
            throw new NotImplementedException();
        }

        public override float DivisionProbability(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            throw new NotImplementedException();
        }

        public override float MigrateProbability(Pos pos, Tumor tumor)
        {
            throw new NotImplementedException();
        }
    }
    public class HardProbability : Probability
    {
        public override float DivisionProbability(Pos pos, Dictionary<Pos, Cell> pos_cell_dict, int radio, int distance)
        {
            throw new NotImplementedException();
        }

        public override float MigrateProbability(Pos pos, Tumor tumor)
        {
            throw new NotImplementedException();
        }
        public override float ContaminateProbability(Pos pos, Cell cell)
        {
            throw new NotImplementedException();
        }

        public override float ContamineProbability(Pos pos)
        {
            throw new NotImplementedException();
        }
    }
}
