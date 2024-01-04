namespace app.Test.Functional
{
    public class DifferentiateExpressionErrorTest
    {
        /// <summary>
        /// Test to simualte user entering a math expression and differentiating it errors.
        /// </summary>
        [Test]
        [TestCase("x^(2*x)", "Error: Differentiation with non-constant power is not supported")]
        //[TestCase("ln(x))", "Error: Function 'ln' is not supported for differentiation")] // we don't tokenise any not supported function.
        [TestCase("x=2", "Error: Unsupported node type for differentiation")]
        public void Test_DifferentiateExpression_Error(string input, string error)
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
            string expectedError = error;

            // ---
            // ACT
            // ---
            viewModel.DifferentiateCmd.Execute(null);

            // ------
            // ASSERT
            // ------ 
            // We check F# engine returns a string to make sure our GUI output is a clear string.
            Assert.That(viewModel.Answer, Is.EqualTo(expectedError), "Errors don't match expected");
        }
    }
}
