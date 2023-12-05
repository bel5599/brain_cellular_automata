using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plotly.NET;

namespace AutomataCelularLogic
{
    class ExperimentalResultsGraph
    {
        public static void RandomPointScatter()
        {
            var random = new Random();
            //var x = DataGen.RandomNormal(random, 1000, 0, 1);
            //var y = DataGen.RandomNormal(random, 1000, 0, 1);
            //var z = DataGen.RandomNormal(random, 1000, 0, 1);

            //var trace = Chart.Scatter3d(x, y, z, Mode.Markers);
            //var chart = Chart.Plot(trace);
            //chart.Show();
        }
        
        public static void Histogram()
        {
            var random = new Random();
            //var data = DataGen.RandomNormal(random, 1000, 0, 1);

            //var trace = Chart.Histogram(data);
            //var chart = Chart.Plot(trace);
            //chart.Show();
        }

        public static void Heatmap()
        {
            var random = new Random();
            //var data = DataGen.RandomNormal(random, 1000, 0, 1);

            //var trace = Chart.Heatmap(data);
            //var chart = Chart.Plot(trace);
            //chart.Show();
        }

    }
}
