using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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
        private readonly IPlotEquationEvaluator _equationEvaluator;
        private PlotModel _oxyPlotModel;

        public PlotingService(IPlotEquationEvaluator equationEvaluator)
        {
            _equationEvaluator = equationEvaluator;
            _oxyPlotModel = new PlotModel();
        }

        public PlotModel OxyPlotModel => _oxyPlotModel;

        public PlotResult CreatePlot(string function, double xmin, double xmax, double xstep)
        {
            var result = _equationEvaluator.Evaluate(xmin, xmax, xstep, function);
            if (result.HasError)
            {
                return new PlotResult(null, result.Error);
            }

            LineSeries newSeries = NewLineSeries(result.Points);
            LineSeries customisedNewLineSeries = CustomiseLineSeries(newSeries, function);
            
            _oxyPlotModel.Series.Add(customisedNewLineSeries);
            SetupAxis(xmin, xmax);

            _oxyPlotModel.InvalidatePlot(true);

            return new PlotResult(_oxyPlotModel, null);
        }

        // Add Tangent at point X for a Function.
        // Draw in range xmin - xmax with step xstep.
        public PlotResult AddTangent(double x, string function, double xmin, double xmax, double xstep)
        {
            // Calculate the y-value of the function at x.
            var pointOfTangent = _equationEvaluator.Evaluate(x, x, 0.1, function);
            var y = pointOfTangent.Points[0][0];
            // Calculate the derivative - tangent slope at x.
            double slope = _equationEvaluator.TakeDerivative(x, function);

            // Calculate y-intercept of the tangent line:
            // y = mx + c => c = y - mx
            double yIntercept = y - slope * x;

            // Create the tangent line series.
            var tangentLineSeries = new LineSeries
            {
                Title = $"Tangent at x={x}",
                Color = OxyColors.Red, 
                LineStyle = LineStyle.Dash
            };

            // Draw the tangent line across the entire plot range.
            for (double i = xmin; i <= xmin; i += xstep)
            {
                double j = slope * i + yIntercept;
                tangentLineSeries.Points.Add(new DataPoint(i, j));
            }

            _oxyPlotModel.Series.Add(tangentLineSeries);
            _oxyPlotModel.InvalidatePlot(true);

            return new PlotResult(_oxyPlotModel, null);
        }

        /// <summary>
        /// Clears all plots from the OxyPlot PlotModel and from _equationColours collection.
        /// </summary>
        public void ClearPlots()
        {
            _oxyPlotModel.Series.Clear();
            _oxyPlotModel.InvalidatePlot(true);
        }

        private LineSeries CustomiseLineSeries(LineSeries lineSeries, string function)
        {
            lineSeries.Title = "y=" + function;

            return lineSeries;
        }

        private LineSeries NewLineSeries(double[][] points)
        {
            var lineSeries = new LineSeries();

            foreach (var point in points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            return lineSeries;
        }

        private void SetupAxis(double min, double max)
        {
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Bottom, Minimum = min, Maximum = max });
            _oxyPlotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Left, Minimum = min, Maximum = max });
        }
    }
}
