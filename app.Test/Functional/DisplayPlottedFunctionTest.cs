namespace app.Test.Functional
{
    public class DisplayPlottedFunctionTest
    {
        /// <summary>
        /// Test to simulate user adding a plot and it displaying its function in the "Plotted Functions" panel.
        /// </summary>
        [Test]
        public void TestDisplayPlottedFunction()
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

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(1), "There must be 1 plot");
            Assert.That(plotViewModel.Plots[0].Function, Is.EqualTo(userSetInputEquation), "Incorrect equation is stored in the Plots collection");
        }
    }
}
