using OxyPlot;

namespace app
{
    /// <summary>
    /// Interface for a service that provides functionality like creating and managing plots on the OxyPlot model.
    /// </summary>
    public interface IPlotter
    {
        public CreatePlotResult CreatePlot(PlotModel plotModel, string function, double xmin, double xmax, double xstep);
        public AddTangentResult AddTangent(PlotModel plotModel, double x, string function, double xmin, double xmax, double xstep);
        public CreatePlotResult CreatePlotFromExpression(PlotModel plotModel, Expression expression);
    }
}
