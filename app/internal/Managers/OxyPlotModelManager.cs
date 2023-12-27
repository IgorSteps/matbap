using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;

namespace app
{
    /// <summary>
    /// Manages the PlotModel from OxyPlot.
    /// </summary>
    public class OxyPlotModelManager : IOxyPlotModelManager
    {
        public void AddSeriesToPlotModel(PlotModel plotModel, Series series)
        {
            plotModel.Series.Add(series);
        }

        public void SetupAxisOnPlotModel(PlotModel plotModel, double min, double max)
        {
            plotModel.Axes.Clear();
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = min, Maximum = max });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = min, Maximum = max });
        }

        /// <summary>
        /// Refresh OxyPlot's Plot Modal.
        /// </summary>
        public void RefreshPlotModel(PlotModel plotModal)
        {
            plotModal.InvalidatePlot(true);
        }

        /// <summary>
        /// Clears series from Oxyplot's Plot Model.
        /// </summary>
        public void ClearPlotModel(PlotModel plotModel)
        {
            plotModel.Series.Clear();
        }
    }
}
