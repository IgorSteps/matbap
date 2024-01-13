using Engine;
using Microsoft.FSharp.Core;
using Moq;
using FSharpASTNode = Engine.Types.Node;

namespace app.Test
{
    public class FSharpASTGetterWrapper_test
    {
        private Mock<Engine.IASTGetter> _mockASTGetter;
        private FSharpASTGetterWrapper _getter;

        [SetUp]
        public void Setup()
        {
            _mockASTGetter = new Mock<Engine.IASTGetter>();
            _getter = new FSharpASTGetterWrapper(_mockASTGetter.Object);
        }

        [Test]
        public void Test_FSharpASTGetterWrapper_GetAST_HappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            var sampleNode = FSharpASTNode.NewVariable("x");
            var expectedMockResult = FSharpResult<FSharpASTNode, string>.NewOk(sampleNode);
            _mockASTGetter.Setup(g => g.GetAST(expression)).Returns(expectedMockResult);
            // -----
            // ACT
            // -----
            var result = _getter.GetAST(expression);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.AST, Is.EqualTo(sampleNode), "Nodes don't match");
        }

    }
}
