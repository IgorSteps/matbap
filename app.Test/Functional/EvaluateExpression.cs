namespace app.Test.Functional
{
    public class EvaluateExpression
    {
        /// <summary>
        /// Test to simualte user entering a math expression and getting an answer back.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_ReturnType()
        {
            // --------
            // ASSEMBLE
            // --------
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var expression = "2";
            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
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
            // We check F# engine returns a string to make sure our GUI output is a clear string.
            Assert.That(viewModel.Answer, Is.EqualTo("2"), "Answer doesn't match expected");
        }

        /// <summary>
        /// Test to simualte user entering a math expression with new lines and getting an answer back.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithNewLine()
        {
            // --------
            // ASSEMBLE
            // --------
            // Same as 
            // x=1;
            // y=1;
            // x+y
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var expression = "x=1;\r\ny=1;\r\nx+y";
            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
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
            Assert.That(viewModel.Answer, Is.EqualTo("2"), "Expression with new lines has returned wrong answer");
        }
    }
}
