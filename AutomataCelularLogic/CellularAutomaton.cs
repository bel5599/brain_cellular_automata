using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using System.Diagnostics;

namespace AutomataCelularLogic
{ 
    public class CellularAutomaton
    {
        #region PARAMETROS PARA GRAFICAR RESULTADOS

        double[] time;
        double[] cell_count;

        #endregion

        #region Variables relacionadas directamente con el automata 
        public Probability move_prob;
        public Cell[,,] space;
        public Tumor tumor;
        public Cell tumor_stem_cell;
        public MathematicalModel model;

        public Normal distribution;
        private int proliferation_age = 16;

        public Cell[] random_space;
        #endregion

        #region Informacion de la cantidad de celulas

        public static int stem_cells_count = 100;
        public static int astrocytes_count = 50;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 50;


        #endregion

        #region Estructuras relacionados con los fenotipos de celulas

        //public List<Cell> astrocyte_list;
        public Dictionary<Pos, Cell> astrocyte_list;

        //public List<Cell> neuron_list;
        public Dictionary<Pos, Cell> neuron_list;
        //public List<Cell> stem_cell_list;
        public Dictionary<Pos, Cell> stem_cell_list;


        //hay que cambiar a private
        public  Dictionary<Cell, int[]> proliferation_cells;
        private Dictionary<Pos,Cell> new_tumoral_cells;

        //private List<Cell> new_quiescent_cells;
        Dictionary<Pos, Cell> new_quiescent_cells;

        //public  List<Cell> quiescent_cell_list;
        public Dictionary<Pos, Cell> quiescent_cell_list;

        public List<Cell> necrotic_cell_list;

        //estructuras
        public Dictionary<Pos, Cell> pos_cell_dict;

        public List<Cell> empty_cells;
        //public Dictionary<Pos, Cell> empty_cells;

        public Dictionary<Pos, Pos> next_old_stem_position;
        public Dictionary<Pos, Pos> next_migratory_position;

        #endregion

        List<Cell> new_proliferative_cells;

        #region BloodVessels

        public Dictionary<Pos, BloodVessel> pos_vessel_dict;

        public List<Cell> closer_cells_to_vessels;

        public List<Cell> vasos_cooptados;

        //public List<Cell> new_artery_list;

        public List<EdgeTree> edges;


        Dictionary<Cell, Cell> vasos_en_crecimiento_dict;
        public List<Tuple<Cell, Cell>> vasos_en_crecimiento;

        //public List<Cell> posible_migratory_cells;
        //public List<Cell> migratory_cells;

        //public List<Cell> migratory_cells_actual;
        //public List<Cell> next_migratory_cells;

        public Dictionary<Pos, Cell> next_migratory_cells;
        public Dictionary<Pos, Cell> migratory_cells_actual;


        public Dictionary<Pos, BloodVessel> pos_neo_vessel_dict;
        public List<Cell> final_tip_of_vessel_list;

        //variables que estoy utilizando ahora
        private List<Cell> next_blood_vessels;
        private List<Cell> blood_vessels_actual;

        public List<Cell> neo_blood_vessels;

        private Dictionary<Cell, Tuple<Cell,int>> first_vessel_close_tumoral_cell_dict;

        #endregion

        #region Migration

        double const_mutation = 10;
        double mutation_radiuos;
        double estimated_mutation_time = 10;

        double prob_migra = 0.2;

        #endregion

        #region PARAMETROS RELACIONADOS CON EL FLUJO DE SANGRE EN LOS VASOS SANGUINEOS
        public List<BloodVesselSegment> vessel_segment_list;
        List<BloodVesselSegment> immature_vessel;
        List<BloodVesselSegment> wss_segment_list;

        //este es para agregar en el metodo que tiene comentarios
        List<BloodVesselSegment> new_vessel_segment_list;
        public List<BloodVesselSegment> neo_vessel_segment_list;

        List<BloodVessel> neo_blood_vessel_list;

        public Dictionary<BloodVessel, List<Cell>> growing_vessels;

        Dictionary<BloodVesselSegment, BloodVessel> segment_neo_vessel_dict;

        Dictionary<BloodVessel, BloodVesselSegment> vessel_segment_dict;

        //List<Cell> new_endothelial_cells;
        Dictionary<Pos, Cell> new_endothelial_cells;

        public Dictionary<Pos,Cell> endothelial_cells;

        Dictionary<Pos, Children> pos_children_dict = new Dictionary<Pos, Children>();

        List<Cell> new_vessel_cells;


        #endregion



        

        //Dictionary<Pos, Cell> proliferation_cells;


        //Dictionary<Pos, Cell> neuron_list;



        public double tumoral_angiogenic_factor;

        public CellularAutomaton(int x, int y, int z, Probability move_prob, int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            //space = new Cell[x, y, z];
            random_space = new Cell[x * y * z];

            tumoral_angiogenic_factor = 0;

            this.move_prob = move_prob;
            InicializarListas();

            //StartCellularLifeInTheBrain();
            World world = new World(x, y, z, 1, stem_cells_count, astrocytes_count, neuron_count);
            edges = world.edges;
            pos_cell_dict = world.pos_cell_dict;
            pos_vessel_dict = world.pos_artery_dict;
            pos_children_dict = world.pos_children;

            astrocyte_list = world.astrocyte_list;
            neuron_list = world.neuron_list;
            stem_cell_list = world.stem_cell_list;

            space = world.world;
            tumor_stem_cell = world.tumor_stem_cell;

            CreateTumor(avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);

            UpdateNeighborhood();
            AleatorizarCelulas();

            model = new MathematicalModel(space, world.edge_order_dict.Count);
            

            BuildTheVesselSegments(world);
        }

        void InicializarListas()
        {
            distribution = new Normal(proliferation_age, proliferation_age / 2);
            //ESTO VA EN LA PARTE DE LA CELULA PARA LA PROLIFERACION

            new_tumoral_cells = new Dictionary<Pos, Cell>();

            empty_cells = new List<Cell>();

            next_old_stem_position = new Dictionary<Pos, Pos>();
            pos_cell_dict = new Dictionary<Pos, Cell>();

            //pos_vessel_dict = new Dictionary<Pos, Cell>();

            proliferation_cells = new Dictionary<Cell, int[]>();

            new_quiescent_cells = new Dictionary<Pos, Cell>();
            quiescent_cell_list = new Dictionary<Pos, Cell>();

            pos_vessel_dict = new Dictionary<Pos, BloodVessel>();

            vasos_en_crecimiento = new List<Tuple<Cell, Cell>>();
            vasos_en_crecimiento_dict = new Dictionary<Cell, Cell>();

            //migratory_cells = new List<Cell>();
            //posible_migratory_cells = new List<Cell>();
            next_migratory_position = new Dictionary<Pos, Pos>();

            vasos_cooptados = new List<Cell>();
            //new_artery_list = new List<Cell>();

            closer_cells_to_vessels = new List<Cell>();

            neo_blood_vessels = new List<Cell>();
            pos_neo_vessel_dict = new Dictionary<Pos, BloodVessel>();
            final_tip_of_vessel_list = new List<Cell>();

            next_migratory_cells = new Dictionary<Pos, Cell>();
            migratory_cells_actual = new Dictionary<Pos, Cell>();

            next_blood_vessels = new List<Cell>();
            blood_vessels_actual = new List<Cell>();
            first_vessel_close_tumoral_cell_dict = new Dictionary<Cell, Tuple<Cell, int>>();

            new_proliferative_cells = new List<Cell>();

            vessel_segment_list = new List<BloodVesselSegment>();
            immature_vessel = new List<BloodVesselSegment>();

            new_vessel_segment_list = new List<BloodVesselSegment>();
            neo_vessel_segment_list = new List<BloodVesselSegment>();

            growing_vessels = new Dictionary<BloodVessel, List<Cell>>();
            new_vessel_cells = new List<Cell>();
            new_endothelial_cells = new Dictionary<Pos, Cell>();
            neo_blood_vessel_list = new List<BloodVessel>();
            segment_neo_vessel_dict = new Dictionary<BloodVesselSegment, BloodVessel>();

            necrotic_cell_list = new List<Cell>();
            endothelial_cells = new Dictionary<Pos, Cell>();

            vessel_segment_dict = new Dictionary<BloodVessel, BloodVesselSegment>();

            time = new double[500];
            cell_count = new double[500];
        }

        public void BuildTheVesselSegments(World world)
        {
            
            foreach (var item in world.edge_order_dict)
            {
                BloodVessel vessel1 = pos_vessel_dict[item.Key.Item1];
                BloodVessel vessel2 = pos_vessel_dict[item.Key.Item2];

                //int distance = Utils.EuclideanDistance(vessel1.pos, vessel2.pos);
                //if (distance != 5)
                //    Console.WriteLine("Pos1: {0} {1} {2} Pos2: {3} {4} {5} {6}",vessel1.pos.X, vessel1.pos.Y, vessel1.pos.Z, vessel2.pos.X, vessel2.pos.Y, vessel2.pos.Z, distance);

                Tuple<double, double, double> diameter_length_pc = model.strahler_order_dict[item.Value];
                double diameter = diameter_length_pc.Item1;
                double length = diameter_length_pc.Item2;
                double pc = diameter_length_pc.Item3;
                BloodVesselSegment segment = new BloodVesselSegment(vessel1, vessel2, item.Value, diameter, length, pc);
                //vessel_segment_dict.Add(vessel1, segment);
                //vessel_segment_dict.Add(vessel2, segment);

                vessel_segment_list.Add(segment);
            }
        }

        public void AleatorizarCelulas()
        {
            int i = 0;
            int count = random_space.Length;
            Dictionary<Cell, int> dict = new Dictionary<Cell, int>();
            while(count > 0)
            {
                int x = Utils.rdm.Next(0, space.GetLength(0));
                int y = Utils.rdm.Next(0, space.GetLength(1));
                int z = Utils.rdm.Next(0, space.GetLength(2));
                if(!dict.ContainsKey(space[x,y,z]))
                {
                    random_space[i] = space[x, y, z];
                    dict.Add(random_space[i], i);
                    i++;
                    count--;
                }
            }
        }

