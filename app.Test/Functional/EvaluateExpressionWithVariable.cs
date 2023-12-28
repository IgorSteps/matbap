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
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "x=5";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo("10"), "Responses don't match");
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
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "x=5";
            string nextVariable = "y=5";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo("10"), "Responses don't match");
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
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "x=5";
            string nextVariable = "x=10";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo("15"), "Responses don't match");
        }
    }
}
