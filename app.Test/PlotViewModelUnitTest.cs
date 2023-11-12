
using OxyPlot.Series;

namespace app.Test
{
    internal class PlotViewModelUnitTest
    {

        [Test]
        public void PlotViewModel_Updates_WhenSlopeChanges()
        {
            // --------
            // ASSEMBLE
            // --------
            var viewModel = new PlotViewModel();
            var expectedSlope = 2.0;

            // ---
            // ACT
            // ---
            viewModel.Slope = expectedSlope;
            LineSeries lineSeries = viewModel.PlotModel.Series[0] as LineSeries;

            // ------
            // ASSERT
            // ------
            Assert.That(lineSeries, Is.Not.Null, "line series shouldn't be null");

            // Points[1].Y = -18
            // Points[1].X = -9
            // Slope = y/x =-18/-9 = 2
            Assert.That(lineSeries.Points[1].Y / lineSeries.Points[1].X, Is.EqualTo(expectedSlope), "slope should be equal to 2");
        }

        [Test]
        public void PlotViewModel_ShouldUpdate_WhenInterceptChanges()
        {
            // --------
            // ASSEMBLE
            // --------
            var viewModel = new PlotViewModel();
            var slope = viewModel.Slope; // Default value is 1.
            var expectedIntercept = 10.0;

            // ---
            // ACT
            // ---
            viewModel.Intercept = expectedIntercept;
            LineSeries lineSeries = viewModel.PlotModel.Series[0] as LineSeries;

            // ------
            // ASSERT
            // ------
            Assert.That(lineSeries, Is.Not.Null, "line series shouldn't be null");
            
            // From the line equation y=ax+b, where b(intercept) = y-ax.
            var actualIntercept = lineSeries.Points[0].Y - slope * lineSeries.Points[0].X;
            Assert.That(actualIntercept, Is.EqualTo(expectedIntercept), "intercept  should be equal to 10");
        }
    }
}
