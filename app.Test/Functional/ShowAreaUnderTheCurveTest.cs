
namespace app.Test.Functional
{
    public class ShowAreaUnderTheCurveTest
    {
        [Test]
        public void Test_ShowAreaUnderCurve_Successfully()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "5";
            var viewModel = Utils.CreaePlotViewModel();
            viewModel.InputEquation = expression;
            viewModel.XMinimum = -10;
            viewModel.XMaximum = 10;
            viewModel.XStep = 0.1;
            viewModel.IntegrationStep = 1;

            // --------
            // ACT
            // --------
            viewModel.PlotCmd.Execute(null);
            viewModel.ShowAreaUnderCurveCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(viewModel.Error, Is.EqualTo("Area under the curve = 100"), "Must display area in the error box");
            Assert.That(viewModel.OxyPlotModel.Series.Count, Is.EqualTo(21), "Must have 21 line series, 20 trapeziums and 1 function plot");
            Assert.That(viewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(viewModel.Plots.Count, Is.EqualTo(1), "Plots collection count must be 1");
        }
    }
}
