
using OxyPlot.Series;
using System.Collections.ObjectModel;

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

        [Test]
        public void PlotViewModel_HornerMethod_Returns_Correct_Result()
        {
            // --------
            // ASSEMBLE
            // --------
            var viewModel = new PlotViewModel();
            viewModel.PolynomialCoefficients = new ObservableCollection<double> { 1, -2, 1 }; // x^2 - 2x + 1

            // ---
            // ACT
            // ---
            double resultForXEquals2 = viewModel.Horner(2); // Should be 2^2 - 2*2 + 1 = 1

            // ------
            // ASSERT
            // ------
            Assert.That(resultForXEquals2, Is.EqualTo(1));
        }

        [Test]
        public void PlotViewModel_UpdatePlot_Creates_PolynomialSeries_With_Correct_DataPoints()
        {
            // --------
            // ASSEMBLE
            // --------
            var viewModel = new PlotViewModel();
            viewModel.PolynomialCoefficients = new ObservableCollection<double> { 1, -2, 1 }; // x^2 - 2x + 1
            viewModel.XMinimum = -1;
            viewModel.XMaximum = 1;
            viewModel.XStep = 0.5;

            // ---
            // ACT
            // ---
            viewModel.UpdatePlot();
            var series = viewModel.PlotModel.Series[1] as LineSeries; // It will be the second series after the line plot.

            // ------
            // ASSERT
            // ------
            Assert.IsNotNull(series, "series can't be null");
            Assert.IsNotEmpty(series.Points, "series's points list can't be empty");
            Assert.That(series.Points.Count, Is.EqualTo(5), "incorrect number of points in a series");
        }
    }
}
