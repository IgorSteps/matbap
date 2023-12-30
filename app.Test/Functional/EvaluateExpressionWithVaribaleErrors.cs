namespace app.Test.Functional
{
    public class EvaluateExpressionWithVaribalesErrors
    {
        /// <summary>
        /// Test to simualte user entering math expression with an unknown variable that causes error in the Parser.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithVariables_UnknownIdetifierError()
        {
            // --------
            // ASSEMBLE
            // --------
            var manager = new ExpressionManager();
            var symTableManager = new SymbolTableManager();
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager);
            var viewModel = new ExpressionViewModel(service);
            viewModel.Expression = "x=5";
            string nextExpression = "y=5";
            string sumExpression = "x+z";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = sumExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("Error: Error while parsing: Identifier not found"), "Errors don't match");
        }
    }
}
