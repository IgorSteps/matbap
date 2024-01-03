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
            var expression = input;
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);
            var converter = new ASTManager();
            var service = new ExpressionEvaluatingService(symTableManager, evaluator, manager, converter);
            var viewModel = new ExpressionViewModel(service);
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
