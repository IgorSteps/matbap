using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace app
{
    /// <summary>
    /// Handles operations related to trapeziums: creating and getting OxyPlot line series.
    /// </summary>
    public class TrapeziumManager : ITrapeziumManager
    {
        public List<Trapezium> Trapeziums { get; private set; }

        public TrapeziumManager()
        {
            Trapeziums = new List<Trapezium>();
        }

        /// <summary>
        /// Create Trapezium and add to list of all other trapeziums
        /// </summary>
        public Trapezium CreateTrapezium(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            Trapezium trapezium = new Trapezium(x1, y1, x2, y2, x3, y3, x4, y4);
            Trapeziums.Add(trapezium);
            return trapezium;
        }

        /// <summary>
        /// Gets LineSeries for every trapezium.
        /// </summary>
        public List<LineSeries> GetAllTrapeziumSeries()
        {
            var seriesList = new List<LineSeries>();
            foreach (var trapezium in Trapeziums)
            {
                var trapeziumSeries = new LineSeries { Title = $"Trapezium {seriesList.Count + 1}", Color = OxyColor.FromRgb(0, 0, 125) };

                // The order of vertices matters to plot correctly.
                trapeziumSeries.Points.Add(new DataPoint(trapezium.X1, trapezium.Y1));
                trapeziumSeries.Points.Add(new DataPoint(trapezium.X2, trapezium.Y2));
                trapeziumSeries.Points.Add(new DataPoint(trapezium.X3, trapezium.Y3));
                trapeziumSeries.Points.Add(new DataPoint(trapezium.X4, trapezium.Y4));
                // Closing the loop with fifth point.
                trapeziumSeries.Points.Add(new DataPoint(trapezium.X1, trapezium.Y1)); 

                seriesList.Add(trapeziumSeries);
            }

            return seriesList;
        }

    }
}