        public void UpdateNeighborhood()
        {
            foreach (var cell in space)
            {
                cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);

                if(cell.behavior_state != CellState.nothing)
                    pos_cell_dict.Add(cell.pos, cell);
                //if (cell.behavior_state == CellState.StemCell)
                //    next_old_stem_position.Add(cell.pos, Utils.NullPos());

                //if(cell.behavior_state == CellState.nothing)
                //{
                //    Console.WriteLine("{0} {1} {2} vecindad: {3}", cell.pos.X, cell.pos.Y, cell.pos.Z, cell.neighborhood.Count);
                //}

                //if(cell is Artery && pos_artery_dict.ContainsValue((Artery)cell))
                //{
                //    if(cell.neighborhood == null || cell.neighborhood.Count == 0)
                //        Console.WriteLine("Soy null");
                //}
            }
        }

        //OJO CAMBIAR NOMBREEEEEE

        #region Creation
        public void CreateTumor(int avascular_carrying_capacity, int vascular_carrying_capacity, double growth_rate, int initial_population)
        {
            tumor = new Tumor(tumor_stem_cell, avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);
        }

        #endregion

        
        public void SearchCloserStemPosition()
        {
            foreach (var key_value in stem_cell_list)
            {
                Pos pos = key_value.Key;

                Pos closer_pos = Utils.MinDistancePos(pos_cell_dict[pos].neighborhood, next_old_stem_position, tumor_stem_cell.pos); ;
                int distance = Utils.EuclideanDistance(tumor_stem_cell.pos, closer_pos);

                if (space[closer_pos.X, closer_pos.Y, closer_pos.Z].behavior_state == CellState.nothing && space[closer_pos.X, closer_pos.Y, closer_pos.Z].loca_state == CellLocationState.MatrixExtracelular)
                {
                    if (distance >= 5)
                        next_old_stem_position.Add(closer_pos, pos);
                    else if (distance != 0 && Utils.rdm.Next(0, 2) == 1)
                        next_old_stem_position.Add(closer_pos, pos);

                }
            }
        }

        //public void SearchCloserStemPosition()
        //{
        //    Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
        //    foreach (var key_value in next_stem_position)
        //    {
        //        Pos pos = key_value.Key;
        //        //Cell cell = pos_cell_dict[pos];
        //        //List<Cell> neig = cell.neighborhood;

        //        Pos closer_pos = Utils.MinDistancePos(pos_cell_dict[pos].neighborhood, tumor_stem_cell.pos);
        //        int distance = Utils.EuclideanDistance(tumor_stem_cell.pos, closer_pos);

        //        if (space[closer_pos.X, closer_pos.Y, closer_pos.Z].behavior_state == CellState.nothing && space[closer_pos.X, closer_pos.Y, closer_pos.Z].loca_state == CellLocationState.MatrixExtracelular)
        //        {
        //            if (distance >= 5)
        //                temp.Add(key_value.Key, closer_pos);
        //            else if (distance != 0 && Utils.rdm.Next(0, 2) == 1)
        //                temp.Add(key_value.Key, closer_pos);

        //        }
        //    }
        //    next_stem_position = temp;
        //}

        public void ClearCloserCellsToVessels()
        {
            closer_cells_to_vessels = new List<Cell>();
        }

        #region FLUJO DE SANGRE A TRAVES DE LOS VASOS SANGUINEOS
        public void UpdateBloodFlow()
        {
            foreach (var item in vessel_segment_list)
            {
                item.hydraulic_permeability = model.CalculatePermeabilityOfTheVesselWall(item);
                item.interstitial_pressure = model.CalculateInterstitialPressure(item, item.hydraulic_permeability);
                item.pressure_collapse = model.CalculateCollapsePressure(item, item.hydraulic_permeability);
                item.radius = model.CalculateRadius(item, item.interstitial_pressure, item.pressure_collapse);

                 //= radius;
            }
            foreach (var item in neo_vessel_segment_list)
            {
                item.hydraulic_permeability = model.CalculatePermeabilityOfTheVesselWall(item);
                item.interstitial_pressure = model.CalculateInterstitialPressure(item, item.hydraulic_permeability);
                item.pressure_collapse = model.CalculateCollapsePressure(item, item.hydraulic_permeability);
                item.radius = model.CalculateRadius(item, item.interstitial_pressure, item.pressure_collapse);

                //= radius;
            }
            //UpdateLp();
            //UpdatePi();
            //UpdatePc();
            //UpdateRadius();
        }

        public void VEGFEnLosVasos()
        {
            foreach (var item in vessel_segment_list)
            {
                if (item.radio_clasf == RadioClassification.MatureVessel)
                    VEGFEnLosVasos(item);
            }

            foreach (var item in neo_vessel_segment_list)
            {
                if (item.radio_clasf == RadioClassification.MatureVessel)
                    VEGFEnLosVasos(item);
            }
        }

        public void VEGFEnLosVasos(BloodVesselSegment segment)
        {
            List<double> vegf = new List<double>();
            vegf = NutrientConcentrationValues(segment.blood_vessel1.neighborhood, model.vegf_conc_matrix);
            vegf.AddRange(NutrientConcentrationValues(segment.blood_vessel2.neighborhood, model.vegf_conc_matrix));

            foreach (var item in vegf)
            {
                if (item >= 0.1E-95 /*Utils.rdm.Next(0,2)==1*/)
                {
                    immature_vessel.Add(segment);
                    segment.radio_clasf = RadioClassification.ImmatureVessel;
                    return;
                }
            }

        }

        public void UpdateImmatureVesselSegmentRadius()
        {
            foreach (var item in immature_vessel)
            {
                if(item.radius < 10)
                    item.radius += 0.40;
            }
        }

        public void WSS()
        {
            foreach (var item in immature_vessel)
            {
                double wss = model.CalculateWSS(item);
                if (wss < model.wss)
                    wss_segment_list.Add(item);
            }
        }

        #endregion

        public void EndothelialCellMigration()
        {
            foreach (var item in immature_vessel)
            {
                if(item.order == StrahlerOrder.StrahlerOrder_1)
                {
                    if ((item.blood_vessel1.maturation_time >= 18 && !growing_vessels.ContainsKey(item.blood_vessel1)) || (item.blood_vessel2.maturation_time >= 18 && !growing_vessels.ContainsKey(item.blood_vessel2)))
                    {
                        if (pos_children_dict[item.blood_vessel1.pos].child_left == null && pos_children_dict[item.blood_vessel1.pos].child_left == null)
                            GetPositionForEndothelialCellMigration(item.blood_vessel1);
                        else
                            GetPositionForEndothelialCellMigration(item.blood_vessel2);
                    }
                }
            }

            //foreach (var item in segment_neo_vessel_dict)
            //{
            //    if(item.Value.maturation_time < 18 && Utils.rdm.Next(0,2) == 1)
            //        GetPositionForEndothelialCellMigration(item.Value);
            //}

            //PORQUE REINICIO ESTO?
           // segment_neo_vessel_dict = new Dictionary<BloodVesselSegment, NeoBloodVessel>();
        }

        public void GetPositionForEndothelialCellMigration(BloodVessel vessel)
        {
            List<Cell> empty_cells = Utils.EmptyPositions(vessel.neighborhood);
            List<double> tnf_list = NutrientConcentrationValues(empty_cells, model.vegf_conc_matrix);

            List<Cell> selection_cells = new List<Cell>();

            for (int i = 0; i < tnf_list.Count; i++)
            {
                if (tnf_list[i] >= 0.1E-95 /*Utils.rdm.Next(0,2)==1*/)
                    selection_cells.Add(empty_cells[i]);
            }

            if (selection_cells.Count > 0)
            {
                if(selection_cells.Count == 1 && !new_endothelial_cells.ContainsKey(selection_cells[0].pos) && !growing_vessels.ContainsKey(vessel))
                {
                    growing_vessels.Add(vessel, new List<Cell>() { selection_cells[0]});
                    new_endothelial_cells.Add(selection_cells[0].pos, selection_cells[0]);

                    Console.WriteLine("Endotelial Pos: {0} {1} {2}", selection_cells[0].pos.X, selection_cells[0].pos.Y, selection_cells[0].pos.Z);
                }
                else if(selection_cells.Count > 1 && !new_endothelial_cells.ContainsKey(selection_cells[0].pos) && !new_endothelial_cells.ContainsKey(selection_cells[1].pos) && !growing_vessels.ContainsKey(vessel))
                {
                    growing_vessels.Add(vessel, new List<Cell>() { selection_cells[0], selection_cells[1] });
                    new_endothelial_cells.Add(selection_cells[0].pos, selection_cells[0]);
                    new_endothelial_cells.Add(selection_cells[1].pos, selection_cells[1]);

                    Console.WriteLine("Endotelial Pos: {0} {1} {2}", selection_cells[0].pos.X, selection_cells[0].pos.Y, selection_cells[0].pos.Z);
                    Console.WriteLine("Endotelial Pos: {0} {1} {2}", selection_cells[1].pos.X, selection_cells[1].pos.Y, selection_cells[1].pos.Z);
                }
            }

            //agregar aqui los nuevos posibles vasos


        }

