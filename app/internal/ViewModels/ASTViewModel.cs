using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Msagl.Drawing;
using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public class ASTViewModel : ObservableObject
    {
        private ASTNode _rootNode;
        private Graph _graph;
        private IASTConverter _converter;

        public ASTViewModel(IASTConverter converter)
        {
            // @TODO: Remove me once actual implementation is there.
            var input = "1 + 2 * 3 / (4 - 5) ^ 6 % 7";
            var tokens = Engine.Tokeniser.tokenise(input);
            var ast = Engine.ASTParser.parse(tokens.ResultValue);
            
            // Initialize F# AST.
            FSharpASTNode fSharpAST = ast.ResultValue;

            // Convert F# AST to C# AST.
            _converter = converter;
            RootNode = _converter.Convert(fSharpAST);

            // Setup the graph.
            _graph = ConvertAstToGraph(RootNode);
        }

        private Graph ConvertAstToGraph(ASTNode root)
        {
            var graph = new Graph();

            AddAstNodeToGraph(graph, null, root);

            return graph;
        }

        private void AddAstNodeToGraph(Graph graph, Node parentGraphNode, ASTNode astNode)
        {
            var graphNode = graph.AddNode(astNode.ToString());
            graphNode.Attr.Shape = Shape.Box;

            if (parentGraphNode != null)
            {
                graph.AddEdge(parentGraphNode.Id, graphNode.Id);
            }

            foreach (var child in astNode.Children)
            { 
                AddAstNodeToGraph(graph, graphNode, child);
            }
        }

        public Graph Graph
        {
            get => _graph;
            private set => SetProperty(ref _graph, value);
        }

        public ASTNode RootNode
        {
            get => _rootNode;
            set => SetProperty(ref _rootNode, value);
        }
    }
}
