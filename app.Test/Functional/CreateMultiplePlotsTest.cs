﻿namespace app.Test.Functional
{
    public class CreateMultiplePlotsTest
    {
        /// <summary>
        /// Test to simulate user creating multiple plots.
        /// </summary>
        [Test]
        public void TestCreateMultiplePlots()
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

            string userSetInputEquationSecond = "x*2";
            string userSetInputEquationThird = "2";
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
            Assert.That(plotViewModel.Error, Is.Empty, "Must not have an error");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(3), "Must have 3 line series");
            Assert.That(plotViewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.That(plotViewModel.SelectedPlot.Function, Is.EqualTo(userSetInputEquationThird), "Selected plot must be the third plot added");
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(3), "Plots collection count must be 1");
        }
    }
}
