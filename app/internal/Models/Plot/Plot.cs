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
        public Error Intilise(PlotModel plotModel)
        {
            Error err = _plotter.CreatePlot(plotModel, Function, XMin, XMax, XStep);
            if (err != null)
            {
                return err;
            }

            return null;
        }

        /// <summary>
        /// Adds a tangent line to the plot at a specified x-coordinate on a Plot Model.
        /// </summary>
        public Error AddTangent(PlotModel plotModel, double x)
        {
            Error err = _plotter.AddTangent(plotModel, x, Function, XMin, XMax, XStep);
            if (err != null)
            {
                return err;
            }

            return null;
        }
    }
}
