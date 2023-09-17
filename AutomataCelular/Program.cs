using System;
using System.Collections.Generic;


namespace AutomataCelular
{
    class Program
    {
        public static int time = 0;
        public static int distance = 5;
        public static TumorCell tumor_stem_cell = null;
        public static List<StemCell> tumor_cell_list = new List<StemCell>();
        public static List<StemCell> stem_cell_list = new List<StemCell>();

        static Random rdm = new Random();
        public static int stem_cells_count = rdm.Next(0, 20);
        

        public static int limit_of_x = 50;
        public static int limit_of_y = 50;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static void StartCellularLifeInTheBrain()
        {
            //Crear la primera celula tumoral
            tumor_stem_cell = new TumorCell(new Pos(0,0));

            //Crear las celulas madres pluripotencial
            for (int i = 0; i < stem_cells_count; i++)
            {
                stem_cell_list.Add(new StemCell(GetRandomPosition()));
            }

            //HACER!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Crear las celulas astrocitos, pericitos, etc
        }

        public static void GetCellsThatSenseTheTumorSubstance()
        {
            for (int i = 0; i < stem_cell_list.Count; i++)
            {
                StemCell cell = stem_cell_list[i];
                if(EuclideanDistance(tumor_stem_cell.pos, cell.pos) <= distance)
                {
                    tumor_cell_list.Add(cell);
                }
            }
        }

        public static int EuclideanDistance(Pos pos_tumor_cell, Pos pos_stem_cell)
        {
            int d = (int)Math.Sqrt(Math.Pow(pos_stem_cell.X - pos_tumor_cell.X, 2) + Math.Pow(pos_stem_cell.Y - pos_tumor_cell.Y, 2));

            return d;
        }

        public static Pos GetRandomPosition()
        {
            //Random rdm = new Random();
            int x = rdm.Next(0 - limit_of_x, limit_of_x);
            int y = rdm.Next(0 - limit_of_y, limit_of_y);

            return new Pos(x, y);
        }
    }
}
