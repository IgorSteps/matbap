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

        private readonly IPlotEquationEvaluator _equationEvaluator;
        private Dictionary<string, LineSeries> Plots { get; set; }
        private PlotModel _oxyPlotModel;

        public PlotingService(IPlotEquationEvaluator equationEvaluator)
        {
            _equationEvaluator = equationEvaluator;
            Plots = new Dictionary<string, LineSeries>();
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

            LineSeries newSeries = AddNewLineSeries(result.Points);
            Plots[function] = newSeries;

            _oxyPlotModel.Series.Add(newSeries);
            _oxyPlotModel.InvalidatePlot(true);

            return new PlotResult(_oxyPlotModel, null);
        }

        /// <summary>
        /// Clears all plots from the OxyPlot PlotModel and from plots Dictionary.
        /// </summary>
        public void ClearPlots()
        {
            Plots.Clear();
            _oxyPlotModel.Series.Clear();
            _oxyPlotModel.InvalidatePlot(true);
        }

        private LineSeries AddNewLineSeries(double[][] points)
        {
            var lineSeries = new LineSeries();

            foreach (var point in points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            return lineSeries;
        }

        private void SetupAxis()
        {
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Bottom, Minimum = AxisMin, Maximum = AxisMax });
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Left, Minimum = AxisMin, Maximum = AxisMax });
        }
    }
}
