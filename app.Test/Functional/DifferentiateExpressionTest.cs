namespace app.Test.Functional
{
    public class DifferentiateExpressionTest
    {
        /// <summary>
        /// Test to simualte user entering a math expression and differentiating it successfuly.
        /// </summary>
        [Test]
        public void Test_DifferentiateExpression_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var expression = "x^2";
            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);
            var converter = new ASTManager();
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager, converter);
            var viewModel = new ExpressionViewModel(service);
            viewModel.Expression = expression;

            // ---
            // ACT
            // ---
            viewModel.DifferentiateCmd.Execute(null);

            // ------
            // ASSERT
            // ------ 
            // We check F# engine returns a string to make sure our GUI output is a clear string.
            Assert.That(viewModel.Answer, Is.EqualTo("2 * x ^ 1"), "Answer doesn't match expected");
        }
    }
}
  