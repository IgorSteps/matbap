using OxyPlot.Series;
using OxyPlot;

namespace app
{
    /// <summary>
    /// Interface for managing Plot Model from OxyPlot.
    /// </summary>
    public interface IOxyPlotModelManager
    {
        public void AddSeriesToPlotModel(PlotModel plotModel, Series series);
        public void SetupAxisOnPlotModel(PlotModel plotModel, double min, double max);
        public void RefreshPlotModel(PlotModel plotModal);
        public void ClearPlotModel(PlotModel plotModel);
    }
}
