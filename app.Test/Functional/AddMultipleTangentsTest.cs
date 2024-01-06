namespace app.Test.Functional
{
    public class AddMultipleTangentsTest
    {
        /// <summary>
        /// Test to simulate user creating a Plot and adding several Tangents to it.
        /// </summary>
        [Test]
        public void TestAddMultipleTangents()
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

            double tangentXFirst = 2;
            plotViewModel.TangentX = tangentXFirst;

            double tangentXSecond = -2;
            double tangentXThird = 3;

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            plotViewModel.AddTangentCmd.Execute(null);

            plotViewModel.TangentX = tangentXSecond;
            plotViewModel.AddTangentCmd.Execute(null);

            plotViewModel.TangentX = tangentXThird;
            plotViewModel.AddTangentCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Empty, "Must not have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(4), "Must have 4 line series: function and its 3 tangents");
            Assert.That(plotViewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent, Is.Not.Null, "Selected plot's tangent must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent.X, Is.EqualTo(tangentXThird), "Selected plot's tangent must equal the last added tangent's x");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(1), "Plots collection count must be 1");
        }
    }
}
