using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutomataCelularLogic
{
    public class MathematicalModel
    {
        //public double difussion_coeficient_oxygen;
        
        public Cell[,,] space;

        double dx;
        double dy;
        double dz;

        //cancer cell density
        private double n_0 = 1.6 * Math.Pow(10, -5);

        //umbral para una celula muera por apostosis
        private double c_ap;

        #region PARAMETROS RELACIONADOS CON LA CONCENTRACION DE OXIGENO

        private double diffussion_coeficient_oxygen = 1.8 * Math.Pow(10, -5); // Difusividad
        //The base oxygen consumption rate
        private double r_c = 4.5 * Math.Pow(10, -17);

        //background oxigen concentration
        private double c_0 = 1.7 * Math.Pow(10, -8);
        private double c_n;

        private double[] consumption_rate;
        public double[] boundary_conditions = { 0.28 };
        public Dictionary<CellState, double[]> metabolic_rate;

        public double[,,] oxygen_matrix;
        public double[,,] oxygen_discret_matrix;
        public double[,,] oxygen_delta;

        private int t_disc;
        private Pos pos_disc;
        private double oxigen_conc_disc;
        private double dif_conc_disc;
        private double oxy_consp_rate_disc;
        #endregion

        #region PARAMETROS RELACIONADOS CON LA DENSIDAD DE LA MATRIX EXTRACELULAR

        public double[,,] density_matrix;

        #endregion

        #region PARAMETROS RELACIONADOS CON LA CONCENTRACION DE REGULADORES ANGIOGENICOS

        private double diffusion_coeficient_angio = 2.0;
        private double natural_disintegration_rate = 6.0;
        //tasa de transferencia del suministro desde las celulas hipoxicas
        private double Sc = Math.Pow(10, 3);
        //este valor hay que calcularlo con la matrix de celulas
        private double hypoxic_cell_volume_fraction;
        //nivel de saturacion
        private double c_sat = 1.0;

        public double[,,] angiogenic_reg_conc_matrix;
        public double[,,] angio_reg_conc_delta;


        #endregion


        private int t = 16;
        public int time;
        private int dt;

        

        public MathematicalModel(Cell[,,] space)
        {
            this.space = space;
            c_n = 0.15 * c_0;
            c_ap = 0.15 * c_0;

            dx = 1.0 / space.GetLength(0);
            dy = 1.0 / space.GetLength(1);
            dz = 1.0 / space.GetLength(2);

            dt = 0;

            oxygen_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            oxygen_discret_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            oxygen_delta = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            density_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            angiogenic_reg_conc_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            angio_reg_conc_delta = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            consumption_rate = new double[] { 2 * r_c, 5 * r_c, 10 * r_c };

            DiscretizeTheOxygenDiffusionCoefficient();
            DiscretizeOxygenConsumptionRate();

            metabolic_rate = new Dictionary<CellState, double[]>();
            double[] proliferation_rate = { r_c, oxy_consp_rate_disc };
            double[] quiescent_rate = { proliferation_rate[0]/5, proliferation_rate[1] / 5 };
            double[] migratory_rate = { proliferation_rate[0] / 2, proliferation_rate[1] / 5 };
            double[] necrotic = { 0 };
            metabolic_rate.Add(CellState.ProliferativeTumoralCell, proliferation_rate);
            metabolic_rate.Add(CellState.QuiescentTumorCell, quiescent_rate);
            metabolic_rate.Add(CellState.MigratoryTumorCell, migratory_rate);
            metabolic_rate.Add(CellState.nothing, necrotic);

            InitializeConcentrationMatrix();
            InitializeDensityMatrix();
        }
        public double[,,] Laplacian3D(double[,,] u, double dx, double dy, double dz)
        {
            int nx = u.GetLength(0), ny = u.GetLength(1), nz = u.GetLength(2);
            double[,,] u_laplacian = new double[nx, ny, nz];

            for (int i = 1; i < nx - 1; i++)
            {
                for (int j = 1; j < ny - 1; j++)
                {
                    for (int k = 1; k < nz - 1; k++)
                    {
                        u_laplacian[i, j, k] = (u[i - 1, j, k] - 2 * u[i, j, k] + u[i + 1, j, k]) / (dx * dx) +
                                             (u[i, j - 1, k] - 2 * u[i, j, k] + u[i, j + 1, k]) / (dy * dy) +
                                             (u[i, j, k - 1] - 2 * u[i, j, k] + u[i, j, k + 1]) / (dz * dz);
                    }
                }
            }

            
            //foreach (var item in u_laplacian)
            //{
            //    Console.WriteLine(item);
            //}
            return u_laplacian;
        }

        public static double Laplacian3D(double[,,] u, Pos pos, double dx, double dy, double dz)
        {
            int i = pos.X;
            int j = pos.Y;
            int k = pos.Z;

            return (u[i - 1, j, k] - 2 * u[i, j, k] + u[i + 1, j, k]) / (dx * dx) +
                   (u[i, j - 1, k] - 2 * u[i, j, k] + u[i, j + 1, k]) / (dy * dy) +
                   (u[i, j, k - 1] - 2 * u[i, j, k] + u[i, j, k + 1]) / (dz * dz);
        }

        private void InitializeConcentrationMatrix()
        {
            for (int i = 0; i < oxygen_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < oxygen_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < oxygen_matrix.GetLength(2); k++)
                    {
                        if (space[i, j, k].behavior_state != CellState.nothing)
                            oxygen_matrix[i, j, k] = oxygen_discret_matrix[i,j,k] = 1.0;
                    }
                }
            }
        }

        private void InitializeDensityMatrix()
        {
            for (int i = 0; i < density_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < density_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < density_matrix.GetLength(2); k++)
                    {
                        var cell = space[i, j, k];
                        if (cell.behavior_state == CellState.nothing && cell.loca_state == CellLocationState.MatrixExtracelular)
                            density_matrix[i, j, k] = Utils.rdm.NextDouble();
                    }
                }
            }
        }

        public void DiscretizeOxigenConcentration(double c)
        {
            oxigen_conc_disc = c / c_0;
        }

        public Pos DiscretizePos(Pos pos)
        {
            return new Pos(pos.X / space.GetLength(0), pos.Y / space.GetLength(1), pos.Z / space.GetLength(2));
        }

        public void DiscretizeTime(int time)
        {
            t_disc = time / t;
        }

        public void DiscretizeTheOxygenDiffusionCoefficient()
        {
            dif_conc_disc = diffussion_coeficient_oxygen * t / space.GetLength(0) * space.GetLength(1) * space.GetLength(2);
        }

        public void DiscretizeOxygenConsumptionRate()
        {
            oxy_consp_rate_disc = t * n_0 * r_c / c_0;
        }
        public void UpdateOxygenConcentration(Cell[,,] space, int time)
        {
            this.space = space;
            DiscretizeTime(time);
            oxygen_delta = Laplacian3D(oxygen_matrix, dx, dy, dz);
            dt = t_disc;

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        CellState cell_state = space[i, j, k].behavior_state;
                        if (cell_state != CellState.ProliferativeTumoralCell && cell_state != CellState.QuiescentTumorCell && cell_state != CellState.MigratoryTumorCell)
                        {
                            oxygen_matrix[i, j, k] = boundary_conditions[0];
                        }
                        else
                        {
                            
                            //oxygen_matrix[i, j, k] += dt * (difussion_coeficient_oxygen * delta_oxygen[i, j, k] + metabolic_rate[CellState.TumoralCell][0]);

                            //oxygen_matrix[i, j, k] += dt * (difussion_coeficient_oxygen * delta_oxygen[i, j, k] - k * delta_oxygen[i, j, k] * v[i, j, k]);
                            double termino = time * (diffussion_coeficient_oxygen * oxygen_delta[i, j, k] * oxygen_matrix[i, j, k] - metabolic_rate[cell_state][0]);
                            double conc = oxygen_matrix[i, j, k] + (termino);
                            oxygen_matrix[i, j, k] += termino;

                            DiscretizeOxigenConcentration(oxygen_matrix[i, j, k]);

                            double t1 = dt * (dif_conc_disc * oxygen_delta[i, j, k] * oxigen_conc_disc - metabolic_rate[cell_state][1]);
                            double tt = oxygen_discret_matrix[i, j, k] + t1;
                            oxygen_discret_matrix[i,j,k] += t1;
                        }
                    }
                }
            }

            oxygen_matrix = DirichletBoundaryConditions(oxygen_matrix);
        }

        public void UpdateAngiogenicRegulatorConcentration(Cell[,,] space, int time)
        {
            this.space = space;
            angio_reg_conc_delta = Laplacian3D(angiogenic_reg_conc_matrix, dx, dy, dz);

            //Hay que calcular el volumen de las celulas hipoxicas

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        double c = angiogenic_reg_conc_matrix[i, j, k];
                        angiogenic_reg_conc_matrix[i,j,k] += time * (diffusion_coeficient_angio * angio_reg_conc_delta[i, j, k] - natural_disintegration_rate * c +
                                                        Sc * hypoxic_cell_volume_fraction * (c_sat - c));
                    }
                }
            }
        }

        public double[,,] DirichletBoundaryConditions(double[,,] oxygen_matrix)
        {
            for (int i = 0; i < oxygen_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < oxygen_matrix.GetLength(1); j++)
                {
                    oxygen_matrix[i, j, 0] = c_0;
                    oxygen_matrix[i, j, oxygen_matrix.GetLength(2) - 1] = c_0;
                    oxygen_matrix[i, 0, j] = c_0;
                    oxygen_matrix[i, oxygen_matrix.GetLength(1) - 1, j] = c_0;
                    oxygen_matrix[0, i, j] = c_0;
                    oxygen_matrix[oxygen_matrix.GetLength(0) - 1, i, j] = c_0;
                }
            }
            return oxygen_matrix;
        }


    }

    
}
