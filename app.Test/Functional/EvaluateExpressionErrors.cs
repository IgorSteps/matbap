namespace app.Test.Functional
{
    public class EvaluateExpressionErrorsTest
    {
        /// <summary>
        /// Test to simualte user entering math expression that causes errors in Lexer and Parser.
        /// </summary>
        [Test]
        [TestCase("2..0", "Invalid Float at token position 1: the mantissa cannot lead with non digit")]
        [TestCase("2.0.0", "Invalid Float at token position 1: Can't have 2 decimal places in a float")]
        [TestCase("@", "Invalid Token at token position 1: @")]
        [TestCase("2++", "Expected number, '(' or '-'.")]
        [TestCase("2/0", "Evaluation error: division by 0.")]
        public void Test_EvaluateExpression_Error(string input, string expectedError)
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = input;

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("Error: "+expectedError), "Errors don't match");
        }
    }
}
