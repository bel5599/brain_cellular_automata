using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AutomataCelularLogic
{
    public static class NetworkHistory
    {
        //general settings
        private static int _amount = 5;

        //directories
        private static string _networkFile = string.Empty;
        private static string _dataFolder = string.Empty;
        private static string _networkId = string.Empty;

        public static List<string> _generationsFiles = null;
        public static string _experimentFolder = string.Empty;
        //public static string _dataFolder = string.Empty;
        public static string _experimentID = string.Empty;


        //VARIABLES DEL MODELO DEL AUTOMATA
        //public static int cell_proliferation;
        public static int avascular_carrying_capacity = 6900;
        public static int vascular_carrying_capacity = 15000;
        public static double growth_rate = 2.95 * Math.Pow(10, -2);
        public static int initial_population = 5;

        //VARIABLES QUE TIENE QUE VER CON EL ENTORNO
        //public static int time = 0;
        public static int distance = 40;
        public static int limit_of_x = 25;
        public static int limit_of_y = 25;
        public static int limit_of_z = 25;

        //VARIABLES QUE TIENE QUE VER CON LAS CELULAS
        public static List<Cell> tumor_cell_list = new List<Cell>(); //CAMBIARLE EL NOMBRE A LA LISTA
        public static List<Cell> stem_cell_list = new List<Cell>();
        public static List<Cell> astrocyte_cell_list = new List<Cell>();
        public static List<Cell> endothelial_cell_list = new List<Cell>();
        public static List<Cell> neuron_cell_list = new List<Cell>();
        //public static List<Cell> cell_list = new List<Cell>();

        //VARIABLES RELACIONADAS CON LOS VASOS SANGUINEOS
        public static List<Cell> artery_list = new List<Cell>();



        public static Dictionary<Pos, Cell> pos_artery_dict = new Dictionary<Pos, Cell>();

        //LISTA RELACIONADAS CON LAS ACCIONES DE LAS CELULAS EN CADA INSTANTE DE TIEMPO
        public static Dictionary<Pos, Pos> contaminate_dict = new Dictionary<Pos, Pos>();
        public static Dictionary<Pos, Pos> division_dict = new Dictionary<Pos, Pos>();

        //VARIBALES QUE TIENEN QUE VER CON EL TUMOR
        public static int tumoral_cell_radio = 5;
        public static Cell tumor_stem_cell = null;



        public static CellularAutomaton ca;

        //VARIABLES QUE TIENEN QUE VER CON LAS ESFERAS TUMORALES
        public static List<Sphere> sphere_list = new List<Sphere>();
        public static Dictionary<Cell, Sphere> sphere_cell_dict = new Dictionary<Cell, Sphere>();
        //esta lista debe tener las celulas que no estan en ninguna esfera
        public static List<Cell> cells_without_sphere = new List<Cell>();
        public static List<Cell> cells_center_of_the_sphere_list = new List<Cell>();

        //public static Timer aTimer;

        //public static int stem_cells_count = rdm.Next(0, 15);


        public static int astrocytes_count = 20;
        public static int blood_vessels_count = 10;
        public static int neuron_count = 20;

        public static void Save()
        {
            
            //streamWriter.WriteLine("networkid = " + _networkId);
            //streamWriter.Write("\n");
            //streamWriter.WriteLine("[Settings]");
            //streamWriter.WriteLine("limit_of_x = " + limit_of_x);
            //streamWriter.WriteLine("limit_of_y = " + limit_of_y);
            //streamWriter.WriteLine("limit_of_z = " + limit_of_z);
            //streamWriter.Write("\n");

            _dataFolder = Directory.GetCurrentDirectory() + "\\Data";
            Directory.CreateDirectory(_dataFolder);

            DateTime time = DateTime.Now;
            string batchID = time.Year.ToString() + time.Month.ToString() + time.Day.ToString() + time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();
            _dataFolder = _dataFolder + "\\" + batchID;
            Directory.CreateDirectory(_dataFolder);

           

            for (int i = 0; i < 2; i++)
            {
                _experimentFolder = _dataFolder + "\\" + _experimentID;
                Directory.CreateDirectory(_experimentFolder);

                Utils.InitializeVariables();
                ca = new CellularAutomaton(limit_of_x, limit_of_y, limit_of_z, new ClassicProbability(), avascular_carrying_capacity, vascular_carrying_capacity, growth_rate, initial_population);

                _generationsFiles = new List<string>();

                string file = _experimentFolder + "\\" + "growth"+".generation ";
                StreamWriter verluhst_growth = new StreamWriter(file);

                for (int j = 0; j < 500; j++)
                {
                    string generationFile = _experimentFolder + "\\" + j + ".generation";
                    _generationsFiles.Add(generationFile);
                    StreamWriter generationWriter = new StreamWriter(generationFile);

                    verluhst_growth.WriteLine("Verhulst Growth");
                    verluhst_growth.Write("{0},{1};", ca.tumor.time, ca.proliferation_cells.Count);

                    generationWriter.WriteLine("[Generation Data]");
                    generationWriter.WriteLine(_experimentID + ":" + j);
                    generationWriter.WriteLine();


                    WriteInTheFile(generationWriter, ca);
                    generationWriter.Write("[EOF]");
                    generationWriter.Close();

                    
                    ca.Update();
                }

                verluhst_growth.Write("[EOF]");
                verluhst_growth.Close();


            }

        }
        public static void Create()
        {
            

            //string generationFile = _experimentFolder + "\\" + generation + ".generation";
            //_generationsFiles.Add(generationFile);
            //StreamWriter generationWriter = new StreamWriter(generationFile);

            //CreateExperimentFolder
            _experimentFolder = _dataFolder + "\\" + _experimentID;
            Directory.CreateDirectory(_experimentFolder);

            //CreateBatchFolder
            //string batchID = time.Year.ToString() + time.Month.ToString() + time.Day.ToString() + time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();
            //_dataFolder = _dataFolder + "\\" + batchID;
            //Directory.CreateDirectory(_dataFolder);

            //CreateDataFolder
            _dataFolder = Directory.GetCurrentDirectory() + "\\Data";
            Directory.CreateDirectory(_dataFolder);
        }

        public static void WriteInTheFile(StreamWriter streamWriter, CellularAutomaton ca)
        {

            Console.WriteLine("Cantidad de celulas proliferativas: {0}", ca.proliferation_cells.Count);

            streamWriter.WriteLine(ca.tumor.time);
            streamWriter.WriteLine();
            streamWriter.WriteLine("Tumoral_Stem_Cell: {0}, {1}, {2}", ca.tumor.ini_cell.pos.X, ca.tumor.ini_cell.pos.Y, ca.tumor.ini_cell.pos.Z);
            streamWriter.WriteLine();
            streamWriter.Write(CellState.ProliferativeTumoralCell+": ");
            foreach (var item in ca.proliferation_cells)
            {
                streamWriter.Write("{0},{1},{2},{3};", item.Key.pos.X, item.Key.pos.Y, item.Key.pos.Z, item.Key.proliferation_age);
            }
            streamWriter.WriteLine();

            streamWriter.Write("Migratory: ");
            foreach (var item in ca.migratory_cells_actual)
            {
                streamWriter.Write("{0},{1},{2};", item.Key.X, item.Key.Y, item.Key.Z);
            }
            streamWriter.WriteLine();

            //streamWriter.Write("Hipoxic: ");
            //foreach (var item in ca.quiescent_cell_list)
            //{
            //    streamWriter.Write("{0},{1},{2};", item.Key.X, item.Key.Y, item.Key.Z);
            //}
            //streamWriter.WriteLine();

            //streamWriter.Write("Necrotic: ");
            //foreach (var item in ca.necrotic_cell_list)
            //{
            //    streamWriter.Write("{0},{1},{2};", item.pos.X, item.pos.Y, item.pos.Z);
            //}
            //streamWriter.WriteLine();
            WriteCells(CellState.MigratoryTumorCell, ca.migratory_cells_actual, streamWriter);
            streamWriter.WriteLine();
            WriteCells(CellState.QuiescentTumorCell, ca.quiescent_cell_list, streamWriter);
            streamWriter.WriteLine();
            WriteCells(CellState.NecroticTumorCell, ca.necrotic_cell_list, streamWriter);
            streamWriter.WriteLine();
            WriteCells(CellState.StemCell, ca.stem_cell_list, streamWriter);
            streamWriter.WriteLine();
            WriteCells(CellState.Neuron, ca.neuron_list, streamWriter);
            streamWriter.WriteLine();
            WriteCells(CellState.Astrocyte, ca.astrocyte_list, streamWriter);
            streamWriter.WriteLine();
            //WriteCells(CellState.StemCell, ca.stem_cell_list, streamWriter);
            //streamWriter.WriteLine();
            WriteCells(CellState.EndothelialCell, ca.endothelial_cells, streamWriter);
            streamWriter.WriteLine();

            streamWriter.WriteLine("BloodVessels: ");
            //foreach (var item in ca.pos_artery_dict)
            //{
            //    streamWriter.Write("{0}, {1}, {2};", item.Key.X, item.Key.Y, item.Key.Z);
            //}

            WriteBloodVesselsSegment(streamWriter, ca.vessel_segment_list);

            streamWriter.WriteLine("NeoBloodVessels: ");
            WriteBloodVesselsSegment(streamWriter, ca.neo_vessel_segment_list);

            
            //foreach (var item in ca.pos_neo_vessel_dict)
            //{
            //    streamWriter.Write("{0}, {1}, {2};", item.Key.X, item.Key.Y, item.Key.Z);
            //}

            WriteConcentrationMatrix(streamWriter,"Oxygen_Matrix", ca.model.oxygen_matrix);
            WriteConcentrationMatrix(streamWriter, "Density_Matrix", ca.model.density_matrix);
            WriteConcentrationMatrix(streamWriter, "MDE_Matrix", ca.model.mde);
            WriteConcentrationMatrix(streamWriter, "VEGF_Matrix", ca.model.vegf_conc_matrix);
            WriteConcentrationMatrix(streamWriter, "EndoDensity_Matrix", ca.model.endo_density_matrix);
        }

        public static void WriteCells(CellState state, Dictionary<Pos,Cell> cell_list, StreamWriter streamWriter)
        {
            streamWriter.Write(state + ": ");
            foreach (var item in cell_list)
            {
                streamWriter.Write("{0},{1},{2};", item.Key.X, item.Key.Y, item.Key.Z);
            }
            streamWriter.WriteLine();
        }

        public static void WriteCells(CellState state, List<Cell> cell_list, StreamWriter streamWriter)
        {
            streamWriter.Write(state + ": ");
            foreach (var item in cell_list)
            {
                streamWriter.Write("{0},{1},{2};", item.pos.X, item.pos.Y, item.pos.Z);
            }
            streamWriter.WriteLine();
        }

        public static void WriteConcentrationMatrix(StreamWriter streamWriter, string concentration_name, double[,,] conc_matrix)
        {
            streamWriter.WriteLine(concentration_name);

            for (int i = 0; i < conc_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < conc_matrix.GetLength(1); j++)
                {
                    for (int k = 0; k < conc_matrix.GetLength(2); k++)
                    {
                        streamWriter.Write("{0};", conc_matrix[i, j, k]);
                    }
                    streamWriter.WriteLine();
                }
                streamWriter.WriteLine();
            }
        }

        public static void WriteBloodVesselsSegment(StreamWriter streamWriter, List<BloodVesselSegment> vesselSegments)
        {
            foreach (var item in vesselSegments)
            {
                streamWriter.Write("{0} {1} {2} {3};", item.blood_vessel1.pos.X, item.blood_vessel1.pos.Y, item.blood_vessel1.pos.Z, item.blood_vessel1.maturation_time);
                streamWriter.Write("{0} {1} {2} {3};", item.blood_vessel2.pos.X, item.blood_vessel2.pos.Y, item.blood_vessel2.pos.Z, item.blood_vessel2.maturation_time);

                streamWriter.Write("{0}; {1}; {2};", item.radius, item.mean_length, item.pressure_collapse);
                streamWriter.Write("{0}; {1}; {2};", item.intravascular_pressure, item.interstitial_pressure, item.hydraulic_permeability);
                streamWriter.Write("{0}; {1};", item.order, item.radio_clasf);

                streamWriter.WriteLine();
            }
        }

        private static void CreateNetworkFile()
        {
            DateTime time = DateTime.Now;
            string processedTime = time.Year.ToString() + time.Month.ToString() + time.Day.ToString() + time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString();

            _networkFile = _dataFolder + "\\" + processedTime + ".network";

            Console.WriteLine("main: test file created - with path: \"" + _networkFile);
            _networkId = processedTime;
        }

        public static void WriteOnFile()
        {

        }
        public static void SaveHistory()
        {
            //int time_start = Environment.TickCount;
            //Notification.PrintProgramLabel();
            //CreateDataFolder();
            //// ReadSettings();

            //List<TVertex> template = MF.GetRegularNeighboursTemplate(_r);
            //List<TVertex> ftemplate = MF.FilterRegularNeighboursTemplate(template);
            //_networkSettings = new NetworkSettings(_gridSizeX, _gridSizeY, _gridDivision, _p, _r, false, _periodic);

            //PrintSettings(template, ftemplate);

            //for (int i = 0; i < _amount; i++)
            //{
            //    Console.WriteLine("main: creating network number: " + (i + 1));
            //    CreateNetworkFile();
            //    Thread.Sleep(1000);
            //    StreamWriter streamWriter = new StreamWriter(_networkFile);
            //    WriteFileOutline(template, ftemplate, streamWriter);

            //    BuildNetwork();

            //    WriteFileData(streamWriter);
            //    _wattsNetwork.Dispose();
            //    _wattsNetwork = null;
            //    streamWriter.Write("\n");
            //    streamWriter.Write("[EOF]");
            //    streamWriter.Close();
            //    streamWriter.Dispose();
            //    Console.WriteLine("main: finished network number: " + (i + 1) + "\n");
            //}
            //int time_elapsedmiliseconds = Environment.TickCount - time_start;
            //string formatted_time = Notification.TimeStamp(time_elapsedmiliseconds);
            //Console.WriteLine("main: program finished" + formatted_time);
            //Console.ReadLine();
        }
        private static void WriteFileData(StreamWriter streamWriter)
        {
            //Console.WriteLine("main: writting network data to file...");
            //int timeStart = Environment.TickCount;
            //streamWriter.WriteLine("[Vertexs]");
            ////int count = _wattsNetwork.Vertices.Count;
            ////dynamic ls = _wattsNetwork.Vertices;
            //bool written = false;
            //Console.WriteLine("main: writting vertexs...");

            //for (int i = 0; i < count; i++)
            //{
            //    Notification.CompletionNotification(i, count, ref written, "");
            //    streamWriter.WriteLine(ls[i]);
            //}

            //Notification.FinalCompletionNotification("");
            //streamWriter.Write("\n");
            //streamWriter.WriteLine("[Edges]");
            //Console.WriteLine("main: writting edges...");
            //ls = _wattsNetwork.Edges;
            //written = false;
            //count = _wattsNetwork.Edges.Count;

            //for (int i = 0; i < count; i++)
            //{
            //    Notification.CompletionNotification(i, count, ref written, "");
            //    streamWriter.WriteLine(ls[i]);
            //}

            //Notification.FinalCompletionNotification("");
            //ls.Clear();
            //int time_elapsedmiliseconds = Environment.TickCount - timeStart;
            //string formatted_time = Notification.TimeStamp(time_elapsedmiliseconds);
            //Console.WriteLine("main: finished writting network data to file" + formatted_time);
        }
        //private static void BuildNetwork()
        //{
        //    Console.WriteLine("main: creating network...");
        //    int timeStart = Environment.TickCount;

        //    _wattsNetwork = new SmallWorldNetwork(_networkSettings);

        //    int timeElapsedMiliseconds = Environment.TickCount - timeStart;
        //    string formattedTime = Notification.TimeStamp(timeElapsedMiliseconds);
        //    Console.WriteLine("main: finished creating network" + formattedTime);
        //}
        //private static void WriteFileOutline(List<TVertex> template, List<TVertex> ftemplate, StreamWriter streamWriter)
        //{
        //    Console.WriteLine("main: writting file outline...");
        //    streamWriter.WriteLine("[Radiant Network File]");
        //    streamWriter.WriteLine("networkid = " + _networkId);
        //    streamWriter.Write("\n");
        //    streamWriter.WriteLine("[Settings]");
        //    streamWriter.WriteLine("grid_size_x = " + _gridSizeX);
        //    streamWriter.WriteLine("grid_size_y = " + _gridSizeY);
        //    streamWriter.WriteLine("grid_division = " + _gridDivision);
        //    streamWriter.WriteLine("r = " + _r);
        //    streamWriter.WriteLine("p = " + _p);
        //    streamWriter.WriteLine("periodic = " + _periodic);
        //    streamWriter.Write("\n");
        //    Console.WriteLine("main: finished writting file outline");
        //}

        //private static void PrintSettings(List<TVertex> template, List<TVertex> ftemplate)
        //{
        //    Console.WriteLine("main: setttings:");
        //    Console.WriteLine("--grid_size_x = " + _gridSizeX);
        //    Console.WriteLine("--grid_size_y = " + _gridSizeY);
        //    Console.WriteLine("--grid_division = " + _gridDivision);
        //    Console.WriteLine("--r = " + _r);
        //    Console.WriteLine("--p = " + _p);
        //    Console.WriteLine("--amount = " + _amount);
        //    Console.WriteLine("--periodic = " + _periodic);
        //    Console.WriteLine("--|template|=" + template.Count);
        //    Console.WriteLine("--|filteredtemplate|=" + ftemplate.Count + "\n");
        //}
        //private static void ReadSettings()
        //{
        //    Console.WriteLine("main: reading settings file...");
        //    string path_settings = Directory.GetCurrentDirectory() + "\\Settings.txt";
        //    if (!File.Exists(path_settings))
        //        path_settings = Directory.GetCurrentDirectory() + "\\..\\..\\Settings.txt";
        //    if (!File.Exists(path_settings))
        //    {
        //        Console.WriteLine("main: failed reading settings file");
        //        Console.WriteLine("main: using default parameters");
        //        return;
        //    }
        //    string[] textbody = File.ReadAllLines(path_settings);
        //    int linenumber = 0;
        //    if (textbody[linenumber] == "[NextGen Network Generator Settings]")
        //    {
        //        linenumber++;
        //        while (linenumber < textbody.Length)
        //        {
        //            string removed_blank_spaces = MF.RemoveBlankSpaces(textbody[linenumber]);
        //            string[] splitted = removed_blank_spaces.Split('=');
        //            switch (splitted[0])
        //            {
        //                case "amount":
        //                    _amount = int.Parse(splitted[1]);
        //                    break;
        //                case "grid_size_x":
        //                    _gridSizeX = int.Parse(splitted[1]);
        //                    break;
        //                case "grid_size_y":
        //                    _gridSizeY = int.Parse(splitted[1]);
        //                    break;
        //                case "grid_division":
        //                    _gridDivision = int.Parse(splitted[1]);
        //                    break;
        //                case "p":
        //                    _p = double.Parse(splitted[1]);
        //                    break;
        //                case "r":
        //                    _r = double.Parse(splitted[1]);
        //                    break;
        //                case "periodic":
        //                    _periodic = bool.Parse(splitted[1]);
        //                    break;
        //                default:
        //                    Console.WriteLine("main: failed reading settings file");
        //                    Console.WriteLine("main: using default parameters");
        //                    return;
        //            }
        //            linenumber++;
        //        }
        //        Console.WriteLine("main: finished reading settings file");
        //    }
        //    else
        //    {
        //        Console.WriteLine("main: failed reading settings file");
        //        Console.WriteLine("main: using default parameters");
        //    }
        //}

        private static void CreateDataFolder()
        {
            _dataFolder = Directory.GetCurrentDirectory() + "\\Data";
            Directory.CreateDirectory(_dataFolder);
            Console.WriteLine("main: data folder created - with path: \"" + _dataFolder + "\"\n");
        }
    }
}
