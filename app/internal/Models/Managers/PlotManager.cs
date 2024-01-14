using OxyPlot.Series;
using OxyPlot;
using FSharpPoints = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;


namespace app
{
    public struct GetLineSeriesResult
    {
        public LineSeries LineSeries { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public GetLineSeriesResult(LineSeries lineSeries, Error error)
        {
            LineSeries = lineSeries;
            Error = error;
        }
    }
    /// <summary>
    /// Handles creation and visusalisation of Plots.
    /// </summary>
    public class PlotManager : IPlotManager
    {
        private readonly IFSharpFunctionEvaluatorWrapper _functionEvaluator;
        public PlotManager(IFSharpFunctionEvaluatorWrapper functionEvaluator)
        {
            _functionEvaluator = functionEvaluator;
        }

        public Plot CreatePlot(string function, double xmin, double xmax, double xstep)
        {
            return new Plot(function, xmin, xmax, xstep);
        }

        public GetLineSeriesResult GetLineSeriesForPlot(Plot plot)
        {
            var result = _functionEvaluator.Evaluate(plot.Function, plot.XMin, plot.XMax, plot.XStep);
            if (result.HasError)
            {
                return new GetLineSeriesResult(null, result.Error);
            }

            LineSeries lineSeries = new LineSeries
            {
                Title = plot.Function
            };

            foreach (var point in result.Points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            return new GetLineSeriesResult(lineSeries, null);
        }
    }
}
