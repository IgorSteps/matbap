using Moq;
using FSharpASTNode = Engine.Types.Node;

namespace app.Test.Unit
{
    public class ExpressionManager_test
    {
        [Test]
        public void Test_ExpressionManager_CreateExpression()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            Expression expected = new Expression(expression);
            var fSharpDiffWrapper = new Mock<IFSharpDifferentiatorWrapper>();
            ExpressionManager manager = new ExpressionManager(fSharpDiffWrapper.Object);

            // ----
            // ACT
            // ----
            Expression actual = manager.CreateExpression(expression);


            // ------
            // ASSERT
            // ------
            Assert.That(actual.Value, Is.EqualTo(expression), "Expressions don't match");
        }

        [Test]
        public void Test_ExpressionManager_DifferentiateSuccess()
        {
            // --------
            // ASSEMBLE
            // --------
            var fSharpDiffWrapper = new Mock<IFSharpDifferentiatorWrapper>();
            ExpressionManager manager = new ExpressionManager(fSharpDiffWrapper.Object);
            
            string testInput = "x";
            Expression expression = new(testInput);
            var ast = FSharpASTNode.NewVariable(testInput);
            expression.FSharpAST = ast;

            var expected = new FSharpDifferentiationResult(ast, null);
            fSharpDiffWrapper.Setup(d => d.DifferentiateExpression(expression.FSharpAST, "x")).Returns(expected);
           
            // ----
            // ACT
            // ----
            var actual = manager.Differentiate(expression);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(actual.HasError, "Shouldn't have an error");
            Assert.IsNull(actual.Error, "Error should be null");
            Assert.That(actual.AST, Is.EqualTo(ast), "ASTs are not equal");
        }

        [Test]
        public void Test_ExpressionManager_DifferentiateError()
        {
            // --------
            // ASSEMBLE
            // --------
            var fSharpDiffWrapper = new Mock<IFSharpDifferentiatorWrapper>();
            ExpressionManager manager = new ExpressionManager(fSharpDiffWrapper.Object);

            string testInput = "x";
            Error testError = new Error("boom");
            Expression expression = new(testInput);

            var expected = new FSharpDifferentiationResult(null, testError);
            fSharpDiffWrapper.Setup(d => d.DifferentiateExpression(expression.FSharpAST, "x")).Returns(expected);

            // ----
            // ACT
            // ----
            var actual = manager.Differentiate(expression);

            // ------
            // ASSERT
            // ------
            Assert.IsTrue(actual.HasError, "Shoul have an error");
            Assert.IsNotNull(actual.Error, "Error shouldn't be null");
            Assert.That(actual.Error, Is.EqualTo(testError), "Errors are not equal");
        }
    }
}
