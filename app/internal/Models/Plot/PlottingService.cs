using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.DirectoryServices.ActiveDirectory;

namespace app
{
    public struct PlotResult
    {
        public PlotModel OxyPlotModel { get; set; }
        public string Error { get; set; }
        public readonly bool HasError => !string.IsNullOrEmpty(Error);

        public PlotResult(PlotModel oxyPlotModel, string error)
        {
            OxyPlotModel = oxyPlotModel;
            Error = error;
        }
    }

    /// <summary>
    /// PlottingService provides functionality for creating and manipulating plot data.
    /// </summary>
    public class PlottingService : IPlotter
    {
        private readonly IPlotEquationEvaluator _equationEvaluator;
        private readonly IValidator _validator;

        public PlottingService(IPlotEquationEvaluator equationEvaluator, IValidator validator)
        {
            _equationEvaluator = equationEvaluator;
           _validator = validator;
        }

        /// <summary>
        /// Creates a plot on the given PlotModel based on the given function, range and step.
        /// </summary>
        public PlotResult CreatePlot(PlotModel plotModel, string function, double xmin, double xmax, double xstep)
        {
            string err = _validator.ValidatePlotInput(xmin, xmax, xstep);
            if (err != null)
            {
                return new PlotResult(null, err);
            }

            var result = EvaluateFunction(function, xmin, xmax, xstep);
            if (result.HasError)
            {
                return new PlotResult(null, result.Error);
            }

            LineSeries newSeries = CreateLineSeries(result.Points, "y = "+function, LineStyle.Solid);
            AddSeriesToPlotModel(plotModel, newSeries);
            SetupAxisOnPlotModel(plotModel, xmin, xmax);

            return new PlotResult(plotModel, null);
        }

        /// <summary>
        /// Add Tangent at point x for a function.
        /// </summary>
        public PlotResult AddTangent(PlotModel plotModel, double x, string function, double xmin, double xmax, double xstep)
        {
            string err = _validator.ValidateAddTangentInput(x, xmin, xmax, xstep);
            if (err != null)
            {
                return new PlotResult(null, err);
            }

            // Hacky way of doing it for now, Evaluate() will return only 1 point
            // if xmin = xmax and xstep > 0.
            var result = EvaluateFunction(function, x, x, x);
            if (result.HasError)
            {
                return new PlotResult(null, result.Error);
            }

            // Calculate y-intercept for tangent,
            double y = result.Points[0][1];
            double slope = _equationEvaluator.TakeDerivative(x, function);
            double yIntercept = y - slope * x;

            // Build the string of the tangent function to pass to evaluator.
            string tangentFunction = $"{slope} * x + {yIntercept}";
            var tangentEvalResult = EvaluateFunction(tangentFunction, xmin, xmax, xstep);
            if (tangentEvalResult.HasError)
            {
                return new PlotResult(null, tangentEvalResult.Error);
            }

            LineSeries tangentLineSeries = CreateLineSeries(tangentEvalResult.Points, "Tangent at x = " + x, LineStyle.Dash);
            AddSeriesToPlotModel(plotModel, tangentLineSeries);
            SetupAxisOnPlotModel(plotModel, xmin, xmax);

            return new PlotResult(plotModel, null);
        }

        /// <summary>
        /// Clears series data from PlotModel.
        /// </summary>
        public void ClearPlotModel(PlotModel plotModel)
        {
            plotModel.Series.Clear();
        }

        private EvaluationResult EvaluateFunction(string function, double xmin, double xmax, double xstep)
        {
            var result = _equationEvaluator.Evaluate(xmin, xmax, xstep, function);
            if (result.HasError)
            {
                return new EvaluationResult(null, result.Error);
            }
            else
            {
                return new EvaluationResult(result.Points, null);
            }
        }

        private LineSeries CreateLineSeries(double[][] points, string title, LineStyle lineStyle)
        {
            var lineSeries = new LineSeries { Title = title, LineStyle = lineStyle };
            foreach (var point in points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }
            return lineSeries;
        }

        private void AddSeriesToPlotModel(PlotModel plotModel, Series series)
        {
            plotModel.Series.Add(series);
        }

        private void SetupAxisOnPlotModel(PlotModel plotModel, double min, double max)
        {
            plotModel.Axes.Clear();
            plotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Bottom, Minimum = min, Maximum = max });
            plotModel.Axes.Add(new LinearAxis{ Position = AxisPosition.Left, Minimum = min, Maximum = max });
        }
    }
}
