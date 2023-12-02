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

        public double transf_rate_from_pree_vessels = 0.25;
        public double pree_blood_vessels_density;

        public double transf_rate_from_neo_vessels;
        public double neo_blood_vessels_density;

        public double hematocrit_on_neo_network;
        public double normal_hematocrit_value;
        public double hematocrit_min;


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

        ///VARIABLES QUE ESTOY UTILIZANDO AHORA
        double beta_c = 3.34 * Math.Pow(10, -3);//s^-1
        double alfa_c = 1.67 * Math.Pow(10, -5);//s^-1

        double diffusion_coeficient_oxygen = 1.67 * Math.Pow(10, -7);//cm^2/s
        double p_e_oxygen = 1 * Math.Pow(10, -3);//um/s
        double c_art_oxygen = 5.56 * 10;//uMolar

        public double oxygen_tumoral_threshold_1 = 1.175;
        public double oxygen_tumoral_threshold_2 = 3.525;
        public double oxygen_healthy_threshold_1 = 35.25;
        public double oxygen_healthy_threshold_2 = 352.5;

        #endregion

        #region PARAMETROS RELACIONADOS CON LA DENSIDAD DE LA MATRIX EXTRACELULAR

        public double[,,] density_matrix;

        double rate_of_degradation_of_ECM_by_MDE = 0.01;
        double rate_of_production_of_ECM_by_tumour_cells = 0.1;
        double rate_of_production_of_ECM_by_EC_sprout_tips = 0.1;

        public double[,,] mde;
        public double[,,] mde_delta;

        double diffusion_coefficient_MDE = 1.0;
        double prod_rate_of_MDE_by_tumour_cells = 100;
        double natural_decay_of_MDE = 10;
        double prod_rate_of_MDE_by_EC_sprout_tips = 1.0;

        //OTRO ARTICULO POR SUERTE ESTE SI FUNCIONA JJAJAJAJAJA
        double degrad_coeff_ECM = 1.3 * Math.Pow(10, 2);

        double prod_rate_of_MDE_by_tumour_cells_2 = 1.7 * Math.Pow(10, -18);
        double diffusion_coefficient_MDE_2 = Math.Pow(10, -9);
        double natural_decay_of_MDE_2 = 1.7 * Math.Pow(10, -8);

        

        #endregion

        #region PARAMETROS RELACIONADOS CON LA CONCENTRACION DE REGULADORES ANGIOGENICOS

        private double diffusion_coeficient_angio = 2.0;
        private double natural_desintegration_rate = 6.0;
        //tasa de transferencia del suministro desde las celulas hipoxicas
        private double Sc = Math.Pow(10, 3);
        //este valor hay que calcularlo con la matrix de celulas
        private double hypoxic_cell_volume_fraction;
        //nivel de saturacion
        private double c_sat = 1.0;

        public double[,,] angiogenic_reg_conc_matrix;
        public double[,,] angio_reg_conc_delta;

        double rate_of_TAF_prod_by_hypo_tum_cells = 100;
        double raye_of_natural_decay_of_TAF = 0.01;

        //VEGF
        public double[,,] vegf_conc_matrix;

        double diffusion_coeficient_of_VEGF = 2.90 * Math.Pow(10, -7);//cmm^2/s
        double const_VEGF = 2 * Math.Pow(10, -2);//nMolar/s
        double arteriole_radius = 3 * 10;//um
        double p_e = 1 * Math.Pow(10, -1);//um/s
        double w_VEGF = 1 * Math.Pow(10, -2);//s^-1
        double delta_S = 2 * 10;
        double delta_t = 1;

        #endregion

        #region PARAMETROS RELACIONADOS CON LA DENSIDAD DE LAS CELULAS ENDOTELIALES

        public double[,,] endo_density_matrix;
        public double[,,] endo_delta_matrix;

        double diffusion_coeficient_endo = 3.5 * Math.Pow(10, -4);
        double chemo_of_EC_sprout_tip = 0.38;
        double decrease_in_chemo_sensitivity = 0.6;
        double hapt_of_EC_sprout_tip = 0.16;

        #endregion


        private int t = 16;
        public int time;
        private int dt;



        public MathematicalModel(Cell[,,] space)
        {
            this.space = space;
            c_n = 0.15 * c_0;
            c_ap = 0.15 * c_0;

            dx = 1.0;
            dy = 1.0;
            dz = 1.0;

            dt = 0;

            oxygen_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            oxygen_discret_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            oxygen_delta = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            density_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            mde = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            mde_delta = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            angiogenic_reg_conc_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            angio_reg_conc_delta = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            endo_density_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            endo_delta_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            vegf_conc_matrix = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];

            consumption_rate = new double[] { 2 * r_c, 5 * r_c, 10 * r_c };

            DiscretizeTheOxygenDiffusionCoefficient();
            DiscretizeOxygenConsumptionRate();

            metabolic_rate = new Dictionary<CellState, double[]>();
            double[] proliferation_rate = { r_c, oxy_consp_rate_disc };
            double[] quiescent_rate = { proliferation_rate[0] / 5, proliferation_rate[1] / 5 };
            double[] migratory_rate = { proliferation_rate[0] / 2, proliferation_rate[1] / 5 };
            double[] necrotic = { 0 };
            metabolic_rate.Add(CellState.TumoralStemCell, proliferation_rate);
            metabolic_rate.Add(CellState.ProliferativeTumoralCell, proliferation_rate);
            metabolic_rate.Add(CellState.QuiescentTumorCell, quiescent_rate);
            metabolic_rate.Add(CellState.MigratoryTumorCell, migratory_rate);
            metabolic_rate.Add(CellState.nothing, necrotic);

            InitializeOxygenConcentrationMatrix();
            InitializeDensityMatrix();
        }
        public double[,,] Laplacian3D(double[,,] u, double dx, double dy, double dz)
        {
            int nx = u.GetLength(0);
            int ny = u.GetLength(1); 
            int nz = u.GetLength(2);
            double[,,] u_laplacian = new double[nx, ny, nz];

            for (int i = 1; i < (nx - 1); i++)
            {
                for (int j = 1; j < (ny - 1); j++)
                {
                    for (int k = 1; k < (nz - 1); k++)
                    {
                        u_laplacian[i, j, k] = (u[i - 1, j, k] - 2 * u[i, j, k] + u[i + 1, j, k]) / (dx * dx) +
                                             (u[i, j - 1, k] - 2 * u[i, j, k] + u[i, j + 1, k]) / (dy * dy) +
                                             (u[i, j, k - 1] - 2 * u[i, j, k] + u[i, j, k + 1]) / (dz * dz);
                    }
                }
            }

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

        private void InitializeOxygenConcentrationMatrix()
        {
            for (int i = 0; i < oxygen_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < oxygen_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < oxygen_matrix.GetLength(2); k++)
                    {
                        //if(k == 0 || k == (oxygen_matrix.GetLength(2) - 1) || j == 0 || j == (oxygen_matrix.GetLength(1) - 1))
                        //{
                        //    oxygen_matrix[i, j, k] = 0.28;
                        //}
                        //else /*if (space[i, j, k].behavior_state != CellState.nothing)*/
                        //{
                        //    oxygen_matrix[i, j, k] = oxygen_discret_matrix[i, j, k] = 1.0;
                        //}
                        CellState behavior = space[i, j, k].behavior_state;
                        if (behavior == CellState.MigratoryTumorCell || behavior == CellState.QuiescentTumorCell || behavior == CellState.ProliferativeTumoralCell || behavior == CellState.TumoralStemCell)
                        {
                            if (TumorNeighborhood(space[i, j, k]))
                                oxygen_matrix[i, j, k] = oxygen_discret_matrix[i, j, k] = oxygen_tumoral_threshold_1;
                            else
                                oxygen_matrix[i, j, k] = oxygen_discret_matrix[i, j, k] = oxygen_tumoral_threshold_2;
                        }
                        else if (behavior == CellState.Astrocyte || behavior == CellState.Neuron || behavior == CellState.StemCell)
                        {
                            if (TumorNeighborhood(space[i, j, k]))
                                oxygen_matrix[i, j, k] = oxygen_discret_matrix[i, j, k] = oxygen_healthy_threshold_1;
                            else
                                oxygen_matrix[i, j, k] = oxygen_discret_matrix[i, j, k] = oxygen_healthy_threshold_2;
                        }
                    }
                }
            }
        }
        
        public bool TumorNeighborhood(Cell cell)
        {
            int tumoral_count = 0;
            int healthy_count = 0;
            foreach (var item in cell.neighborhood)
            {
                CellState behavior = item.behavior_state;
                if (behavior == CellState.MigratoryTumorCell || behavior == CellState.QuiescentTumorCell || behavior == CellState.ProliferativeTumoralCell || behavior == CellState.TumoralStemCell)
                    tumoral_count++;
                else if (behavior == CellState.Astrocyte || behavior == CellState.Neuron || behavior == CellState.StemCell)
                    healthy_count++;
            }
            return tumoral_count > healthy_count;
        }

        private void InitializeDensityMatrix()
        {
            for (int i = 0; i < density_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < density_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < density_matrix.GetLength(2); k++)
                    {
                        density_matrix[i, j, k] = 1.0;
                        //var cell = space[i, j, k];
                        //if (cell.behavior_state == CellState.nothing && cell.loca_state == CellLocationState.MatrixExtracelular)
                        //    density_matrix[i, j, k] = Utils.rdm.NextDouble();
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

        public double OxygenTissuePreExistingBloodVessels(Cell cell)
        {
            int x = cell.pos.X;
            int y = cell.pos.Y;
            int z = cell.pos.Z;
            if (cell is Artery)
                return transf_rate_from_pree_vessels * pree_blood_vessels_density * (1 - oxygen_matrix[x, y, z]);
            return 0;
        }

        public double OxygenPreExistingBloodVessels()
        {
            ///AQUI FALTA TENER UNA LISTA CON LOS VASOS VIEJOS
            double sum = 0;
            foreach (var item in space)
            {
                if (item is Artery)
                    sum += OxygenTissueNeoBloodVessels(item);
            }
            return sum;
        }

        public double OxygenTissueNeoBloodVessels(Cell cell)
        {
            int x = cell.pos.X;
            int y = cell.pos.Y;
            int z = cell.pos.Z;
            if (cell is Artery)
                return transf_rate_from_neo_vessels * neo_blood_vessels_density * HematocritOnNeoNetwork() * (1 - oxygen_matrix[x, y, z]);
            return 0;
        }

        public double OxygenNeoBloodVessels()
        {
            ///AQUI FALTA TENER UNA LISTA CON LOS VASOS NUEVOS
            double sum = 0;
            foreach (var item in space)
            {
                if (item is Artery)
                    sum += OxygenTissueNeoBloodVessels(item);
            }
            return sum;
        }


        public double HematocritOnNeoNetwork()
        {
            return hematocrit_on_neo_network / normal_hematocrit_value - hematocrit_min;
        }

        public double ChemotactiMigration(Pos pos)
        {
            double taf = angiogenic_reg_conc_matrix[pos.X, pos.Y, pos.Z];
            return chemo_of_EC_sprout_tip / (1 + decrease_in_chemo_sensitivity * taf);
        }



        #region Oxygen

        public void UpdateOxygenConcentration2(Cell[,,] space, int time)
        {
            oxygen_delta = Laplacian3D(oxygen_matrix, dx, dy, dz);
            int tumoral_cells = 0;
            double pree_vessels_amount = OxygenPreExistingBloodVessels() / tumoral_cells;
            double neo_vessels_amount = OxygenNeoBloodVessels() / tumoral_cells;

            //TENGO QUE TENER LA CANTIDAD DE CELULAS TUMORALES
            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        CellState cell_state = space[i, j, k].behavior_state;
                        if (cell_state == CellState.ProliferativeTumoralCell || cell_state == CellState.QuiescentTumorCell || cell_state == CellState.MigratoryTumorCell)
                        {

                            double termino = time * (diffussion_coeficient_oxygen * oxygen_delta[i, j, k] * oxygen_matrix[i, j, k] - metabolic_rate[cell_state][0] +
                                            pree_vessels_amount + neo_vessels_amount);
                            double conc = oxygen_matrix[i, j, k] + (termino);
                            oxygen_matrix[i, j, k] += termino;

                        }
                    }
                }
            }
        }

        //ESTE METODO ESTA MAL, DESDE EL INICIO LA CONCENTRACION DE OXYGENO DA MENOR DE 0
        public void UpdateOxygenConcentration(Cell[,,] space, int time)
        {
            this.space = space;
            //DiscretizeTime(time);
            oxygen_delta = Laplacian3D(oxygen_matrix, 0.1, 0.1, 0.1);
            dt = time;
            //dt = t_disc;

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        CellState cell_state = space[i, j, k].behavior_state;
                        //if (cell_state != CellState.ProliferativeTumoralCell && cell_state != CellState.QuiescentTumorCell && cell_state != CellState.MigratoryTumorCell /*&& cell_state != CellState.TumoralStemCell*/)
                        //{
                        //    oxygen_matrix[i, j, k] = boundary_conditions[0];
                        //}
                        if (k == 0 || k == (oxygen_matrix.GetLength(2) - 1) || j == 0 || j == (oxygen_matrix.GetLength(1) - 1))
                        {
                            oxygen_matrix[i, j, k] = 0.28;
                        }
                        else if(cell_state == CellState.ProliferativeTumoralCell || cell_state == CellState.QuiescentTumorCell || cell_state == CellState.MigratoryTumorCell)
                        {

                            //oxygen_matrix[i, j, k] += dt * (difussion_coeficient_oxygen * delta_oxygen[i, j, k] + metabolic_rate[CellState.TumoralCell][0]);

                            //oxygen_matrix[i, j, k] += dt * (difussion_coeficient_oxygen * delta_oxygen[i, j, k] - k * delta_oxygen[i, j, k] * v[i, j, k]);

                            double termino = time * (diffussion_coeficient_oxygen * oxygen_delta[i, j, k] * oxygen_matrix[i, j, k] - metabolic_rate[cell_state][0]);
                            double conc = oxygen_matrix[i, j, k] + (termino);
                            oxygen_matrix[i, j, k] += termino;

                            //DiscretizeOxigenConcentration(oxygen_discret_matrix[i, j, k]);

                            //double t1 = dt * (dif_conc_disc * oxygen_delta[i, j, k] * oxigen_conc_disc - metabolic_rate[cell_state][1]);
                            //double tt = oxygen_matrix[i, j, k] + t1;
                            //oxygen_matrix[i, j, k] += t1;
                            //CellState behavior = space[i, j, k].behavior_state;
                            //if (behavior == CellState.MigratoryTumorCell || behavior == CellState.QuiescentTumorCell || behavior == CellState.ProliferativeTumoralCell || behavior == CellState.TumoralStemCell)
                            //Console.WriteLine("Valor de Laplace: {0} y la concentracion: {1}", oxygen_delta[i, j, k], conc);
                        }
                    }
                }
            }

            //oxygen_matrix = DirichletBoundaryConditions(oxygen_matrix);
        }

        public void UpdateOxygen(Cell[,,] space, int time)
        {
            double[,,] copy_oxygen = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            Array.Copy(oxygen_matrix, copy_oxygen, copy_oxygen.Length);

            oxygen_delta = Laplacian3D(oxygen_matrix, dx, dy, dz);

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        //CONDICIONES DE CONTORNO
                        if (k == 0 || k == (oxygen_matrix.GetLength(2) - 1) || j == 0 || j == (oxygen_matrix.GetLength(1) - 1))
                        {
                            oxygen_matrix[i, j, k] = 0.28;
                        }
                        else
                        {
                            double conc = oxygen_matrix[i, j, k];
                            Cell cell = space[i, j, k];

                            //if (cell.behavior_state == CellState.nothing)
                            //    oxygen_matrix[i, j, k] = 0;
                            //else
                            //{
                            double delta = DeltaVEGFConcentration(cell, conc, copy_oxygen);
                            oxygen_matrix[i, j, k] += delta_t * (diffusion_coeficient_oxygen / Math.Pow(delta_S, 2) * delta - UptakeOxygen(cell)
                                                                    + SourceOxygen(cell, conc));
                            //oxygen_matrix[i, j, k] = Math.Round(oxygen_matrix[i, j, k], 4);

                            double actual = oxygen_matrix[i, j, k];

                            if (cell.behavior_state == CellState.nothing)
                                Console.WriteLine("Oxygen anterior: {0} Oxygen actual: {1}", conc, actual);
                            //CellState behavior = cell.behavior_state;
                            //if (behavior == CellState.MigratoryTumorCell || behavior == CellState.QuiescentTumorCell || behavior == CellState.ProliferativeTumoralCell || behavior == CellState.TumoralStemCell)
                            //    Console.WriteLine("Oxygen Concentration: {0} y El metodo simple: {1}", actual, delta);
                            //}
                        }
                    }
                }
            }
        }

        private double UptakeOxygen(Cell cell)
        {
            CellState behavior_state = cell.behavior_state;
            switch (behavior_state)
            {
                case CellState.ProliferativeTumoralCell:
                    return beta_c;
                case CellState.NecroticTumorCell:
                    return beta_c / 8;
                case CellState.QuiescentTumorCell:
                    return beta_c / 4;
                case CellState.MigratoryTumorCell:
                    return beta_c / 2;
                case CellState.nothing:
                    return 0;
                default:
                    return alfa_c;
            }
        }

        private double SourceOxygen(Cell cell, double conc)
        {
            if (cell is Artery)
                return (2 * Math.PI * arteriole_radius * p_e_oxygen * (c_art_oxygen - conc)) / delta_S;
            return 0;
        }
        #endregion

        #region ECM-MDE
        public void UpdateECMDensity(Cell[,,] space, int time)
        {
            //double rate_of_prod = rate_of_production_of_ECM_by_tumour_cells/tumor.cell_list.Count;
            this.space = space;
            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        CellState cell_state = space[i, j, k].behavior_state;
                        if (cell_state == CellState.ProliferativeTumoralCell || cell_state == CellState.QuiescentTumorCell || cell_state == CellState.MigratoryTumorCell /*|| cell_state == CellState.TumoralStemCell*/)
                        {
                            double d = density_matrix[i, j, k];
                            density_matrix[i, j, k] += time * (-rate_of_degradation_of_ECM_by_MDE * d * mde[i, j, k] + rate_of_production_of_ECM_by_tumour_cells * (1 - d));

                            //Console.WriteLine("Densidad anterior: {0} Densidad en la casilla actual: {1}", d, density_matrix[i, j, k]);
                        }
                    }
                }
            }
        }

        public void UpdateMDE(Cell[,,] space, int time, Tumor tumor)
        {
            this.space = space;
            mde_delta = Laplacian3D(mde, dx, dy, dz);
            double rate_of_prod = (100 * tumor.cell_list.Count) / prod_rate_of_MDE_by_tumour_cells;
            double decay = (100 * tumor.cell_list.Count) / natural_decay_of_MDE;

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        //CONDICIONES DE CONTORNO
                        if (k == 0 || k == (mde.GetLength(2) - 1) || j == 0 || j == (mde.GetLength(1) - 1))
                        {
                            mde[i, j, k] = 0;
                        }
                        else
                        {
                            CellState cell_state = space[i, j, k].behavior_state;
                            if (cell_state == CellState.ProliferativeTumoralCell || cell_state == CellState.QuiescentTumorCell || cell_state == CellState.MigratoryTumorCell)
                            {
                                double value = mde[i, j, k];
                                mde[i, j, k] += time * (diffusion_coefficient_MDE * mde_delta[i, j, k] * value + prod_rate_of_MDE_by_tumour_cells *
                                                        (1 - value) - natural_decay_of_MDE * value);

                                //Console.WriteLine("Valor de Laplace: {0}, rate de prod: {1} y decay: {2}. Valor total actual: {3}", mde_delta[i, j, k], prod_rate_of_MDE_by_tumour_cells, natural_decay_of_MDE, mde[i, j, k]);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateECMDensity2(Cell[,,] space, int time)
        {
            this.space = space;

            for (int i = 0; i < density_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < density_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < density_matrix.GetLength(2); k++)
                    {
                        if (/*i == 0 || i == (u.GetLength(0) - 1) ||*/ k == 0 || k == (density_matrix.GetLength(2) - 1) || j == 0 || j == (density_matrix.GetLength(1) - 1))
                            density_matrix[i, j, k] = 1;
                        else
                        {
                            double d = density_matrix[i, j, k];
                            density_matrix[i, j, k] += time * (-degrad_coeff_ECM * mde[i, j, k] * density_matrix[i, j, k]);

                            //Console.WriteLine("Densidad anterior: {0} Densidad en la casilla actual: {1}", d, density_matrix[i, j, k]);
                        }

                    }
                }
            }
        }

        public void UpdateMDE2(Cell[,,] space, int time, Tumor tumor)
        {

            mde_delta = Laplacian3D(mde, dx, dy, dz);
            for (int i = 0; i < mde.GetLength(0); i++)
            {
                for (int j = 0; j < mde.GetLength(1); j++)
                {
                    for (int k = 0; k < mde.GetLength(2); k++)
                    {
                        if (/*i == 0 || i == (u.GetLength(0) - 1) ||*/ k == 0 || k == (mde.GetLength(2) - 1) || j == 0 || j == (mde.GetLength(1) - 1))
                            mde[i, j, k] = 0;
                        else
                        {
                            CellState behavior = space[i, j, k].behavior_state;
                            double prod = MDEProductionByTumoralCells(behavior);

                            mde[i, j, k] += time * (diffusion_coefficient_MDE_2 * mde_delta[i, j, k] * mde[i, j, k] + prod_rate_of_MDE_by_tumour_cells_2 *
                                            prod - natural_decay_of_MDE_2 * mde[i, j, k]);
                            //if(behavior == CellState.MigratoryTumorCell || behavior == CellState.ProliferativeTumoralCell || behavior == CellState.QuiescentTumorCell)
                            //    Console.WriteLine("Valor de Laplace: {0}. Valor total actual: {1}", mde_delta[i, j, k], mde[i, j, k]);
                        }

                    }
                }
            }
        }

        private double MDEProductionByTumoralCells(CellState behavior)
        {
            switch (behavior)
            {
                case CellState.ProliferativeTumoralCell:
                    return prod_rate_of_MDE_by_tumour_cells_2;
                case CellState.NecroticTumorCell:
                    return prod_rate_of_MDE_by_tumour_cells_2/10;
                case CellState.QuiescentTumorCell:
                    return prod_rate_of_MDE_by_tumour_cells_2 / 5;
                case CellState.MigratoryTumorCell:
                    return 2 * prod_rate_of_MDE_by_tumour_cells_2;
                default:
                    return 0;
            }
        }
        #endregion



        public double[,,] NeumannBoundaryConditions(double[,,] mde_matrix)
        {
            for (int i = 0; i < mde_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < mde_matrix.GetLength(1); j++)
                {
                    mde_matrix[i, j, 0] = 0;
                    mde_matrix[i, j, mde_matrix.GetLength(2) - 1] = 0;
                    mde_matrix[i, 0, j] = 0;
                    mde_matrix[i, mde_matrix.GetLength(1) - 1, j] = 0;
                    //oxygen_matrix[0, i, j] = c_0;
                    //oxygen_matrix[oxygen_matrix.GetLength(0) - 1, i, j] = c_0;
                }
            }
            return mde_matrix;
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
                        CellState cell_state = space[i, j, k].behavior_state;
                        if (cell_state == CellState.QuiescentTumorCell)
                        {
                            double c = angiogenic_reg_conc_matrix[i, j, k];
                            angiogenic_reg_conc_matrix[i, j, k] += time * (diffusion_coeficient_angio * angio_reg_conc_delta[i, j, k] + rate_of_TAF_prod_by_hypo_tum_cells *
                                                                       (1 - c) - raye_of_natural_decay_of_TAF * c);
                        }

                        //angiogenic_reg_conc_matrix[i,j,k] += time * (diffusion_coeficient_angio * angio_reg_conc_delta[i, j, k] - natural_disintegration_rate * c +
                        //                                Sc * hypoxic_cell_volume_fraction * (c_sat - c));
                    }
                }
            }
        }

        #region VEGF
        public void VEGF(Cell[,,] space, int time)
        {
            //falta poner laplacien 
            double[,,] copy_vegf = new double[space.GetLength(0), space.GetLength(1), space.GetLength(2)];
            Array.Copy(vegf_conc_matrix, copy_vegf, copy_vegf.Length);

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        double conc = vegf_conc_matrix[i, j, k];
                        Cell cell = space[i, j, k];
                        double delta = DeltaVEGFConcentration(cell, conc, copy_vegf);
                        double uptake = Uptake(cell, conc);
                        double source = Source(cell);
                        double waste = Waste(conc);
                        vegf_conc_matrix[i, j, k] += diffusion_coeficient_of_VEGF * 0.05 * delta - 0.05 * uptake + source - waste;
                        //vegf_conc_matrix[i, j, k] += delta_t * (diffusion_coeficient_of_VEGF / Math.Pow(delta_S, 2) * DeltaVEGFConcentration(cell, conc, copy_vegf) - Uptake(cell, conc)
                        //                                        + Source(cell) - Waste(conc));

                        //if(vegf_conc_matrix[i, j, k] != 0)
                        //    Console.WriteLine("VEGF anterior: {0} VEGF act: {1} Delta:{2} Uptake: {3} Source: {4} Waste: {5}", conc, vegf_conc_matrix[i, j, k], delta, uptake, source, waste);

                        //Console.WriteLine("Behavior: {0} VEGF: {1}",cell.behavior_state, vegf_conc_matrix[i,j,k]);
                    }
                }
            }
        }

        private double DeltaVEGFConcentration(Cell cell, double conc, double[,,] concentracion)
        {
            List<double> neighbors_sum = new List<double>();
            foreach (var item in cell.neighborhood)
                neighbors_sum.Add(concentracion[item.pos.X, item.pos.Y, item.pos.Z]);

            return neighbors_sum.Sum() - 26 * conc;
        }

        private double Uptake(Cell cell, double vegf_conc)
        {
            //double radius = 3 * 10;
            if (cell is Artery)
                return (2 * Math.PI * arteriole_radius * p_e * vegf_conc);
            return 0;
        }

        private double Source(Cell cell)
        {
            CellState behavior_state = cell.behavior_state;
            switch (behavior_state)
            {
                case CellState.ProliferativeTumoralCell:
                    return const_VEGF / 8;
                case CellState.NecroticTumorCell:
                    return const_VEGF;
                case CellState.QuiescentTumorCell:
                    return const_VEGF / 4;
                case CellState.MigratoryTumorCell:
                    return const_VEGF / 6;
                default:
                    return 0;
            }
        }

        private double Waste(double vegf_conc)
        {
            return w_VEGF * vegf_conc;
        }
        #endregion



        public void UpdateEndothelialDensity(Cell[,,] space, int time)
        {
            this.space = space;

            endo_delta_matrix = Laplacian3D(endo_density_matrix, dx, dy, dz);

            for (int i = 0; i < space.GetLength(0); i++)
            {
                for (int j = 0; j < space.GetLength(1); j++)
                {
                    for (int k = 0; k < space.GetLength(2); k++)
                    {
                        
                        if(space[i,j,k] is Artery || space[i,j,k] is NeoBloodVessels)
                        {
                            Cell cell = space[i, j, k];
                            double d = endo_density_matrix[i, j, k];
                            endo_density_matrix[i, j, k] += time * (diffusion_coeficient_endo * endo_delta_matrix[i, j, k] * endo_density_matrix[i, j, k] - ChemotactiMigration(cell.pos) * d *
                                                            angio_reg_conc_delta[i, j, k] - mde_delta[i, j, k] * hapt_of_EC_sprout_tip * d);
                        }
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
