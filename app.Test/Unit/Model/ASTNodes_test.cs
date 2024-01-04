namespace app.Test.Unit
{
    public class ASTNodes_test
    {
        [Test]
        public void ASTNodeModel_TestNumberNode_ToString()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = 1;
            var expected = "1";

            // ---
            // ACT
            // ---
            var numberNode = new NumberNode<int>(testInput);

            // ------
            // ASSERT
            // ------
            Assert.That(numberNode.NodeType, Is.EqualTo(ASTNodeType.Number));
            Assert.That(numberNode.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTNodeModel_TestBinaryOperationNode_SetParent()
        {
            // --------
            // ASSEMBLE
            // --------
            var testLeftNode = new NumberNode<int>(1);
            var testRightNode = new NumberNode<int>(1);

            // ---
            // ACT
            // ---
            var operNode = new BinaryOperationNode("+", testLeftNode, testRightNode);

            // ------
            // ASSERT
            // ------
            Assert.That(operNode.NodeType, Is.EqualTo(ASTNodeType.BinaryOperation));
            Assert.That(operNode.ToString(), Is.EqualTo("1+1"));
            Assert.That(testLeftNode.Parent, Is.SameAs(operNode));
            Assert.That(testLeftNode.Parent, Is.SameAs(operNode));
        }

        [Test]
        public void ASTNodeModel_TestParenthesisExpressionNode_ToString()
        {
            // --------
            // ASSEMBLE
            // --------
            var numberNode = new NumberNode<int>(1);
            var expected = "(1)";

            // ---
            // ACT
            // ---
            var parenNode = new ParenthesisExpressionNode(numberNode);

            // ------
            // ASSERT
            // ------
            Assert.That(parenNode.NodeType, Is.EqualTo(ASTNodeType.ParenthesisExpression));
            Assert.That(parenNode.ToString(), Is.EqualTo(expected));
            Assert.That(numberNode.Parent, Is.SameAs(parenNode));
        }

        [Test]
        public void ASTNodeModel_TestComplexAST()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "(1+1)";
            var leftTestNode = new NumberNode<int>(1);
            var rightTestNode = new NumberNode<int>(1);
            var binaryTestNode = new BinaryOperationNode("+", leftTestNode, rightTestNode);

            // ---
            // ACT
            // ---
            var root = new ParenthesisExpressionNode(binaryTestNode);

            // ------
            // ASSERT
            // ------
            Assert.That(root.ToString(), Is.EqualTo(expected));
            // Test parent-child relationships.
            Assert.IsNull(root.Parent, "Root can't have a parent.");
            Assert.That(root.Children[0], Is.EqualTo(binaryTestNode), "Root should have binary node as a child.");
        }
    }
}
