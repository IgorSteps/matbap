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
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "x=5";
            string nextExpression = "y=5";
            string sumExpression = "x+z";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.InterpretCmd.Execute(null);

            viewModel.Expression = sumExpression;
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo("Error while parsing: Identifier not found"), "Errors don't match");
        }
    }
}
