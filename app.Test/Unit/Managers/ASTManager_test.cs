using FSharpASTNode = Engine.Types.Node;
using FSharpNumType = Engine.Types.NumType;

namespace app.Test.Unit

{
    public class ASTManager_test
    {
        private ASTManager _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new ASTManager();
        }

        [Test]
        public void ASTConvertionService_Convert_FSharpNumberNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "1";
            var testInput = 1;
            var fSharpIntVal = FSharpNumType.NewInt(testInput);
            var fSharpNumberNode = FSharpASTNode.NewNumber(fSharpIntVal);

            // ---
            // ACT
            // ---
            var cSharpNode = _converter.Convert(fSharpNumberNode);

            // ------
            // ASSERT
            // ------
            Assert.IsInstanceOf<NumberNode<int>>(cSharpNode);
            Assert.That(expected, Is.EqualTo(cSharpNode.ToString()));
        }

        [Test]
        public void ASTConvertionService_Convert_FSharpBinaryOperationNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "1 + 1";
            var testInput = 1;
            var leftTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var rightTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var fSharpBinaryTestNode = FSharpASTNode.NewBinaryOperation("+", leftTestNode, rightTestNode);

            // ---
            // ACT
            // ---
            var cSharpNode = _converter.Convert(fSharpBinaryTestNode);

            // ------
            // ASSERT
            // ------
            Assert.IsInstanceOf<BinaryOperationNode>(cSharpNode);
            Assert.That(expected, Is.EqualTo(cSharpNode.ToString()));
        }

        [Test]
        public void ASTConvertionService_Convert_FSharpParenthesisNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "(1)";
            var testInput = 1;
            var bracketedNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var fSharpParenthesisNode = FSharpASTNode.NewParenthesisExpression(bracketedNode);

            // ---
            // ACT
            // ---
            var cSharpNode = _converter.Convert(fSharpParenthesisNode);

            // ------
            // ASSERT
            // ------
            Assert.IsInstanceOf<ParenthesisExpressionNode>(cSharpNode);
            Assert.That(cSharpNode.ToString(), Is.EqualTo(expected));
        }

        public void ASTConvertionService_Convert_ComplexNodes()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = 1;
            var testInput = 1;
            var leftTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var rightTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var fSharpBinaryNode = FSharpASTNode.NewBinaryOperation("+", leftTestNode, rightTestNode);

            // ---
            // ACT
            // ---
            var cSharpNode = _converter.Convert(fSharpBinaryNode);

            // ------
            // ASSERT
            // ------
            Assert.IsInstanceOf<BinaryOperationNode>(cSharpNode);

            // Cast to C# BinaryOperationNode to access its properties.
            var cSharpBinaryNode = (BinaryOperationNode)cSharpNode;

            // Verify left and right children.
            Assert.IsInstanceOf<NumberNode<int>>(cSharpBinaryNode.Left);
            Assert.That(((NumberNode<int>)cSharpBinaryNode.Left).Value, Is.EqualTo(expected));

            Assert.IsInstanceOf<NumberNode<int>>(cSharpBinaryNode.Right);
            Assert.That(((NumberNode<int>)cSharpBinaryNode.Right).Value, Is.EqualTo(expected));
        }
    }
}
