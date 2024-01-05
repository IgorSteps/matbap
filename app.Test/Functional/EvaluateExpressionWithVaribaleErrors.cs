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
