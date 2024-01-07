using FSharpPoints = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;

namespace app
{
    /// <summary>
    /// Interface for managing plots.
    /// </summary>
    public interface IPlotManager
    {
        public Plot CreatePlot(string function, double xmin, double xmax, double xstep);
        public GetLineSeriesResult GetLineSeriesForPlot(Plot plot);
    }
}
