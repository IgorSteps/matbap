namespace app.Test.Functional
{
    public class EvaluateForLoopPlottingFunctionTest
    {
        [Test]
        public void Test_ForLoop_PlottingFunction()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "for x in range(1,10,0.1): sin(x)";
            var viewModel = Utils.CreateExpressionViewModel();
            var plotViewModel = Utils.CreaePlotViewModel();
            viewModel.Expression = expression;

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.Null, "Answer must be null");
            Assert.That(plotViewModel.Plots, Is.Not.Empty, "Plots should be populated after evaluation");
            Assert.That(plotViewModel.OxyPlotModel.Series.Count, Is.EqualTo(1), "Oxyplot plot model must have 1 line series");
            Assert.That(plotViewModel.SelectedPlot.Function, Is.EqualTo(expression), "PlotViewModel selected plot is not equal to tested expression");
        }
    }
}
