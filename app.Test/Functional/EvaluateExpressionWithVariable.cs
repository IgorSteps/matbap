namespace app.Test.Functional
{
    public class EvaluateExpressionWithVariable
    {
        /// <summary>
        /// Test to simualte user entering math expression with variables.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithVariable()
        {
            // --------
            // ASSEMBLE
            // --------
            var manager = new ExpressionManager();
            var symTableManager = new SymbolTableManager();
            var evaluator = new FSharpEvaluatorWrapper();
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager);
            var viewModel = new ExpressionViewModel(service);
            viewModel.Expression = "x=5";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("10"), "Responses don't match");
        }

        /// <summary>
        /// Test to simualte user entering math expression with 2 variables.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithMultipleVariables()
        {
            // --------
            // ASSEMBLE
            // --------
            var manager = new ExpressionManager();
            var symTableManager = new SymbolTableManager();
            var evaluator = new FSharpEvaluatorWrapper();
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager);
            var viewModel = new ExpressionViewModel(service);
            viewModel.Expression = "x=5";
            string nextVariable = "y=5";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("10"), "Responses don't match");
        }

        /// <summary>
        /// Test to simualte user entering math expression and reassinging variables.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_ReassignVariables()
        {
            // --------
            // ASSEMBLE
            // --------
            var manager = new ExpressionManager();
            var symTableManager = new SymbolTableManager();
            var evaluator = new FSharpEvaluatorWrapper();
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager);
            var viewModel = new ExpressionViewModel(service);
            viewModel.Expression = "x=5";
            string nextVariable = "x=10";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("15"), "Responses don't match");
        }
    }
}
