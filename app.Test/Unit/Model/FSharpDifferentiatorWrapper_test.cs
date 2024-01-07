using Engine;
using Microsoft.FSharp.Core;
using Moq;
using FSharpASTNode = Engine.Types.Node;

namespace app.Test.Unit
{
    public class FSharpDifferentiatorWrapper_test
    {
        [Test]
        public void Test_FSharpDifferentiatorWrapper_DifferetiateSuccess() 
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression expression = new(testInput);
            var ast = FSharpASTNode.NewVariable(testInput);
            expression.FSharpAST = ast;

            var expected = FSharpResult<FSharpASTNode, string>.NewOk(ast);
            var wrapper = new Mock<Engine.IDifferentiator>();
            var differentiator = new FSharpDifferentiatorWrapper(wrapper.Object);
            wrapper.Setup(w => w.Differentiate(expression.FSharpAST, "x")).Returns(expected);

            // ----
            // ACT
            // ----
            var result = differentiator.DifferentiateExpression(ast, "x");

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.AST, Is.EqualTo(ast), "ASTs don't match");
        }

        [Test]
        public void Test_FSharpDifferentiatorWrapper_DifferetiateError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression expression = new(testInput);
            var ast = FSharpASTNode.NewVariable(testInput);
            expression.FSharpAST = ast;
            string error = "test";

            var expected = FSharpResult<FSharpASTNode, string>.NewError(error);
            var wrapper = new Mock<Engine.IDifferentiator>();
            var differentiator = new FSharpDifferentiatorWrapper(wrapper.Object);
            wrapper.Setup(w => w.Differentiate(expression.FSharpAST, "x")).Returns(expected);

            // ----
            // ACT
            // ----
            var result = differentiator.DifferentiateExpression(ast, "x");

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(error), "Error's don't match");
        }
    }
}
