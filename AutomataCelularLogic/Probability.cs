using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    public abstract class Probability
    {
        public abstract double DivisionProbability(Cell cell, List<Cell> cell_list, Tumor tumor);
        public abstract double MigrateProbability(Pos pos, Tumor tumor);
        public abstract double ContaminateProbability(Pos pos, Cell cell);
        public abstract double ContamineProbability(Cell cell, List<Cell> cell_list, Tumor tumor);
        public abstract double AvascularGrowthProbability();
        //public abstract float ProbabilityOfBreakingDownTheGlialBasalLamina(Pos pos);
        //public abstract float ProbabilityOfBreakingDownTheVascularBasalLamina(Pos pos);
        //public abstract float ProbabilityOfBreakingDownTheEndothelialBasalLamina(Pos pos);
        //public abstract float ProbabilityOfBreakingDownTheSmoothVesselCells(Pos pos);

    }

    public class ClassicProbability : Probability
    {
        public ClassicProbability()
        {

        }

        public override double DivisionProbability(Cell cell, List<Cell> cell_list, Tumor tumor)
        {
            //Console.WriteLine("Celula en el metodo division");
            //Console.WriteLine("{0} {1} {2}", cell.pos.X, cell.pos.Y, cell.pos.Z);
            //Console.WriteLine(cell.behavior_state);
            //Console.WriteLine(cell.loca_state);

            //foreach (var item in cell_list)
            //{
            //    Console.WriteLine("Division probability");
            //    Console.WriteLine("{0} {1} {2}", item.pos.X, item.pos.Y, item.pos.Z);
            //    Console.WriteLine(item.behavior_state);
            //    Console.WriteLine(item.loca_state);
            //}

            //if (tumor.new_cells_count > tumor.cell_list.Count)
            //{
            int tumoral_cells_count = 0;
            foreach (Cell item in cell_list)
            {
                if (item.behavior_state == CellState.TumoralCell)
                    tumoral_cells_count++;
            }
            double prob = (double)tumoral_cells_count / cell_list.Count;
            return prob;
            //}
            //return 0;
        }

        public override double MigrateProbability(Pos pos, Tumor tumor)
        {
            if (tumor.tumor_stage == TumosStage.Vascular)
            {
                return 0f;
            }
            return 0f;
        }

        public override double ContaminateProbability(Pos pos, Cell cell)
        {
            return 0f;
        }

        public override double ContamineProbability(Cell cell, List<Cell> cell_list, Tumor tumor)
        {
            //Console.WriteLine("Celula en el metodo contamine");
            //Console.WriteLine("{0} {1} {2}", cell.pos.X, cell.pos.Y, cell.pos.Z);
            //Console.WriteLine(cell.behavior_state);
            //Console.WriteLine(cell.loca_state);

            //foreach (var item in cell_list)
            //{
            //    Console.WriteLine("contamine probability");
            //    Console.WriteLine("{0} {1} {2}", item.pos.X, item.pos.Y, item.pos.Z);
            //    Console.WriteLine(item.behavior_state);
            //    Console.WriteLine(item.loca_state);
            //}

            //if (tumor.new_cells_count > tumor.cell_list.Count)
            //{
            int tumoral_cells_count = 0;
            foreach (Cell item in cell_list)
            {
                if (item.behavior_state == CellState.TumoralCell)
                    tumoral_cells_count++;
            }
            double prob = (double)tumoral_cells_count / cell_list.Count;
            return prob;
            //}
            //return 0f;
        }

        public override double AvascularGrowthProbability()
        {
            throw new NotImplementedException();
        }


        //public override float ProbabilityOfBreakingDownTheGlialBasalLamina(Pos pos)
        //{
        //    int tumoral_cell_in_lamina_count = 0;
        //    int tumoral_cell_count = 0;
        //    for (int i = 0; i < Utils.mov_3d.Count; i++)
        //    {
        //        int[] array = Utils.mov_3d[i];
        //        int x = pos.X;
        //        int y = pos.Y;
        //        int z = pos.Z;
        //        Pos cell_pos = new Pos(x + array[0], y + array[1], z + array[2]);
        //        if (EnvironmentLogic.pos_cell_dict.ContainsKey(cell_pos))
        //        {
        //            if (EnvironmentLogic.pos_cell_dict[cell_pos].cell_behavior is TumorCellBehavior)
        //            {
        //                tumoral_cell_count++;
        //                if (EnvironmentLogic.pos_cell_dict[cell_pos].loca_status == LocationStatus.GlialBasalLamina)
        //                    tumoral_cell_in_lamina_count++;
        //            }
        //        }
        //    }
        //    return tumoral_cell_in_lamina_count / tumoral_cell_count;
        //}

        //public override float ProbabilityOfBreakingDownTheVascularBasalLamina(Pos pos)
        //{
        //    if (EnvironmentLogic.pos_cell_dict[pos].loca_status == LocationStatus.GlialBasalLamina)
        //    {
        //        int tumoral_cell_in_lamina_count = 0;
        //        int tumoral_cell_count = 0;
        //        for (int i = 0; i < Utils.mov_3d.Count; i++)
        //        {
        //            int[] array = Utils.mov_3d[i];
        //            int x = pos.X;
        //            int y = pos.Y;
        //            int z = pos.Z;
        //            Pos cell_pos = new Pos(x + array[0], y + array[1], z + array[2]);
        //            if (EnvironmentLogic.pos_cell_dict.ContainsKey(cell_pos))
        //            {
        //                if (EnvironmentLogic.pos_cell_dict[cell_pos].cell_behavior is TumorCellBehavior)
        //                {
        //                    tumoral_cell_count++;
        //                    if (EnvironmentLogic.pos_cell_dict[cell_pos].loca_status == LocationStatus.VascularBasalLamina)
        //                        tumoral_cell_in_lamina_count++;
        //                }
        //            }
        //        }
        //        return tumoral_cell_in_lamina_count / tumoral_cell_count;
        //    }
        //    return 0f;
        //}

        //public override float ProbabilityOfBreakingDownTheEndothelialBasalLamina(Pos pos)
        //{
        //    if (EnvironmentLogic.pos_cell_dict[pos].loca_status == LocationStatus.SmoothVesselCells_time3)
        //    {
        //        int tumoral_cell_in_lamina_count = 0;
        //        int tumoral_cell_count = 0;
        //        for (int i = 0; i < Utils.mov_3d.Count; i++)
        //        {
        //            int[] array = Utils.mov_3d[i];
        //            int x = pos.X;
        //            int y = pos.Y;
        //            int z = pos.Z;
        //            Pos cell_pos = new Pos(x + array[0], y + array[1], z + array[2]);
        //            if (EnvironmentLogic.pos_cell_dict.ContainsKey(cell_pos))
        //            {
        //                if (EnvironmentLogic.pos_cell_dict[cell_pos].cell_behavior is TumorCellBehavior)
        //                {
        //                    tumoral_cell_count++;
        //                    if (EnvironmentLogic.pos_cell_dict[cell_pos].loca_status == LocationStatus.EndothelialBasalLamina)
        //                        tumoral_cell_in_lamina_count++;
        //                }
        //            }
        //        }
        //        return tumoral_cell_in_lamina_count / tumoral_cell_count;
        //    }
        //    return 0f;
        //}

        //public override float ProbabilityOfBreakingDownTheSmoothVesselCells(Pos pos)
        //{

        //    LocationStatus loc_stat = LocationStatus.MatrixExtracelular;
        //    if (EnvironmentLogic.pos_cell_dict[pos].loca_status == LocationStatus.VascularBasalLamina)
        //        loc_stat = LocationStatus.SmoothVesselCells_time1;
        //    else if (EnvironmentLogic.pos_cell_dict[pos].loca_status == LocationStatus.SmoothVesselCells_time1)
        //        loc_stat = LocationStatus.SmoothVesselCells_time2;
        //    else if (EnvironmentLogic.pos_cell_dict[pos].loca_status == LocationStatus.SmoothVesselCells_time1)
        //        loc_stat = LocationStatus.SmoothVesselCells_time3;

        //    if (loc_stat != LocationStatus.MatrixExtracelular)
        //    {
        //        int tumoral_cell_in_lamina_count = 0;
        //        int tumoral_cell_count = 0;
        //        for (int i = 0; i < Utils.mov_3d.Count; i++)
        //        {
        //            int[] array = Utils.mov_3d[i];
        //            int x = pos.X;
        //            int y = pos.Y;
        //            int z = pos.Z;
        //            Pos cell_pos = new Pos(x + array[0], y + array[1], z + array[2]);
        //            if (EnvironmentLogic.pos_cell_dict.ContainsKey(cell_pos))
        //            {
        //                if (EnvironmentLogic.pos_cell_dict[cell_pos].cell_behavior is TumorCellBehavior)
        //                {
        //                    tumoral_cell_count++;
        //                    if (EnvironmentLogic.pos_cell_dict[cell_pos].loca_status == loc_stat)
        //                        tumoral_cell_in_lamina_count++;
        //                }
        //            }
        //        }
        //        return tumoral_cell_in_lamina_count / tumoral_cell_count;
        //    }
        //    return 0f;
        //}

        public float ProbabilityOfContaminatingAstrocyte()
        {
            return 0f;
        }

        
    }
    //public class MediumProbability : Probability
    //{
    //    public override float ContaminateProbability(Pos pos, Cell cell)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ContamineProbability(Pos pos, int new_cells_count)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float DivisionProbability(Pos pos, int radio, int distance, int new_cells_count)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float MigrateProbability(Pos pos, Tumor tumor)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheEndothelialBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheGlialBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheSmoothVesselCells(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheVascularBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class HardProbability : Probability
    //{
    //    public override float MigrateProbability(Pos pos, Tumor tumor)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    public override float ContaminateProbability(Pos pos, Cell cell)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheGlialBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheVascularBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheEndothelialBasalLamina(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ProbabilityOfBreakingDownTheSmoothVesselCells(Pos pos)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float DivisionProbability(Pos pos, int radio, int distance, int new_cells_count)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override float ContamineProbability(Pos pos, int new_cells_count)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
