namespace app.Test.Functional
{
    public class AddTangentTest
    {
        /// <summary>
        /// Test to simulate user creating a Plot and adding a Tangent to it.
        /// </summary>
        [Test]
        public void TestAddTanget()
        {
            // --------
            // ASSEMBLE
            // --------
            // F# wrappers.
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);

            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var expressionEvaluatingService = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, manager, converter);

            FSharpFunctionEvaluatiorWrapper functionEvaluator = new FSharpFunctionEvaluatiorWrapper(evaluatorWrapper);
            PlotManager plotManager = new PlotManager(functionEvaluator);
            TangentManager tangentManager = new TangentManager(functionEvaluator, expressionEvaluatingService);
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);

            string userSetInputEquation = "x^2";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.5;

            plotViewModel.InputEquation = userSetInputEquation;
            plotViewModel.XMinimum = userSetXMin;
            plotViewModel.XMaximum = userSetXMax;
            plotViewModel.XStep = userSetXStep;

            double tangentX = 2;
            plotViewModel.TangentX = tangentX;

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            plotViewModel.AddTangentCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Error, Is.Empty, "Must not have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(2), "Must have 2 line series: function and its tangent");
            Assert.That(plotViewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent, Is.Not.Null, "Selected plot's tangent must not be null");
            Assert.That(plotViewModel.SelectedPlot.Tangent.X, Is.EqualTo(tangentX), "Selected plot's tangent must not be null");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(1), "Plots collection count must be 1");
        }
    }
}