        public void VesselGrowth()
        {
            //SE REALIZA LA CREACION SI EXISTE LA CELULA ENDOTELIAL EN ESA POSICION
            foreach (var item in growing_vessels)
            {
                //si se tiene la edad de maduracion y se cumple una variable aleatoria en 1 entonces
                if (item.Key.maturation_time >= 18 && Utils.rdm.Next(0, 2) == 1)
                {
                    if (item.Value.Count == 2 && endothelial_cells.ContainsKey(item.Value[0].pos) && endothelial_cells.ContainsKey(item.Value[1].pos))
                    {
                        new_vessel_cells.Add(item.Value[0]);
                        new_vessel_cells.Add(item.Value[1]);

                        item.Key.maturation_time = 0;
                    }
                    else if (endothelial_cells.ContainsKey(item.Value[0].pos))
                    {
                        new_vessel_cells.Add(item.Value[0]);
                        item.Key.maturation_time = 0;
                    }
                    
                    
                }

            }
        }

        public void UpdateNewNeoVesselsSegment()
        {
            List<BloodVessel> remove_vessel = new List<BloodVessel>();
            foreach (var item in growing_vessels)
            {
                if (item.Value.Count == 2 && pos_neo_vessel_dict.ContainsKey(item.Value[0].pos) && pos_neo_vessel_dict.ContainsKey(item.Value[1].pos))
                {
                    //aqui los bordes son los que estan en la lista del value
                    var vessel1 = /*(BloodVessel)*/item.Value[0] as BloodVessel;
                    var vessel2 = /*(BloodVessel)*/item.Value[1] as BloodVessel;

                    if (vessel1 is BloodVessel)
                    {
                        CreateNeoSegment(vessel1, item.Key);
                        //NO ENTIENDO PORQUE DOS LISTAS
                    }
                    if (vessel2 is BloodVessel)
                    {
                        CreateNeoSegment(vessel2, item.Key);
                    }
                    remove_vessel.Add(item.Key);

                    foreach (var segment in vessel_segment_list)
                    {
                        if(segment.blood_vessel1 == item.Key || segment.blood_vessel2 == item.Key)
                            segment.order = StrahlerOrder.StrahlerOrder_2;
                    }
                    
                    //vessel_segment_dict[item.Key].order = StrahlerOrder.StrahlerOrder_2;
                }
                else if (item.Value.Count == 1 && pos_neo_vessel_dict.ContainsKey(item.Value[0].pos))
                {
                    var vessel1 = item.Value[0] as BloodVessel;
                    if (vessel1 is BloodVessel)
                    {
                        CreateNeoSegment(vessel1, item.Key);
                        //NO ENTIENDO PORQUE DOS LISTAS
                    }
                    remove_vessel.Add(item.Key);

                    foreach (var segment in vessel_segment_list)
                    {
                        if (segment.blood_vessel1 == item.Key || segment.blood_vessel2 == item.Key)
                            segment.order = StrahlerOrder.StrahlerOrder_2;
                    }

                    //vessel_segment_dict[item.Key].order = StrahlerOrder.StrahlerOrder_2;
                }
            }

            foreach (var item in remove_vessel)
            {
                growing_vessels.Remove(item);
            }
        }

        private void CreateNeoSegment(BloodVessel neovessel, BloodVessel vessel)
        {
            var diameter_length_pc = model.strahler_order_dict[StrahlerOrder.StrahlerOrder_1];
            BloodVesselSegment segment = new BloodVesselSegment(vessel, neovessel, StrahlerOrder.StrahlerOrder_1, diameter_length_pc.Item1, diameter_length_pc.Item2, diameter_length_pc.Item3);
            segment_neo_vessel_dict.Add(segment, neovessel);
            neo_vessel_segment_list.Add(segment);
        }

        public void UpdateMaturationTime()
        {
            //foreach (var item in neo_blood_vessel_list)
            //{
            //    item.maturation_time += 1;
            //}
            foreach (var item in immature_vessel)
            {
                if(item.order == StrahlerOrder.StrahlerOrder_1)
                {
                    item.blood_vessel1.maturation_time += 1;
                    item.blood_vessel2.maturation_time += 1;
                }
            }
        }


        #region ObtenerPosicionesCercanaALosVasosParaContaminar
        public void ObtainCellsPositionsToDivide()
        {
            foreach (var tumor_cell in tumor.cell_list)
            {
                Tuple<Cell, Cell> closer_cell_to_artery = ObtenerVecinoCercanoAVasos(tumor_cell.Value.neighborhood);
                if (closer_cell_to_artery != null)
                {
                    closer_cells_to_vessels.Add(closer_cell_to_artery.Item1);

                    empty_cells.Add(closer_cell_to_artery.Item1);
                }
            }
        }

        //ARREGLAR AQUI PARA TENER EN CUENTA LAS ARTERIOLAS TAMBIEN
        public Tuple<Cell, Cell> ObtenerVecinoCercanoAVasos(List<Cell> cell_list)
        {
            int min = int.MaxValue;
            Tuple<Cell, Cell> closer_cell_to_artery = null;

            foreach (var item in cell_list)
            {
                if (item.behavior_state == CellState.nothing)
                {
                    int distance_min = int.MaxValue;
                    Cell artery = null;
                    foreach (var item2 in pos_vessel_dict)
                    {
                        if (!vasos_cooptados.Contains(item2.Value))
                        {
                            int distance = Utils.EuclideanDistance(item.pos, item2.Key);
                            if (distance < distance_min)
                            {
                                distance_min = distance;
                                artery = pos_vessel_dict[item2.Key];
                            }
                        }
                    }
                    if (min >= distance_min)
                    {
                        min = distance_min;
                        closer_cell_to_artery = new Tuple<Cell, Cell>(item, artery);
                    }
                }
            }
            //Console.WriteLine("{0} {1} {2}", closer_cell_to_artery.Item1.pos.X, closer_cell_to_artery.Item1.pos.Y, closer_cell_to_artery.Item1.pos.Z);
            //Console.WriteLine("{0} {1} {2}", closer_cell_to_artery.Item2.pos.X, closer_cell_to_artery.Item2.pos.Y, closer_cell_to_artery.Item2.pos.Z);
            return closer_cell_to_artery;
        }
        #endregion

        #region MigratoryCells
        //buscar posiciones alrededor del tumor y hallar la probabilidad para convertilas en migratorias

        
        //ESTO SE PUEDE MEJORAR
        private void CellMutation()
        {
            List<Cell> migra_list = new List<Cell>();
            //int total_amount = (int)((proliferation_cells.Count * 1.0)/100);

            foreach (var item in proliferation_cells.Keys)
            {
                if(item.behavior_state == CellState.ProliferativeTumoralCell)
                {
                    if(Utils.rdm.NextDouble() <= prob_migra /*&& (migra_list.Count < total_amount)*/ && (Utils.EmptyPositions(item.neighborhood).Count > 5))
                    {
                        migra_list.Add(item);
                        
                    }
                }
            }
            for (int i = 0; i < migra_list.Count; i++)
            {
                next_migratory_cells.Add(migra_list[i].pos, migra_list[i]);
            }
            //next_migratory_cells.AddRange(migra_list);
        }

        #region Codigo que no he utilizado
        private void CellMutation(Cell proli, double r)
        {
            List<Cell> cellsWithinRadius = PositionsAvailableForMutation(proli, r);

            Cell posible_migra = cellsWithinRadius[Utils.rdm.Next(0, cellsWithinRadius.Count)];

            double d = MutationTransferFunction(posible_migra.pos, proli.pos, tumor.time, tumor.time, r, estimated_mutation_time * tumor.time);
        }

        private List<Cell> PositionsAvailableForMutation(Cell proli, double r)
        {
            List<Cell> cellsWithinRadius = new List<Cell>();

            foreach (Cell cell in space)
            {
                if (Utils.EuclideanDistance(cell.pos, proli.pos) <= r)
                {
                    Pos pos = cell.pos;
                    if (model.density_matrix[pos.X, pos.Y, pos.Z] > 0.5 && model.oxygen_discret_matrix[pos.X, pos.Y, pos.Z] < 0.6)
                        cellsWithinRadius.Add(cell);
                }
            }

            return cellsWithinRadius;
        }

        public double MutationTransferFunction(Pos x, Pos xPrime, double t, double tPrime, double rPrime, double ktPrime)
        {
            double g = GaussianDistribution(x, xPrime, rPrime);
            double a = TemporalActivationFunction(t, tPrime, ktPrime, TimeValueMutation(t, tPrime, ktPrime));
            return g * a;
        }

        public double GaussianDistribution(Pos x, Pos xPrime, double rPrime)
        {
            return const_mutation * Math.Exp(-Utils.EuclideanDistance(x, xPrime) / Math.Pow(rPrime, 2));
        }

        public double TemporalActivationFunction(double t, double tPrime, double ktPrime, double b)
        {
            return 4 * b / (1 - b);
        }

        private double TimeValueMutation(double t, double tPrime, double ktPrime)
        {
            return (t - tPrime) / ktPrime;
        }

        public bool ShouldMutate(double mutationRate)
        {
            double randomNumber = Utils.rdm.NextDouble();
            return randomNumber < mutationRate;
        }
        #endregion




        public void UpdateMigratoryCells()
        {
            if (next_migratory_cells.Count == 0 && migratory_cells_actual.Count == 0)
                CellMutation();
            //GetMigratoryCells();

            else
            {
                NextMove();
                CellMutation();
            }
        }
        public void GetMigratoryCells()
        {
            foreach (var item in tumor.cell_list)
            {
                //if (Utils.rdm.NextDouble() < move_prob.MigrateProbability(item, tumor))
                //    posible_migratory_cells.Add(item);
            }
        }

