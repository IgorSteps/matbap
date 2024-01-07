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
            var expression = "x=2";

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;
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
            Assert.That(viewModel.Answer, Is.EqualTo("Error: Evaluation error: variable identifier not found."), "Errors don't match");
        }
    }
}
