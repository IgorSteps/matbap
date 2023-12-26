using OxyPlot;
using OxyPlot.Series;
using System.Linq;

namespace app
{
    /// <summary>
    /// Represents a plot with its function, range, step.
    /// </summary>
    public class Plot
    {
        public string Function { get; private set; }
        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double XStep { get; private set; }

        private readonly IPlotter _plotter;

        public Plot(string function, double xmin, double xmax, double xstep, IPlotter plotter)
        {
            Function = function;
            XMin = xmin;
            XMax = xmax;
            XStep = xstep;
            _plotter = plotter;
        }

        /// <summary>
        /// Initialises the plot by populating a PlotModel.
        /// </summary>
        public string Intilise(PlotModel plotModel)
        {
            var result = _plotter.CreatePlot(plotModel, Function, XMin, XMax, XStep);
            if (result.HasError)
            {
                return result.Error;
            }

            return null;
        }

        /// <summary>
        /// Adds a tangent line to the plot at a specified x-coordinate.
        /// </summary>
        public string AddTangent(PlotModel plotModel, double x)
        {
            var plotResult = _plotter.AddTangent(plotModel, x, Function, XMin, XMax, XStep);
            if (plotResult.HasError)
            {
                return plotResult.Error;
            }

            return null;
        }
    }
}
