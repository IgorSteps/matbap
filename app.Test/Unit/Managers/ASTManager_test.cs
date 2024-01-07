using Microsoft.Msagl.Drawing;
using Moq;
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
        public void ASTManager_Convert_FSharpNumberNode_CreatesCorrectCSharpNode()
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
            var result = _converter.Convert(fSharpNumberNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shoudln't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<NumberNode<int>>(result.AST);
            Assert.That(expected, Is.EqualTo(result.AST.ToString()));
        }

        [Test]
        public void ASTManager_Convert_FSharpBinaryOperationNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "1+1";
            var testInput = 1;
            var leftTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var rightTestNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(testInput));
            var fSharpBinaryTestNode = FSharpASTNode.NewBinaryOperation("+", leftTestNode, rightTestNode);

            // ---
            // ACT
            // ---
            var result = _converter.Convert(fSharpBinaryTestNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shoudln't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<BinaryOperationNode>(result.AST);
            Assert.That(expected, Is.EqualTo(result.AST.ToString()));
        }

        [Test]
        public void ASTManager_Convert_FSharpParenthesisNode_CreatesCorrectCSharpNode()
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
            var result = _converter.Convert(fSharpParenthesisNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shoudln't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<ParenthesisExpressionNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_FSharpVariableNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "x";
            var testInput = "x";
            var varNode = FSharpASTNode.NewVariable(testInput);

            // ---
            // ACT
            // ---
            var result = _converter.Convert(varNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shoudln't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<VariableNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_FSharpVariableAssignemntNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "x=10";

            var varNode = FSharpASTNode.NewVariableAssignment("x", FSharpASTNode.NewNumber(FSharpNumType.NewInt(10)));

            // ---
            // ACT
            // ---
            var result = _converter.Convert(varNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shouldn't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<VariableAssignmentNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_FSharpFunctionNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "sin(x)";

            var varNode = FSharpASTNode.NewFunction("sin", FSharpASTNode.NewVariable("x"));

            // ---
            // ACT
            // ---
            var result = _converter.Convert(varNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shouldn't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<FunctionNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_FSharpUnaryMinusOperationNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "-1";

            var varNode = FSharpASTNode.NewUnaryMinusOperation("-", FSharpASTNode.NewNumber(FSharpNumType.NewInt(1)));

            // ---
            // ACT
            // ---
            var result = _converter.Convert(varNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shouldn't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<UnaryMinusNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_FSharpForLoopNode_CreatesCorrectCSharpNode()
        {
            // --------
            // ASSEMBLE
            // --------
            var expected = "for x=1 in range(1,1,1)";
            var numberNode = FSharpASTNode.NewNumber(FSharpNumType.NewInt(1));
            var varAssignmentNode = FSharpASTNode.NewVariableAssignment("x", numberNode);

            var varNode = FSharpASTNode.NewForLoop(varAssignmentNode, numberNode, numberNode, numberNode);

            // ---
            // ACT
            // ---
            var result = _converter.Convert(varNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shouldn't have an error");
            Assert.IsNull(result.Error, "Error must be null");
            Assert.IsInstanceOf<ForLoopNode>(result.AST);
            Assert.That(result.AST.ToString(), Is.EqualTo(expected));
        }

        public void ASTManager_Convert_ComplexNodes()
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
            var result = _converter.Convert(fSharpBinaryNode);

            // ------
            // ASSERT
            // ------
            Assert.IsFalse(result.HasError, "Shoudln't have an error");
            Assert.IsNull(result.Error, "Error must be null");

            Assert.IsInstanceOf<BinaryOperationNode>(result.AST);

            // Cast to C# BinaryOperationNode to access its properties.
            var cSharpBinaryNode = (BinaryOperationNode)result.AST;

            // Verify left and right children.
            Assert.IsInstanceOf<NumberNode<int>>(cSharpBinaryNode.Left);
            Assert.That(((NumberNode<int>)cSharpBinaryNode.Left).Value, Is.EqualTo(expected));

            Assert.IsInstanceOf<NumberNode<int>>(cSharpBinaryNode.Right);
            Assert.That(((NumberNode<int>)cSharpBinaryNode.Right).Value, Is.EqualTo(expected));
        }

        [Test]
        public void ASTManager_Convert_ASTtoGraph()
        {
            // --------
            // ASSEMBLE
            // --------
            // Expected AST.
            var rootAstNode = new NumberNode<int>(1);
            var leftChild = new NumberNode<int>(2);
            var rightChild = new NumberNode<int>(3);
            rootAstNode.AddChild(leftChild);
            rootAstNode.AddChild(rightChild);

            // ---
            // ACT
            // ---
            var graph = _converter.ConvertAstToGraph(rootAstNode);

            // ------
            // ASSERT
            // ------
            Assert.That(graph.NodeCount, Is.EqualTo(3), "Graph must have 3 nodes");
            Assert.That(graph.EdgeCount, Is.EqualTo(2), "Graph must have 2 edges");

            var rootNode = graph.FindNode(rootAstNode.Value.ToString());
            Assert.That(rootNode, Is.Not.Null, "Root node can't be null");

            // Check nodes.
            var leftNode = graph.FindNode(leftChild.Value.ToString());
            var rightNode = graph.FindNode(rightChild.Value.ToString());
            Assert.That(leftNode, Is.Not.Null, "Left child can't be null");
            Assert.That(rightNode, Is.Not.Null, "Right child can't be null");

            // Check edges.
            Assert.IsTrue(EdgeExists(graph, rootNode.Id, leftNode.Id), "Edge from root to left child must exist");
            Assert.IsTrue(EdgeExists(graph, rootNode.Id, rightNode.Id), "Edge from root to right child must exist");
        }

        private bool EdgeExists(Graph graph, string sourceNodeId, string targetNodeId)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.SourceNode.Id == sourceNodeId && edge.TargetNode.Id == targetNodeId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
