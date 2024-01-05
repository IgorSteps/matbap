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
