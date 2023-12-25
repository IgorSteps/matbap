using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public struct PlotResult
    {
        public PlotModel OxyPlotModel { get; set; }
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);

        public PlotResult(PlotModel oxyPlotModel, string error)
        {
            OxyPlotModel = oxyPlotModel;
            Error = error;
        }

    }

    public class PlotingService : IPlotter
    {
        const double AxisMin = 0;
        const double AxisMax = 10;

        /// <summary>
        /// Represents an OxyPlot plot.
        /// </summary>
        private PlotModel _oxyPlotModel;
        private readonly IPlotEquationEvaluator _equationEvaluator;


        public PlotingService(IPlotEquationEvaluator equationEvaluator)
        {
            _equationEvaluator = equationEvaluator;
            _oxyPlotModel = new PlotModel();

            SetupAxis();
        }

        public PlotResult CreatePlot(string function, double xmin, double xmax, double xstep)
        {
            var result = _equationEvaluator.Evaluate(xmin, xmax, xstep, function);
            if (result.HasError)
            {
                return new PlotResult(null, result.Error);
            }


            return new PlotResult(UpdatePlotModelWithPoints(result.Points), null);
        }

        private PlotModel UpdatePlotModelWithPoints(double[][] points)
        {
            _oxyPlotModel.Series.Clear();
            var lineSeries = new LineSeries();

            foreach (var point in points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            _oxyPlotModel.Series.Add(lineSeries);

            // Refresh the plot with new data.
            _oxyPlotModel.InvalidatePlot(true);

            return _oxyPlotModel;
        }

        private void SetupAxis()
        {
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Bottom, Minimum = AxisMin, Maximum = AxisMax });
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Left, Minimum = AxisMin, Maximum = AxisMax });
        }
    }
}
