using Engine;

namespace app.Test.Functional
{
    public class DisplayMultiplePlottedFunctionsTest
    {
        /// <summary>
        /// Test to simulate user adding 3 plots and GUI displaying their functions in the "Plotted Functions:" panel.
        /// </summary>
        [Test]
        public void TestDisplayMultiplePlottedFunctions()
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

            string userSetInputEquationSecond = "2*x+1";
            string userSetInputEquationThird = "123";

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            
           
            plotViewModel.InputEquation = userSetInputEquationSecond;
            plotViewModel.PlotCmd.Execute(null);
            
            plotViewModel.InputEquation = userSetInputEquationThird;
            plotViewModel.PlotCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(3), "There must be 3 plot");
            Assert.That(plotViewModel.Plots[0].Function, Is.EqualTo(userSetInputEquation), "Incorrect equation is stored in the Plots collection");
            Assert.That(plotViewModel.Plots[1].Function, Is.EqualTo(userSetInputEquationSecond), "Incorrect equation is stored in the Plots collection");
            Assert.That(plotViewModel.Plots[2].Function, Is.EqualTo(userSetInputEquationThird), "Incorrect equation is stored in the Plots collection");
        }
    }
}
