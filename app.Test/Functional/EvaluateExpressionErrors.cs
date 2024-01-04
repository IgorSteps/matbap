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

            // F# wrappers.
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);

            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var service = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, manager, converter);

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