        public void NextMove()
        {
            //Dictionary<Pos, Pos> temp = new Dictionary<Pos, Pos>();
            //foreach (var item in next_migratory_position)
            //{
            //    //aqui se quiere obtener las casillas vacias vecinas
            //    //List<Cell> free_neighbord = Utils.EmptyPositions(pos_cell_dict[item.Key].neighborhood);
            //    //if (free_neighbord.Count > 0)
            //    //{
            //    //    temp.Add(item.Key, free_neighbord[Utils.rdm.Next(0, free_neighbord.Count)].pos);
            //    //}

            //    temp.Add(item.Key, SelectionOfCellsToMigrate(item.Key));
            //}
            //next_migratory_position = temp;

            foreach (var item in migratory_cells_actual)
            {
                if (Utils.EmptyPositions(item.Value.neighborhood).Count == 0)
                {
                    Console.WriteLine("En realidad no tengo ninguna casilla vacia");
                    new_quiescent_cells.Add(item.Key, item.Value);
                }
                else
                {

                    Cell cell = SelectionOfCellsToMigrate(item.Value, model.oxygen_matrix);
                    if (cell != null && !next_migratory_cells.ContainsKey(cell.pos))
                        next_migratory_cells.Add(cell.pos, cell);
                }
            }
            
        }

        private Cell SelectionOfCellsToMigrate(Cell migration_cell, double[,,] nutrien_conc)
        {
            List<double> nutrient_conc;
            List<Cell> empty_cells = Utils.EmptyPositions(migration_cell.neighborhood);
            

            List<Cell> selection_cells = new List<Cell>();

            foreach (var item in empty_cells)
            {
                nutrient_conc = NutrientConcentrationValues(Utils.EmptyPositions(item.neighborhood), nutrien_conc);
                if (nutrient_conc.Count > 0 && (nutrient_conc.Max() - nutrient_conc.Average()) > CalculateStandardDeviation(nutrient_conc))
                    selection_cells.Add(item);
            }

            if (selection_cells.Count == 1)
                return selection_cells[0];
            else
            {
                if(selection_cells.Count == 0)
                    Console.WriteLine("Estoy vacio");
                return ProbabilityOfSelection(empty_cells, nutrien_conc);
            }

        }

        private Cell ProbabilityOfSelection(List<Cell> selection, double[,,] nutrient_conc)
        {
            double prob = double.MinValue;
            Cell cell = null;
            double[] conc = new double[selection.Count];

            for (int i = 0; i < selection.Count; i++)
                conc[i] = AverageNutrientConcentration(selection[i], nutrient_conc);

            for (int i = 0; i < conc.Length; i++)
            {
                double result = conc[i] / conc.Sum();
                if (result > prob)
                {
                    prob = result;
                    cell = selection[i];
                }
            }
            if(cell == null)
                Console.WriteLine("Soy null");
            return cell;
        }

        private double AverageNutrientConcentration(Cell cell, double[,,] nutrien_conc)
        {
            List<double> conc = NutrientConcentrationValues(Utils.EmptyPositions(cell.neighborhood), nutrien_conc);

            if (conc.Count == 0) return 0;
            return conc.Average();
        }

        /// <summary>
        /// Metodo donde se obtiene la matrix de concentracion de nutrientes de las casillas vacias
        /// </summary>
        /// <param name="cell_list"></param>
        /// <param name="nutrien_conc"></param>
        /// <returns></returns>
        private List<double> NutrientConcentrationValues(List<Cell> cell_list, double[,,] nutrien_conc)
        {
            List<double> conc = new List<double>();
            foreach (var item in cell_list)
                conc.Add(nutrien_conc[item.pos.X, item.pos.Y, item.pos.Z]);
            return conc;
        }


        private double CalculateStandardDeviation(List<double> nutrient_concentration)
        {
            double average = nutrient_concentration.Average();
            double sum = nutrient_concentration.Sum(value => Math.Pow(value - average, 2));
            return Math.Sqrt(sum / nutrient_concentration.Count);
        }

        //La idea de las celulas migratorias es moverse a lugares donde exista alta concentracion de nutrientes
        //por tanto lo mas probable es que entre al lumen de los vasos sanguineos

        
        #endregion

        public bool CloserToTumoralCell(Cell cell)
        {
            return Utils.EuclideanDistance(cell.pos, tumor_stem_cell.pos) <= 5;
        }

        #region CooptionVessels
        //metodo que comprueba si hay vasos cooptados
        public void CooptionVessels()
        {
            //List<Cell> vasos_cooptados_temp = new List<Cell>();
            foreach (var item in pos_vessel_dict.Values)
            {
                int count = 0;
                foreach (var cell in item.neighborhood)
                {
                    if (cell.behavior_state == CellState.ProliferativeTumoralCell)
                        count++;
                }
                if (count >= 10)
                {
                    //vasos_cooptados_temp.Add(item);
                    vasos_cooptados.Add(item);
                }
            }
            //return vasos_cooptados_temp;

        }

        
        #endregion


        #region Angiogenesis

        public void Angiogenesis()
        {
            CrearCaminoParaAlcanzarAlTumor();

            ContinuarCrecimiento();
        }


        #region NewAngiogenesis

        //public void NewAngiogenesis()
        //{
        //    if(neo_blood_vessels.Count == 0)
        //    {
        //        foreach (Cell cell in pos_artery_dict.Values)
        //        {
        //            if (cell.neighborhood == null || cell.neighborhood.Count == 0)
        //                Console.WriteLine("Hell {0}", cell.neighborhood);

        //            if (!first_vessel_close_tumoral_cell_dict.ContainsKey(cell))
        //                first_vessel_close_tumoral_cell_dict.Add(cell, CancerCellClosestToTheVessel(cell));
        //        }
        //    }
        //    //else
        //    ArteriesNearTheTumor();
        //}

        public void NewAngiogenesis()
        {
            VEGFEnLosVasos();

            EndothelialCellMigration();

            VesselGrowth();
        }

        //////////////////ESTE METODO NO DEBE IR AQUI///////////////////
        private Tuple<Cell,int> CancerCellClosestToTheVessel(Cell artery)
        {
            Cell cell = null;
            int min_distance = int.MaxValue;

            foreach (var item in tumor.cell_list)
            {
                int distance = Utils.EuclideanDistance(artery.pos, item.Key);
                if (distance < min_distance)
                {
                    cell = item.Value;
                    min_distance = distance;
                }
            }

            return new Tuple<Cell, int>(cell, min_distance);
        }
        /////////////////////////////FINAL DEL METODO QUE NO DEBE IR AQUI
        

        //Metodo relacionado con el algoritmo de angiogenesis 2
        public void ArteriesNearTheTumor()
        {
            List<Cell> vessel_pos_to_growth = new List<Cell>();
            //HAY QUE TRABAJAR AQUI
            if (neo_blood_vessels.Count == 0)
            {
                foreach (Cell cell in pos_vessel_dict.Values)
                {
                    if (cell.neighborhood == null || cell.neighborhood.Count == 0)
                        Console.WriteLine("Hell {0}", cell.neighborhood);

                    if (cell == null)
                        Console.WriteLine("Soy null");
                    if (GrowthFactorInNeighboringCells(cell))
                        vessel_pos_to_growth.Add(cell);

                }
            }
            else
            {
                foreach (Cell cell in neo_blood_vessels)
                {
                    if (cell == null)
                        Console.WriteLine("Soy null");
                    if (GrowthFactorInNeighboringCells(cell))
                        vessel_pos_to_growth.Add(cell);

                }
            }

            foreach (Cell cell in vessel_pos_to_growth)
            {
                FindTheBestPositionForTheNewVesselUsingMigrateMethod(cell);
            }
        }


        //Metodo relacionado con el algoritmo de angiogenesis 2
        private bool GrowthFactorInNeighboringCells(Cell artery)
        {
            List<Cell> empty_cells = Utils.EmptyPositions(artery.neighborhood);
            List<double> tnf_list = NutrientConcentrationValues(empty_cells, model.vegf_conc_matrix);

            for (int i = 0; i < tnf_list.Count; i++)
            {
                var tum_cell_dist = first_vessel_close_tumoral_cell_dict[artery];
                Console.WriteLine("tnf_list {0}", tnf_list[i]);
                //if (tnf_list[i] > 0.1)
                //{
                Console.WriteLine("Ya se cumple que el tnf es > 0.1");
                if (Utils.EuclideanDistance(empty_cells[i].pos, tum_cell_dist.Item1.pos) < tum_cell_dist.Item2) return true;
                //}
            }
            return false;
        }

