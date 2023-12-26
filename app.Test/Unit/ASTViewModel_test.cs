using Microsoft.Msagl.Drawing;
using Moq;
using System.Windows.Forms.VisualStyles;
using FSharpASTNode = Engine.Types.Node;

namespace app.Test
{
    public class ASTViewModelUnitTest
    {
        private Mock<IASTConverter> _mockConverter;
        private ASTViewModel _viewModel;

        [Test]
        public void ASTViewModel_CorrectConversionToGraph()
        {
            // --------
            // ASSEMBLE
            // --------
            // Create a sample AST.
            var rootAstNode = new NumberNode<int>(1);
            var leftChild = new NumberNode<int>(2);
            var rightChild = new NumberNode<int>(3);
            rootAstNode.AddChild(leftChild);
            rootAstNode.AddChild(rightChild);

            _mockConverter = new Mock<IASTConverter>();
            _mockConverter.Setup(c => c.Convert(It.IsAny<FSharpASTNode>())).Returns(rootAstNode);

            // ---
            // ACT
            // ---
            _viewModel = new ASTViewModel(_mockConverter.Object);

            // ------
            // ASSERT
            // ------
            var graph = _viewModel.Graph;
            Assert.That(graph.NodeCount, Is.EqualTo(3), "Graph must have 3 nodes");
            Assert.That(graph.EdgeCount, Is.EqualTo(2), "Graph must have 2 edges");

            var rootNode = graph.FindNode(rootAstNode.Value.ToString());
            Assert.That(rootNode, Is.Not.Null, "Root node can't be null");
            
            // Check nodes.
            var leftNode = graph.FindNode(leftChild.Value.ToString());
            var rightNode = graph.FindNode(rightChild.Value.ToString());
            Assert.That(leftNode, Is.Not.Null, "Left child can't be null");
            Assert.That(rightNode, Is.Not.Null, "Right child  can't be null");

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
