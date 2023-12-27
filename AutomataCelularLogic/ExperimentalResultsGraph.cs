using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plotly.NET;
using ScottPlot;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

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


        public static void HeatMap(List<Pos> positions)
        {
            //var points = new ChartValues<ScatterPoint>();
            //foreach (var pos in positions)
            //{
            //    points.Add(new ScatterPoint(pos.X, pos.Y, pos.Z));
            //}

            //// Crear un mapa de calor
            //var scatterSeries = new ScatterSeries
            //{
            //    Values = points,
            //    MinPointShapeDiameter = 1,
            //    MaxPointShapeDiameter = 5
            //};

            //CartesianChart cartesianChart = new CartesianChart
            //{
            //    Series = new SeriesCollection { scatterSeries },
            //    Zoom = ZoomingOptions.Xy
            //};

            //// Agregar el gráfico a la ventana
            //this.Content = cartesianChart;
        }

        public static void LogisticGrowth(double[] time, double[] cells)
        {
            //var plt = new ScottPlot.Plot(600, 400);
            //plt.AddScatter(time, cells);
            //plt.Title("Crecimiento de un Glioblastoma Multiforme");
            //plt.XLabel("Tiempo");
            //plt.YLabel("Número de células");
            //plt.SaveFig("grafico.png");
        }
    }
}
