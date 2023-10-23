using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCelularLogic
{
    //public abstract class Action
    //{
    //    public Cell cell;
    //    public abstract void Execute();
    //}

    //public class Division: Action
    //{
    //    public Pos pos;
    //    Tumor tumor;
    //    public Division(Tumor tumor, Cell cell, Pos pos)
    //    {
    //        this.cell = cell;
    //        this.pos = pos;
    //        this.tumor = tumor;
    //    }

    //    //public override void Execute()
    //    //{
    //    //    Cell cell = new Cell(pos, new TumorCellBehavior(), new ClassicProbability(), LocationStatus.MatrixExtracelular);
    //    //    tumor.cell_list.Add(cell);
    //    //    EnvironmentLogic.pos_cell_dict.Add(pos, cell);
    //    //    EnvironmentLogic.cells_without_sphere.Add(cell);
    //    //    //cell_div_list.Add(cell);
    //    //}
    //}

    //public class Contaminate: Action
    //{
    //    public Cell cont_cell;
    //    Tumor tumor;
    //    public Contaminate(Tumor tumor, Cell cell, Cell cont_cell)
    //    {
    //        this.cell = cell;
    //        this.cont_cell = cont_cell;
    //        this.tumor = tumor;
    //    }
    //    public override void Execute()
    //    {
    //        if (cont_cell.cell_behavior is AstrocyteCellBehavior)
    //        {
    //            cont_cell.cell_behavior = new TumorAstrocyteCellBehavior();
    //            tumor.cell_list.Add(cont_cell);
    //            EnvironmentLogic.cells_without_sphere.Add(cont_cell);
    //        }
    //        else if(cont_cell.cell_behavior is NeuronCellBehavior)
    //        {
    //            cont_cell.cell_behavior = new TumorNeuronCellBehavior();
    //            tumor.cell_list.Add(cont_cell);
    //            EnvironmentLogic.cells_without_sphere.Add(cont_cell);
    //        }
    //        else if(cont_cell.cell_behavior is EndothelialCellBehavior)
    //        {
    //            if(cont_cell.loca_status == LocationStatus.MatrixExtracelular)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheGlialBasalLamina(cont_cell.pos);
    //                if(prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.GlialBasalLamina;
    //                }
    //            }
    //            else if(cont_cell.loca_status == LocationStatus.GlialBasalLamina)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheVascularBasalLamina(cont_cell.pos);
    //                if(prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.VascularBasalLamina;
    //                }
    //            }
    //            else if(cont_cell.loca_status == LocationStatus.VascularBasalLamina)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheSmoothVesselCells(cont_cell.pos);
    //                if (prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.SmoothVesselCells_time1;
    //                }
    //            }
    //            else if(cont_cell.loca_status == LocationStatus.SmoothVesselCells_time1)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheSmoothVesselCells(cont_cell.pos);
    //                if (prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.SmoothVesselCells_time2;
    //                }
    //            }
    //            else if (cont_cell.loca_status == LocationStatus.SmoothVesselCells_time2)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheSmoothVesselCells(cont_cell.pos);
    //                if (prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.SmoothVesselCells_time3;
    //                }
    //            }
    //            else if(cont_cell.loca_status == LocationStatus.SmoothVesselCells_time3)
    //            {
    //                float prob = cont_cell.move_prob.ProbabilityOfBreakingDownTheEndothelialBasalLamina(cont_cell.pos);
    //                if(prob >= 0.5)
    //                {
    //                    cont_cell.loca_status = LocationStatus.EndothelialBasalLamina;
    //                }
    //            }
    //            else if(cont_cell.loca_status == LocationStatus.EndothelialBasalLamina)
    //            {
    //                //AQUI YA ESTA DENTRO DEL VASO SANGUINEO
    //            }
    //        }
    //    }
    //}

    //public class Migrate: Action
    //{
    //    public override void Execute()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class NothingAction : Action
    //{
    //    public override void Execute() { }
    //}
}