        //Metodo relacionado con el algoritmo de angiogenesis 2
        public void FindTheBestPositionForTheNewVesselUsingMigrateMethod(Cell artery)
        {
            if (CheckIsNotANeighborOfTumorCell(artery))
            {
                Cell cell = SelectionOfCellsToMigrate(artery, model.vegf_conc_matrix);
                if (cell != null)
                {
                    Console.WriteLine("Neo Blood Vessel Pos: {0} {1} {2} VEGF: {3}", cell.pos.X, cell.pos.Y, cell.pos.Z, model.vegf_conc_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z]);
                    neo_blood_vessels.Add(cell);
                }
            }
            else
                final_tip_of_vessel_list.Add(artery);
        }

        //Sirve para ambos modelos
        public bool CheckIsNotANeighborOfTumorCell(Cell cell)
        {
            foreach (var item in cell.neighborhood)
            {
                if (item.behavior_state == CellState.ProliferativeTumoralCell || item.behavior_state == CellState.QuiescentTumorCell || item.behavior_state == CellState.MigratoryTumorCell)
                    return false;
            }
            return true;
        }

        //Metodo relacionado con el algoritmo de angiogenesis 1
        public void FindTheBestPositionForTheNewVessel(Cell artery)
        {
            if (CheckIsNotANeighborOfTumorCell(artery))
            {
                List<Cell> empty_cells = Utils.EmptyPositions(artery.neighborhood);

                double density_min = double.MaxValue;
                Cell cell = null;

                for (int i = 0; i < empty_cells.Count; i++)
                {
                    Pos pos = empty_cells[i].pos;
                    if (model.density_matrix[pos.X, pos.Y, pos.Z] < density_min)
                    {
                        density_min = model.density_matrix[pos.X, pos.Y, pos.Z];
                        cell = empty_cells[i];
                    }
                }

                

                neo_blood_vessels.Add(cell);
            }
            else
                final_tip_of_vessel_list.Add(artery);
        }

        

        
        #endregion


        #region Codigo no utilizado por ahora
        public void CrearCaminoParaAlcanzarAlTumor()
        {
            List<Tuple<Cell, Cell>> tumorCells_bloodVessels = NewVesselsFormation();

            int i = Utils.rdm.Next(0, tumorCells_bloodVessels.Count);

            Tuple<Cell, Cell> tumorCell_bloodVessel = tumorCells_bloodVessels[i];
            Cell vecino_mas_cercano = VecinoMasCercano(tumorCell_bloodVessel);

            if (tumorCell_bloodVessel.Item2 is BloodVessel)
            {
                //new_artery_list.Add(vecino_mas_cercano);

                //Arteriole arteriole = new Arteriole(vecino_mas_cercano.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //pos_arteriole_dict.Add(vecino_mas_cercano.pos, arteriole);
                //space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;

                vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
                //vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(tumorCell_bloodVessel.Item1, arteriole));

                //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
            }
            //else if(tumorCell_bloodVessel.Item2 is Cell)
            //{
            //    new_artery_list.Add(vecino_mas_cercano);
            //    vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
            //}
            //else if(tumorCell_bloodVessel.Item2 is Capillary)
            //{
            //    new_capillary_list.Add(vecino_mas_cercano);
            //    vasos_en_crecimiento_dict.Add(tumorCell_bloodVessel.Item1, vecino_mas_cercano);
            //}
        }

        //Aqui lo que estoy haciendo es buscando todas las casillas vacias alrededor del tumor para hacer crecer un nuevo vaso hasta esa casilla;
        public List<Tuple<Cell, Cell>> NewVesselsFormation()
        {
            int min = int.MaxValue;
            Cell blood_vessel = null;
            List<Cell> cells = BuscarPosicionesAlBordeDelTumor();

            List<Cell> blood_vessels = new List<Cell>();
            blood_vessels.AddRange(pos_vessel_dict.Values);
            //blood_vessels.AddRange(pos_arteriole_dict.Values);
            //blood_vessels.AddRange(pos_capillary_dict.Values);

            List<Tuple<Cell, Cell>> tumorCell_bloodVessel = new List<Tuple<Cell, Cell>>();

            foreach (var item in cells)
            {
                foreach (var item2 in blood_vessels)
                {
                    if (!vasos_cooptados.Contains(item2))
                    {
                        int distance = Utils.EuclideanDistance(item.pos, item2.pos);
                        if (distance < min)
                        {
                            min = distance;
                            blood_vessel = item2;
                        }
                    }

                }
                tumorCell_bloodVessel.Add(new Tuple<Cell, Cell>(item, blood_vessel));
            }
            return tumorCell_bloodVessel;


        }

        public List<Cell> BuscarPosicionesAlBordeDelTumor()
        {
            List<Cell> emptyPosition = new List<Cell>();
            foreach (var item in tumor.cell_list)
            {
                foreach (var item1 in item.Value.neighborhood)
                {
                    if (item1.behavior_state == CellState.nothing)
                        emptyPosition.Add(item1);
                }
            }
            return emptyPosition;
        }

        //hay que ver porque una vez que se esta formando un vaso no puede aparecer en la creacion


        public Cell VecinoMasCercano(Tuple<Cell, Cell> tumorCell_bloodVessel)
        {
            int min = int.MaxValue;
            Cell vecino_mas_cercano = null;
            foreach (var item in tumorCell_bloodVessel.Item2.neighborhood)
            {
                if (item.behavior_state == CellState.nothing && item.loca_state == CellLocationState.MatrixExtracelular)
                {
                    int distance = Utils.EuclideanDistance(tumorCell_bloodVessel.Item1.pos, item.pos);
                    if (distance < min)
                    {
                        min = distance;
                        vecino_mas_cercano = item;
                    }
                }
                //Hay que analizar si cuando se actualiza el automata se cierra el camino porque aparece una celula tumoral en el medio
            }
            return vecino_mas_cercano;
        }

        //una vez se crean los vasos este metodo es el update del crecimiento
        public void ContinuarCrecimiento()
        {
            Dictionary<Cell, Cell> temp = new Dictionary<Cell, Cell>();
            foreach (var item in vasos_en_crecimiento)
            {
                var vecino = VecinoMasCercano(item);
                //if (item.Item2 is Arteriole)
                //{
                //    new_artery_list.Add(vecino);
                //    //Arteriole arteriole = new Arteriole(vecino.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                //    //pos_arteriole_dict.Add(vecino.pos, arteriole);
                //    //space[arteriole.pos.X, arteriole.pos.Y, arteriole.pos.Z] = arteriole;

                //    temp.Add(item.Item1, vecino);
                //    //vasos_en_crecimiento.Add(new Tuple<Cell, Cell>(item.Item1, arteriole));

                //    //AGREGAR AQUI ESTA ARTERIOLA AL DICCIONARIO DE ARTERIAS CON ARTERIOLAS
                //}
                if (item.Item2 is BloodVessel)
                {
                    //new_artery_list.Add(vecino);
                    temp.Add(item.Item1, vecino);
                }
                //else if (item.Item2 is Capillary)
                //{
                //    new_capillary_list.Add(vecino);
                //    temp.Add(item.Item1, vecino);
                //}
            }
            vasos_en_crecimiento_dict = temp;

        }
        #endregion


        #endregion

        public void CelulasTumoralesSinEspacio()
        {
            foreach (var item in tumor.cell_list)
            {
                if(item.Value.behavior_state == CellState.ProliferativeTumoralCell || item.Value.behavior_state == CellState.MigratoryTumorCell)
                {
                    if(Utils.EmptyPositions(item.Value.neighborhood).Count == 0 && !new_quiescent_cells.ContainsKey(item.Key))
                    {
                        new_quiescent_cells.Add(item.Key, item.Value);
                    }
                }
            }
        }


        public void UpdateProliferationAgeCounter()
        {
            foreach (var key_value in proliferation_cells)
            {
                if (key_value.Value[0] <= proliferation_age)
                {
                    double randomVariable = distribution.Sample();
                    key_value.Key.proliferation_age = randomVariable;
                    key_value.Value[0] += Utils.rdm.Next(1, 5);
                }
                else
                {
                    List<Cell> empty_cell = Utils.EmptyPositions(key_value.Key.neighborhood, empty_cells);
                    if (empty_cell.Count > 0)
                    {
                        double density_min = double.MaxValue;
                        Cell cell = null;
                        for (int i = 0; i < empty_cell.Count; i++)
                        {
                            Pos pos = empty_cell[i].pos;
                            if (!new_tumoral_cells.ContainsKey(pos))
                            {

                                if (model.density_matrix[pos.X, pos.Y, pos.Z] < density_min)
                                {
                                    density_min = model.density_matrix[pos.X, pos.Y, pos.Z];
                                    cell = empty_cell[i];
                                }
                            }
                        }
                        if (cell != null)
                        {
                            new_tumoral_cells.Add(cell.pos, cell);
                            //new_tumoral_cells.Add(empty_cell[Utils.rdm.Next(0, empty_cell.Count)]);

                            key_value.Key.proliferation_age = 0;
                            key_value.Value[0] = 0;
                        }
                    }
                    else
                        new_quiescent_cells.Add(key_value.Key.pos, key_value.Key);
                }
            }
        }


        #region Update

        public void Update()
        {
            Stopwatch crono = new Stopwatch();

            //Aspectos relacionados con el tumor


            if (tumor.cell_list.Count == tumor.new_cells_count)
            {
                tumor.time++;
                tumor.VerhulstEquation();
            }


            crono.Start();
            model.UpdateOxygen(space, tumor.time);
            //model.UpdateOxygenConcentration(space, tumor.time);

            model.VEGF(space, tumor.time);
            model.UpdateEndothelialDensity(space, tumor.time);
            model.UpdateMDE2(space, tumor.time, tumor);
            model.UpdateECMDensity2(space, tumor.time);

            crono.Stop();
            Console.WriteLine("Concentration: {0} segundos", crono.ElapsedMilliseconds / 1000);

            crono.Start();
            


            //Variables relacionadas con las celulas proliferativas
            UpdateProliferationAgeCounter();


            //ObtainCellsPositionsToDivide();

            CelulasTumoralesSinEspacio();

            SearchCloserStemPosition();

            if(new_endothelial_cells.Count>0)
            {
                foreach (var item in new_endothelial_cells)
                {
                    Console.WriteLine("ENDO {0} {1} {2} {3}",space[item.Key.X, item.Key.Y, item.Key.Z].behavior_state,  item.Key.X, item.Key.Y, item.Key.Z);
                }
            }

            crono.Stop();
            Console.WriteLine("Antes del update: {0} segundos", crono.ElapsedMilliseconds / 1000);

            crono.Start();

            //foreach (Cell cell in random_space)
            //{
                
            //}
            for (int i = 0; i < random_space.Length; i++)
            {
                Cell cell = random_space[i];
                if (cell.behavior_state != CellState.nothing || (next_old_stem_position.ContainsKey(cell.pos) || next_migratory_cells.ContainsKey(cell.pos) || new_tumoral_cells.ContainsKey(cell.pos)))
                {
                    //Stopwatch crono1 = new Stopwatch();
                    //crono1.Start();
                    UpdateState(cell, i);
                    //crono1.Stop();
                    //Console.WriteLine("Update Cell: {0} segundos", crono1.ElapsedMilliseconds / 1000);
                }
            }

            crono.Stop();
            Console.WriteLine("Update Cells: {0} segundos", crono.ElapsedMilliseconds / 1000);


            //ClearCloserCellsToVessels();
            crono.Start();

            UpdateOxygenConcentration(new_proliferative_cells);
            new_proliferative_cells = new List<Cell>();

            //Metodos que actualizan la lista actual y vacian la lista que representa el estado en el siguiente estado
            UpdateNextStemPositionDict();


            #region Nueva Organizacion
            UpdateImmatureVesselSegmentRadius();
            UpdateMaturationTime();
            UpdateBloodFlow();
            WSS();

            #endregion

            if (tumoral_angiogenic_factor > 0.5)
            {
                if (tumor.tumor_stage == TumosStage.Avascular)
                {
                    tumor.vasc_mecha = VascularizationMechanism.Angiogenesis;
                    tumor.tumor_stage = TumosStage.Vascular;
                }
                //Angiogenesis();
                UpdateMigratoryCells();

                NewAngiogenesis();
            }

            UpdateNewNeoVesselsSegment();

            //UpdateBloodVessels();

            UpdateNewMigratoryCells();


            crono.Stop();
            Console.WriteLine("Restantes metodos: {0} segundos", crono.ElapsedMilliseconds / 1000);

            foreach (var item in pos_cell_dict)
            {
                if(item.Value.behavior_state == CellState.nothing)
                    Console.WriteLine("Hay una celula vacia en el diccionario de pos_cell_dict {0} {1} {2}", item.Key.X, item.Key.Y, item.Key.Z);
            }

            Console.WriteLine("tiempo {0} ", tumor.time);
            Console.WriteLine("Crecimiento {0}", tumor.new_cells_count);
            //Console.WriteLine("cantidad de celulas tumorales {0}", tumor.cell_list.Count);
            //Console.WriteLine("cantidad de celulas que da la ecuacion de verhuslt {0}", tumor.new_cells_count);
            Console.WriteLine();

            //time[tumor.time] = tumor.time;
            //cell_count[tumor.time] = tumor.cell_list.Count;
        }

        private void UpdateBloodVessels()
        {
            if (next_blood_vessels.Count > 0)
            {
                blood_vessels_actual = next_blood_vessels;
                next_blood_vessels = new List<Cell>();
            }
        }

        public void UpdateNewMigratoryCells()
        {
            if (next_migratory_cells.Count > 0)
            {
                migratory_cells_actual = next_migratory_cells;
                next_migratory_cells = new Dictionary<Pos, Cell>();
            }
        }

        private void UpdateOxygenConcentration(List<Cell> cell_list)
        {
            foreach (var item in cell_list)
            {
                //int x = item.pos.X;
                //int y = item.pos.Y;
                //int z = item.pos.Z;
                //if(y == 0 || y == space.GetLength(1) || z == 0 || z == space.GetLength(2))
                //    model.oxygen_matrix[item.pos.X, item.pos.Y, item.pos.Z] = 0.28;
                //else
                //    model.oxygen_matrix[item.pos.X, item.pos.Y, item.pos.Z] = 1.0;

                if (model.TumorNeighborhood(item))
                    model.oxygen_matrix[item.pos.X, item.pos.Y, item.pos.Z] = model.oxygen_tumoral_threshold_1;
                else
                    model.oxygen_matrix[item.pos.X, item.pos.Y, item.pos.Z] = model.oxygen_tumoral_threshold_2;

                //Console.WriteLine(model.oxygen_matrix[item.pos.X, item.pos.Y, item.pos.Z]);
            }
        }

        public void UpdateTumorState()
        {
            if (tumor.vasc_mecha == VascularizationMechanism.InitialGrowth)
            {
                tumor.vasc_mecha = VascularizationMechanism.VascularCooption;
            }
            else if (tumor.vasc_mecha == VascularizationMechanism.VascularCooption)
            {
                tumor.vasc_mecha = VascularizationMechanism.Angiogenesis;
                tumor.tumor_stage = TumosStage.Vascular;
            }
            else if (tumor.vasc_mecha == VascularizationMechanism.Angiogenesis)
            {
                tumor.vasc_mecha = VascularizationMechanism.Transdifferentiation;
            }
        }

        public void UpdateNextStemPositionDict()
        {
            List<Pos> remove_pos = new List<Pos>();
            foreach (var item in next_old_stem_position)
            {
                if (stem_cell_list.ContainsKey(item.Key))
                    remove_pos.Add(item.Key);
            }

            foreach (var item in remove_pos)
                next_old_stem_position.Remove(item);
        }

        #region UPDATE BEHAVIOR CELLS
        public void UpdateState(Cell cell, int pos)
        {
            //foreach (var item in new_endothelial_cells.Values)
            //{
            //    if (item == cell)
            //    {

            //    }
            //    int x = item.pos.X;
            //    int y = item.pos.Y;
            //    int z = item.pos.Z;

            //    int x1 = cell.pos.X;
            //    int y1 = cell.pos.Y;
            //    int z1 = cell.pos.Z;

            //    if (x == x1 && y == y1 && z == z1)
            //    {
            //        if (new_endothelial_cells.ContainsKey(cell.pos) && endothelial_cells[cell.pos] == cell)
            //        {

            //        }
            //    }
            //}
                //UpdateCellWithNoOxigen(cell);

                int tumoral_cells_count = cell.TumoralCellCount();

            if (cell.behavior_state == CellState.Astrocyte || cell.behavior_state == CellState.Neuron)
                UpdateAstrocyteOrNeuronCell(cell);
            else if (cell.behavior_state == CellState.ProliferativeTumoralCell)
                UpdateProliferativeTumorCell(cell);
            else if (cell.behavior_state == CellState.MigratoryTumorCell)
                UpdateMigratoryTumorCell(cell);
            else if (cell.behavior_state == CellState.StemCell)
                UpdateStemCell(cell, tumoral_cells_count);
            else if (cell.behavior_state == CellState.QuiescentTumorCell)
                UpdateQuiescentCell(cell);
            else if (cell.behavior_state == CellState.nothing && cell.loca_state == CellLocationState.MatrixExtracelular)
                UpdateEmptyCell(cell);
            else if (cell.behavior_state == CellState.EndothelialCell)
                UpdateEndothelialCell(cell, pos);

            //if(new_endothelial_cells. == cell.pos)
            //{
                
            //}
            

                
                //if(item.X == cell.pos.X && item.Y == cell.pos.Y && item.Z == cell.pos.Z)
                //{
                //    if(new_endothelial_cells.ContainsKey(cell.pos))
                //    {

                //    }
                //}
            //if (new_endothelial_cells.ContainsKey(cell.pos))
            //{

            //}
        }

        

        public void UpdateAstrocyteOrNeuronCell(Cell cell)
        {
            double prob = move_prob.ContamineProbability(cell, cell.neighborhood, tumor);
            if (0.09 < prob)
            {
                if (cell.behavior_state == CellState.Astrocyte)
                    astrocyte_list.Remove(cell.pos);
                else if (cell.behavior_state == CellState.Neuron)
                    neuron_list.Remove(cell.pos);

                cell.behavior_state = CellState.ProliferativeTumoralCell;
                cell.proliferation_age = 0;
                tumor.cell_list.Add(cell.pos, cell);

                new_proliferative_cells.Add(cell);

                proliferation_cells.Add(cell, new int[] { 0, 0 });

            }
            //si no se cumple eso entonces sigue con el mismo estado
        }

        public void UpdateProliferativeTumorCell(Cell cell)
        {
            if (new_quiescent_cells.ContainsKey(cell.pos))
            {
                proliferation_cells.Remove(cell);
                new_quiescent_cells.Remove(cell.pos);

                cell.behavior_state = CellState.QuiescentTumorCell;
                cell.proliferation_age = -1;

                quiescent_cell_list.Add(cell.pos, cell);
                //ARREGLAR EL VALOR A AGREGAR
                tumoral_angiogenic_factor += 0.01;
            }
            else if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] < 1.0 && !quiescent_cell_list.ContainsKey(cell.pos))
            {
                proliferation_cells.Remove(cell);

                cell.behavior_state = CellState.QuiescentTumorCell;
                cell.proliferation_age = -1;

                quiescent_cell_list.Add(cell.pos, cell);

                tumoral_angiogenic_factor += 0.01;
            }
            if (next_migratory_cells.ContainsKey(cell.pos))
            {
                proliferation_cells.Remove(cell);

                cell.behavior_state = CellState.MigratoryTumorCell;
                //next_migratory_position.Add(cell.pos, Utils.NullPos());
                migratory_cells_actual.Add(cell.pos,cell);
            }
            //tener en cuenta aqui tambien lo de la migracion de las celulas tumorales

        }
        //List<Pos> next_migratory_cellsRemove = new List<Pos>();
        //List<Pos> migratory_cells_actualRemove = new List<Pos>();

        public void UpdateMigratoryTumorCell(Cell cell)
        {
            if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
            {
                next_migratory_cells.Remove(cell.pos);
                migratory_cells_actual.Remove(cell.pos);

                cell.behavior_state = CellState.NecroticTumorCell;
                necrotic_cell_list.Add(cell);
            }
            else if (new_quiescent_cells.ContainsKey(cell.pos))
            {
                new_quiescent_cells.Remove(cell.pos);
                migratory_cells_actual.Remove(cell.pos);

                cell.behavior_state = CellState.QuiescentTumorCell;
                cell.proliferation_age = -1;

                quiescent_cell_list.Add(cell.pos, cell);
                //ARREGLAR EL VALOR A AGREGAR
                tumoral_angiogenic_factor += 0.01;
            }
            else if (migratory_cells_actual.ContainsKey(cell.pos))
            {
                pos_cell_dict.Remove(cell.pos);
                tumor.cell_list.Remove(cell.pos);

                cell.behavior_state = CellState.nothing;
            }
        }

        public void UpdateStemCell(Cell cell, int tumoral_cells_count)
        {
            if (!stem_cell_list.ContainsKey(cell.pos))
            {
                //stem_cell_list.Remove(cell.pos);
                pos_cell_dict.Remove(cell.pos);

                cell.behavior_state = CellState.nothing;

            }
            else if ((tumoral_cells_count >= 1 && tumor.new_cells_count > tumor.cell_list.Count) || (CloserToTumoralCell(cell) && tumor.new_cells_count > tumor.cell_list.Count))
            {
                stem_cell_list.Remove(cell.pos);
                next_old_stem_position.Remove(cell.pos);

                cell.behavior_state = CellState.ProliferativeTumoralCell;
                cell.proliferation_age = 0;
                tumor.cell_list.Add(cell.pos,cell);

                new_proliferative_cells.Add(cell);

                proliferation_cells.Add(cell, new int[] { 0, 0 });

            }
            //else if ()
            //{
            //    stem_cell_list.Remove(cell.pos);
            //    next_stem_position.Remove(cell.pos);

            //    //aqui es donde se supone que hay que analizar las celulas que tiene alrededor de un radio de 5 celdas
            //    cell.behavior_state = CellState.ProliferativeTumoralCell;
            //    cell.proliferation_age = 0;
            //    tumor.cell_list.Add(cell.pos, cell);

            //    new_proliferative_cells.Add(cell);

            //    proliferation_cells.Add(cell, new int[] { 0, 0 });

            //}
        }

        public void UpdateQuiescentCell(Cell cell)
        {
            if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
            {
                quiescent_cell_list.Remove(cell.pos);

                cell.behavior_state = CellState.NecroticTumorCell;
                necrotic_cell_list.Add(cell);
            }
            tumoral_angiogenic_factor += 0.01;
        }

        public void UpdateEmptyCell(Cell cell)
        {



            if (next_old_stem_position.ContainsKey(cell.pos))
            {
                //if (empty_cells.Contains(cell))
                //    empty_cells.Remove(cell);

                cell.behavior_state = CellState.StemCell;
                pos_cell_dict.Add(cell.pos, cell);

                stem_cell_list.Add(cell.pos, cell);

                stem_cell_list.Remove(next_old_stem_position[cell.pos]);
            }
            else if (new_endothelial_cells.ContainsKey(cell.pos))
            {
                new_endothelial_cells.Remove(cell.pos);

                cell.behavior_state = CellState.EndothelialCell;
                model.endo_density_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] = model.initial_density_endo;

                //No se si deba agregarlo al tumor
                endothelial_cells.Add(cell.pos, cell);
                pos_cell_dict.Add(cell.pos, cell);
            }
            else if (next_migratory_cells.ContainsKey(cell.pos))
            {
                //if (empty_cells.Contains(cell))
                //    empty_cells.Remove(cell);

                //proliferation_cells.Remove(cell);

                cell.behavior_state = CellState.MigratoryTumorCell;
                migratory_cells_actual.Add(cell.pos, cell);
                pos_cell_dict.Add(cell.pos, cell);
            }
            else if (tumor.new_cells_count > tumor.cell_list.Count && (Utils.rdm.NextDouble() < move_prob.DivisionProbability(cell, cell.neighborhood, tumor) || new_tumoral_cells.ContainsKey(cell.pos)))
            {
                //if (empty_cells.Contains(cell))
                //    empty_cells.Remove(cell);

                cell.behavior_state = CellState.ProliferativeTumoralCell;
                cell.proliferation_age = 0;
                pos_cell_dict.Add(cell.pos, cell);
                tumor.cell_list.Add(cell.pos, cell);

                new_proliferative_cells.Add(cell);

                proliferation_cells.Add(cell, new int[] { 0, 0 });

            }
            //else if (neo_blood_vessels.Contains(cell))
            //{
            //    cell = new BloodVessel(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
            //    cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);
            //    pos_neo_vessel_dict.Add(cell.pos, (BloodVessel)cell);
            //}
            
        }

        public void UpdateEndothelialCell(Cell cell, int pos)
        {
            if(new_vessel_cells.Contains(cell))
            {
                endothelial_cells.Remove(cell.pos);
                new_vessel_cells.Remove(cell);
                pos_cell_dict.Remove(cell.pos);

                cell.behavior_state = CellState.nothing;
                cell.loca_state = CellLocationState.GlialBasalLamina;
                cell = new BloodVessel(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
                cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);
                space[cell.pos.X, cell.pos.Y, cell.pos.Z] = cell;
                random_space[pos]= cell;

                //hay que adicionar los nuevos vasos a la lista de nuevos vasos
                neo_blood_vessel_list.Add((BloodVessel)cell);
                pos_neo_vessel_dict.Add(cell.pos, (BloodVessel)cell);

            }
        }

        //public void UpdateCellWithNoOxigen(Cell cell)
        //{
        //    CellState cell_state = cell.behavior_state;
        //    if (cell_state != CellState.nothing && model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
        //    {
        //        if (cell_state == CellState.ProliferativeTumoralCell)
        //        {
        //            proliferation_cells.Remove(cell);
        //            cell.proliferation_age = -1;

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else if (cell_state == CellState.QuiescentTumorCell)
        //        {
        //            quiescent_cell_list.Remove(cell.pos);

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else if (cell_state == CellState.MigratoryTumorCell)
        //        {
        //            next_migratory_cells.Remove(cell.pos);
        //            migratory_cells_actual.Remove(cell.pos);

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else
        //        {

        //        }

        //    }
        //}
        #endregion

        //List<Pos> new_endothelial_cellsRemove = new List<Pos>();

        //List<Pos> empty_cellsRemove = new List<Pos>();

        //List<Pos> pos_cell_dictRemove = new List<Pos>();

        //List<Pos> stem_cell_listRemove = new List<Pos>();

        //List<Pos> proliferation_cellsRemove = new List<Pos>();

        //List<Pos> quiescent_cell_listRemove = new List<Pos>();

        //List<Pos> tumor_cell_listRemove = new List<Pos>();

        //List<Pos> astrocyte_listRemove = new List<Pos>();
        //List<Pos> neuron_listRemove = new List<Pos>();
        //List<Pos> new_quiescent_cellsRemove = new List<Pos>();



        #endregion

        #region UPDATE BEHAVIOR CELLS
        //public void UpdateState(Cell cell)
        //{
        //    //UpdateCellWithNoOxigen(cell);

        //    int tumoral_cells_count = cell.TumoralCellCount();

        //    if (cell.behavior_state == CellState.Astrocyte || cell.behavior_state == CellState.Neuron)
        //        UpdateAstrocyteOrNeuronCell(cell);
        //    else if (cell.behavior_state == CellState.ProliferativeTumoralCell)
        //        UpdateProliferativeTumorCell(cell);
        //    //else if (cell.behavior_state == CellState.MigratoryTumorCell)
        //    //    UpdateMigratoryTumorCell(cell);
        //    else if (cell.behavior_state == CellState.StemCell)
        //        UpdateStemCell(cell, tumoral_cells_count);
        //    //else if (cell.behavior_state == CellState.QuiescentTumorCell)
        //    //    UpdateQuiescentCell(cell);
        //    else if (cell.behavior_state == CellState.nothing && cell.loca_state == CellLocationState.MatrixExtracelular)
        //        UpdateEmptyCell(cell);
        //    //else if (cell.behavior_state == CellState.EndothelialCell)
        //    //    UpdateEndothelialCell(cell);
        //}



        //public void UpdateAstrocyteOrNeuronCell(Cell cell)
        //{
        //    double prob = move_prob.ContamineProbability(cell, cell.neighborhood, tumor);
        //    if (0.09 < prob)
        //    {
        //        if (cell.behavior_state == CellState.Astrocyte)
        //        {
        //            astrocyte_list.Remove(cell.pos);
        //            //astrocyte_listRemove.Add(cell.pos);
        //        }
        //        else if (cell.behavior_state == CellState.Neuron)
        //        {
        //            neuron_list.Remove(cell.pos);
        //            //neuron_listRemove.Add(cell.pos);
        //        }

        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });



        //        //tumor.UpdateNewCellCount();
        //    }
        //    //si no se cumple eso entonces sigue con el mismo estado
        //}

        //public void UpdateProliferativeTumorCell(Cell cell)
        //{
        //    //if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
        //    //{
        //    //    proliferation_cells.Remove(cell);
        //    //    //proliferation_cellsRemove.Add(cell.pos);

        //    //    cell.proliferation_age = -1;

        //    //    cell.behavior_state = CellState.NecroticTumorCell;
        //    //    necrotic_cell_list.Add(cell);
        //    //}
        //    if (new_quiescent_cells.ContainsKey(cell.pos))
        //    {
        //        proliferation_cells.Remove(cell);
        //        //proliferation_cellsRemove.Add(cell.pos);

        //        new_quiescent_cells.Remove(cell.pos);
        //        //new_quiescent_cellsRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.QuiescentTumorCell;
        //        cell.proliferation_age = -1;

        //        quiescent_cell_list.Add(cell.pos, cell);
        //        //ARREGLAR EL VALOR A AGREGAR
        //        tumoral_angiogenic_factor += 0.01;
        //    }
        //    if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] < 1.0)
        //    {
        //        proliferation_cells.Remove(cell);
        //        //proliferation_cellsRemove.Add(cell.pos);


        //        cell.behavior_state = CellState.QuiescentTumorCell;
        //        cell.proliferation_age = -1;

        //        quiescent_cell_list.Add(cell.pos, cell);

        //        tumoral_angiogenic_factor += 0.01;
        //    }
        //    if (next_migratory_cells.ContainsKey(cell.pos))
        //    {
        //        proliferation_cells.Remove(cell);
        //        //proliferation_cellsRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.MigratoryTumorCell;
        //        //next_migratory_position.Add(cell.pos, Utils.NullPos());
        //        migratory_cells_actual.Add(cell.pos, cell);
        //    }
        //    //tener en cuenta aqui tambien lo de la migracion de las celulas tumorales

        //}
        ////List<Pos> next_migratory_cellsRemove = new List<Pos>();
        ////List<Pos> migratory_cells_actualRemove = new List<Pos>();

        //public void UpdateMigratoryTumorCell(Cell cell)
        //{
        //    //if (next_migratory_position.ContainsKey(cell.pos) && next_migratory_position[cell.pos].NullPos())
        //    //{
        //    //    pos_cell_dict.Remove(cell.pos);
        //    //    tumor.cell_list.Remove(cell);
        //    //    cell.behavior_state = CellState.nothing;
        //    //}
        //    if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
        //    {
        //        next_migratory_cells.Remove(cell.pos);
        //        //next_migratory_cellsRemove.Add(cell.pos);

        //        migratory_cells_actual.Remove(cell.pos);
        //        //migratory_cells_actualRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.NecroticTumorCell;
        //        necrotic_cell_list.Add(cell);
        //    }
        //    else if (new_quiescent_cells.ContainsKey(cell.pos))
        //    {
        //        new_quiescent_cells.Remove(cell.pos);
        //        //new_quiescent_cellsRemove.Add(cell.pos);

        //        migratory_cells_actual.Remove(cell.pos);
        //        //migratory_cells_actualRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.QuiescentTumorCell;
        //        cell.proliferation_age = -1;

        //        quiescent_cell_list.Add(cell.pos, cell);
        //        //ARREGLAR EL VALOR A AGREGAR
        //        tumoral_angiogenic_factor += 0.01;
        //    }
        //    else if (migratory_cells_actual.ContainsKey(cell.pos))
        //    {
        //        pos_cell_dict.Remove(cell.pos);
        //        //pos_cell_dictRemove.Add(cell.pos);

        //        tumor.cell_list.Remove(cell.pos);
        //        //tumor_cell_listRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.nothing;
        //    }
        //}

        //public void UpdateStemCell(Cell cell, int tumoral_cells_count)
        //{
        //    if (next_stem_position.ContainsKey(cell.pos))
        //    {
        //        stem_cell_list.Remove(cell.pos);
        //        //stem_cell_listRemove.Add(cell.pos);

        //        pos_cell_dict.Remove(cell.pos);
        //        //pos_cell_dictRemove.Add(cell.pos);


        //        cell.behavior_state = CellState.nothing;

        //    }
        //    else if (tumoral_cells_count >= 1 /*&& Utils.rdm.Next(0, 2) == 1*/ && tumor.new_cells_count > tumor.cell_list.Count)
        //    {
        //        stem_cell_list.Remove(cell.pos);
        //        //stem_cell_listRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });

        //        //tumor.UpdateNewCellCount();
        //        //pos_cell_dict.Add(cell.pos, cell);
        //    }
        //    else if (CloserToTumoralCell(cell) /*&& Utils.rdm.Next(0, 2) == 1*/ && tumor.new_cells_count > tumor.cell_list.Count)
        //    {
        //        stem_cell_list.Remove(cell.pos);
        //        //stem_cell_listRemove.Add(cell.pos);

        //        next_stem_position.Remove(cell.pos);

        //        //aqui es donde se supone que hay que analizar las celulas que tiene alrededor de un radio de 5 celdas
        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });

        //        //tumor.UpdateNewCellCount();
        //        //pos_cell_dict.Add(cell.pos, cell);
        //    }
        //}

        //public void UpdateQuiescentCell(Cell cell)
        //{
        //    if (model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
        //    {
        //        quiescent_cell_list.Remove(cell.pos);
        //        //quiescent_cell_listRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.NecroticTumorCell;
        //        necrotic_cell_list.Add(cell);
        //    }
        //    //Pos pos = cell.pos;
        //    //if(model.oxygen_matrix[pos.X, pos.Y, pos.Z] < 0.28)//cambiar este valor
        //    //    cell.behavior_state = CellState.NecroticTumorCell;
        //    //else
        //    tumoral_angiogenic_factor += 0.01;
        //}

        //public void UpdateEmptyCell(Cell cell)
        //{
        //    if (next_stem_position.ContainsValue(cell.pos))
        //    {
        //        if (empty_cells.Contains(cell))
        //        {
        //            empty_cells.Remove(cell);
        //            //empty_cellsRemove.Add(cell.pos);
        //        }

        //        cell.behavior_state = CellState.StemCell;
        //        pos_cell_dict.Add(cell.pos, cell);

        //        stem_cell_list.Add(cell.pos, cell);
        //    }
        //    else if (closer_cells_to_vessels.Contains(cell) && tumor.new_cells_count > tumor.cell_list.Count && Utils.rdm.NextDouble() < move_prob.DivisionProbability(cell, cell.neighborhood, tumor))
        //    {
        //        if (empty_cells.Contains(cell))
        //        {
        //            empty_cells.Remove(cell);
        //            //empty_cellsRemove.Add(cell.pos);
        //        }

        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        pos_cell_dict.Add(cell.pos, cell);
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });

        //        //tumor.UpdateNewCellCount();
        //    }
        //    else if (next_migratory_cells.ContainsKey(cell.pos))
        //    {
        //        if (empty_cells.Contains(cell))
        //        {
        //            empty_cells.Remove(cell);
        //            //empty_cellsRemove.Add(cell.pos);
        //        }

        //        //proliferation_cells.Remove(cell);

        //        cell.behavior_state = CellState.MigratoryTumorCell;
        //        migratory_cells_actual.Add(cell.pos, cell);
        //        pos_cell_dict.Add(cell.pos, cell);
        //    }
        //    else if (tumor.new_cells_count > tumor.cell_list.Count && Utils.rdm.NextDouble() < move_prob.DivisionProbability(cell, cell.neighborhood, tumor))
        //    {
        //        if (empty_cells.Contains(cell))
        //        {
        //            empty_cells.Remove(cell);
        //            //empty_cellsRemove.Add(cell.pos);
        //        }

        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        pos_cell_dict.Add(cell.pos, cell);
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);
        //        //System.ArgumentException: 'An item with the same key has already been added.'

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });

        //        //tumor.UpdateNewCellCount();
        //    }
        //    //else if (next_migratory_position.ContainsValue(cell.pos))
        //    //{
        //    //    if (empty_cells.Contains(cell))
        //    //        empty_cells.Remove(cell);

        //    //    cell.behavior_state = CellState.MigratoryTumorCell;
        //    //    migratory_cells.Add(cell);
        //    //    pos_cell_dict.Add(cell.pos, cell);
        //    //}

        //    //else if (new_artery_list.Contains(cell))
        //    //{
        //    //    if (empty_cells.Contains(cell))
        //    //        empty_cells.Remove(cell);

        //    //    pos_cell_dict.Remove(cell.pos);
        //    //    cell = new Cell(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
        //    //    pos_artery_dict.Add(cell.pos, (Artery)cell);

        //    //    space[cell.pos.X, cell.pos.Y, cell.pos.Z] = cell;
        //    //}
        //    else if (new_tumoral_cells.Contains(cell) && tumor.new_cells_count > tumor.cell_list.Count)
        //    {
        //        if (empty_cells.Contains(cell))
        //        {
        //            empty_cells.Remove(cell);
        //            //empty_cellsRemove.Add(cell.pos);
        //        }

        //        cell.behavior_state = CellState.ProliferativeTumoralCell;
        //        cell.proliferation_age = 0;
        //        pos_cell_dict.Add(cell.pos, cell);
        //        tumor.cell_list.Add(cell.pos, cell);

        //        new_proliferative_cells.Add(cell);

        //        proliferation_cells.Add(cell, new int[] { 0, 0 });
        //    }
        //    else if (neo_blood_vessels.Contains(cell))
        //    {
        //        cell = new NeoBloodVessel(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
        //        cell.neighborhood = Utils.GetMooreNeighbours3D(cell.pos, space);
        //        pos_neo_vessel_dict.Add(cell.pos, (NeoBloodVessel)cell);
        //    }
        //    else if (new_endothelial_cells.ContainsKey(cell.pos))
        //    {
        //        new_endothelial_cells.Remove(cell.pos);
        //        //new_endothelial_cellsRemove.Add(cell.pos);

        //        cell.behavior_state = CellState.EndothelialCell;
        //        model.endo_density_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] = model.initial_density_endo;

        //        //No se si deba agregarlo al tumor
        //        edothelial_cells.Add(cell);
        //        pos_cell_dict.Add(cell.pos, cell);
        //    }
        //}

        //public void UpdateEndothelialCell(Cell cell)
        //{
        //    if (new_vessel_cells.Contains(cell))
        //    {
        //        cell = new NeoBloodVessel(cell.pos, CellState.nothing, CellLocationState.GlialBasalLamina);
        //        //hay que eliminar la celula endotelial de la lista de celulas endoteliales
        //        //hay que adicionar los nuevos vasos a la lista de nuevos vasos
        //        neo_blood_vessel_list.Add((NeoBloodVessel)cell);
        //    }
        //}

        //public void UpdateCellWithNoOxigen(Cell cell)
        //{
        //    CellState cell_state = cell.behavior_state;
        //    if (cell_state != CellState.nothing && model.oxygen_matrix[cell.pos.X, cell.pos.Y, cell.pos.Z] <= 0.28)
        //    {
        //        if (cell_state == CellState.ProliferativeTumoralCell)
        //        {
        //            proliferation_cells.Remove(cell);
        //            cell.proliferation_age = -1;

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else if (cell_state == CellState.QuiescentTumorCell)
        //        {
        //            quiescent_cell_list.Remove(cell.pos);

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else if (cell_state == CellState.MigratoryTumorCell)
        //        {
        //            next_migratory_cells.Remove(cell.pos);
        //            migratory_cells_actual.Remove(cell.pos);

        //            cell.behavior_state = CellState.NecroticTumorCell;
        //            necrotic_cell_list.Add(cell);
        //        }
        //        else
        //        {

        //        }

        //    }
        //}
        #endregion

    }
}
