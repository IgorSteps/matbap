//namespace app.Test.Functional
//{
//    public class VisualiseParseTreeTest
//    {
//        /// <summary>
//        /// Test user can visualuse a parse tree of the equation entered.
//        /// </summary>
//        [Test]
//        public void TestVisualiseParseTree()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            var expression = "2*x+1/5";

//            // F# wrappers.
//            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
//            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
//            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

//            // C# wrappers.
//            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
//            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
//            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);

//            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
//            var symTableManager = new SymbolTableManager();
//            var converter = new ASTManager();
//            var validator = new ValidationService();

//            var service = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, manager, converter);

//            var viewModel = new ExpressionViewModel(service);
//            viewModel.Expression = expression;

//            // -----
//            // ACT
//            // -----
//            viewModel.VisualiseCmd.Execute(null);

//            // ------
//            // ASSERT
//            // ------
//            // Cannot be tested. Because WPF Windows are not gloablly accessible, hence we cannot track if it was opened.
//        }
//    }
//}
