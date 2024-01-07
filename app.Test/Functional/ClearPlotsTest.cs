namespace app.Test.Functional
{
    public class ClearPlotTest
    {
        /// <summary>
        /// Test to simulate user adding a plot and then clicking the Clear button.
        /// </summary>
        [Test]
        public void TestClearPlot()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotViewModel plotViewModel = Utils.CreaePlotViewModel();

            string userSetInputEquation = "x^2";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.5;

            plotViewModel.InputEquation = userSetInputEquation;
            plotViewModel.XMinimum = userSetXMin;
            plotViewModel.XMaximum = userSetXMax;
            plotViewModel.XStep = userSetXStep;

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            plotViewModel.ClearCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Empty, "Must not have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }
    }
}
