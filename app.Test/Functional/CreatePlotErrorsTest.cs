namespace app.Test.Functional
{
    public class CreatePlotErrorsTest
    {
        /// <summary>
        /// Test to simulate user inputing equation, xmin that is bigger than xmax, xstep and clicking Plot button.
        /// </summary>
        [Test]
        public void TestCreatePlot_InvalidInputError_XMinGreaterXMax()
        {
            // --------
            // ASSEMBLE
            // --------
            FSharpFunctionEvaluatiorWrapper evaluator = new FSharpFunctionEvaluatiorWrapper();
            PlotManager plotManager = new PlotManager(evaluator);
            TangentManager tangentManager = new TangentManager(evaluator);
            ValidationService validator = new ValidationService();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "x^2";
            double userSetXMin = 10, userSetXMax = -10, userSetXStep = 0.5;

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
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: XMin can't be greater than XMax"), "Errors don't match");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }

        /// <summary>
        /// Test to simulate user inputing equation, xmin, xmax, xstep that is 0 and clicking Plot button.
        /// </summary>
        [Test]
        public void TestCreatePlot_InvalidInputError_XStepIsZero()
        {
            // --------
            // ASSEMBLE
            // --------
            FSharpFunctionEvaluatiorWrapper evaluator = new FSharpFunctionEvaluatiorWrapper();
            PlotManager plotManager = new PlotManager(evaluator);
            TangentManager tangentManager = new TangentManager(evaluator);
            ValidationService validator = new ValidationService();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "x^2";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0;

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
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: XStep can't be 0"), "Errors don't match");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }

        /// <summary>
        /// Test to simulate user inputing equation with invalid token, xmin, xmax, xstep and clicking Plot button.
        /// </summary>
        [Test]
        public void TestCreatePlot_InvalidInputError_LexerError()
        {
            // --------
            // ASSEMBLE
            // --------
            FSharpFunctionEvaluatiorWrapper evaluator = new FSharpFunctionEvaluatiorWrapper();
            PlotManager plotManager = new PlotManager(evaluator);
            TangentManager tangentManager = new TangentManager(evaluator);
            ValidationService validator = new ValidationService();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "@";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.1;

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
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: Invalid Token at token position 1: @"), "Errors don't match");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }

        /// <summary>
        /// Test to simulate user inputing equation with an implicit multiplication that failes parsing, xmin, xmax, xstep and clicking Plot button.
        /// </summary>
        [Test]
        public void TestCreatePlot_InvalidInputError_ParserError()
        {
            // --------
            // ASSEMBLE
            // --------
            FSharpFunctionEvaluatiorWrapper evaluator = new FSharpFunctionEvaluatiorWrapper();
            PlotManager plotManager = new PlotManager(evaluator);
            TangentManager tangentManager = new TangentManager(evaluator);
            ValidationService validator = new ValidationService();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "2x";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.1;

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
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: Error while parsing: Unexpected token or end of expression"), "Errors don't match");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }

        /// <summary>
        /// Test to simulate user inputing equation with an assignemnt '=' that failes parsing, xmin, xmax, xstep and clicking Plot button.
        /// </summary>
        [Test]
        public void TestCreatePlot_InvalidInputError_ParserAssignmentError()
        {
            // --------
            // ASSEMBLE
            // --------
            FSharpFunctionEvaluatiorWrapper evaluator = new FSharpFunctionEvaluatiorWrapper();
            PlotManager plotManager = new PlotManager(evaluator);
            TangentManager tangentManager = new TangentManager(evaluator);
            ValidationService validator = new ValidationService();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "y=2";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.1;

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
            Assert.That(plotViewModel.Error, Is.Not.Empty, "Must have an error");
            Assert.That(plotViewModel.Error, Is.EqualTo("Error: Can't use assignment in plotting mode"), "Errors don't match");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Must have 0 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Null, "Selected plot must be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(0), "Plots collection count must be 0");
        }
    }
}
