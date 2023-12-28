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
        [TestCase("2++", "Error while parsing: Unexpected token or end of expression")]
        [TestCase("2/0", "Error while parsing: division by 0")]
        [TestCase("2.0 % 2", "Error while parsing: modulo cannot be used with floats")]
        public void Test_EvaluateExpression_Error(string input, string expectedError)
        {
            // --------
            // ASSEMBLE
            // --------
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = input;
            
            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo(expectedError), "Errors don't match");
        }
    }
}
