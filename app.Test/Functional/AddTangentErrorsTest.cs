namespace app.Test.Functional
{
    public class AddTangentErrorsTest
    {
        /// <summary>
        /// Test to simulate user creating a Plot and adding a Tangent to it with Tangent's X outside of [xmin,xmax] range.
        /// </summary>
        [Test]
        public void TestAddTangent_XOutsideOFRange_Error()
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

            double tangentX = 200;
            plotViewModel.TangentX = tangentX;

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            plotViewModel.AddTangentCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: Tangent's X must be in the range [XMin, XMax]"), "Must have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(1), "Must have 1 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent, Is.Null, "Selected plot's tangent must be null");;
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(1), "Plots collection count must be 1");
        }

        /// <summary>
        /// Test to simulate user creating a Plot and adding a Tangent to it with Tangent's X set to zero.
        /// </summary>
        [Test]
        public void TestAddTangent_XSetToZero_Error()
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

            double tangentX = 0;
            plotViewModel.TangentX = tangentX;

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            plotViewModel.AddTangentCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: Tangent's X can't be 0"), "Must have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(1), "Must have 1 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent, Is.Null, "Selected plot's tangent must be null"); ;
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(1), "Plots collection count must be 1");
        }

        /// <summary>
        /// Test to simulate user adding a Tangent when there is no plot.
        /// </summary>
        [Test]
        public void TestAddTangent_SelectedPlotIsNull_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotViewModel plotViewModel = Utils.CreaePlotViewModel();

            double tangentX = 1;
            plotViewModel.TangentX = tangentX;

            // -----
            // ACT
            // -----
            plotViewModel.AddTangentCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("You must select the plot to add a tangent to it."), "Must have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }
    }
}
