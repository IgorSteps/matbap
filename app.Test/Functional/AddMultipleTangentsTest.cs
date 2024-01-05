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
            // F# wrappers.
            Engine.EvaluatorWrapper engineEvaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            FSharpFunctionEvaluatiorWrapper functionEvaluatorWrapper = new FSharpFunctionEvaluatiorWrapper(engineEvaluatorWrapper);
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(engineEvaluatorWrapper);

            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var expressionEvaluatorService = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, manager, converter);

            PlotManager plotManager = new PlotManager(functionEvaluatorWrapper);
            TangentManager tangentManager = new TangentManager(functionEvaluatorWrapper, expressionEvaluatorService);
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

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
