using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace app.Test.Unit
{
    public class OxyPlotModelManager_test
    {
        [Test]
        public void Test_OxyPlotModelManager_AddSeriesToPlotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            OxyPlotModelManager modelManager = new OxyPlotModelManager();
            PlotModel plotModel = new PlotModel();
            LineSeries lineSeries = new LineSeries();

            // --------
            // ACT
            // --------
            modelManager.AddSeriesToPlotModel(plotModel, lineSeries);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Series.Count, Is.EqualTo(1), "PlotModel should have a Line series");
        }

        [Test]
        public void Test_OxyPlotModelManager_SetupAxisOnPlotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            OxyPlotModelManager modelManager = new OxyPlotModelManager();
            PlotModel plotModel = new PlotModel();
            LineSeries lineSeries = new LineSeries();
            double xmin = 1, xmax = 2;

            // --------
            // ACT
            // --------
            modelManager.SetupAxisOnPlotModel(plotModel, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Axes.Count, Is.EqualTo(2), "PlotModel should have 2 axes");
            foreach (var axis in plotModel.Axes)
            {
                Assert.That(axis.Minimum, Is.EqualTo(xmin), "Min is not right for the axis");
                Assert.That(axis.Maximum, Is.EqualTo(xmax), "Max is not right for the axis");
            }
        }

        [Test]
        public void Test_OxyPlotModelManager_ClearPlotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            OxyPlotModelManager modelManager = new OxyPlotModelManager();
            PlotModel plotModel = new PlotModel();
            LineSeries lineSeries = new LineSeries();

            plotModel.Series.Add(lineSeries);
            // --------
            // ACT
            // --------
            modelManager.ClearPlotModel(plotModel);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "PlotModel should've been cleared");
        }
    }
}
