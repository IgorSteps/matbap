using OxyPlot;

namespace app
{
    /// <summary>
    /// Interface implemented by the PlottingService, that provides functions to modify plot data.
    /// </summary>
    public interface IPlotter
    {
        public Error CreatePlot(PlotModel plotModel, string function, double xmin, double xmax, double xstep);

        public void ClearPlotModel(PlotModel plotModel);

        public Error AddTangent(PlotModel plotModel, double x, string function, double xmin, double xmax, double xstep);
    }
}
