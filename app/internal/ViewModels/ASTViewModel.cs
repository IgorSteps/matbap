using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public class ASTViewModel : ObservableObject
    {
        private ASTNode _rootNode;
        private Graph _graph;
        public ASTViewModel()
        {
            var input = "1 + 2 * 3 / (4 - 5) ^ 6 % 7";
            var tokens = Engine.Tokeniser.tokenise(input);
            var ast = Engine.ASTParser.parse(tokens.ResultValue);
            // Initialize your F# AST here or receive it as a parameter
            FSharpASTNode fSharpAST = ast.ResultValue;

            // Convert F# AST to C# AST
            var converter = new ASTConversionService();
            RootNode = converter.Convert(fSharpAST);
            Graph = ConvertAstToGraph(RootNode);
        }
        private Graph ConvertAstToGraph(ASTNode node)
        {
            var graph = new Graph();
            AddAstNodeToGraph(graph, null, node);
            return graph;
        }

        private void AddAstNodeToGraph(Graph graph, Node parentGraphNode, ASTNode astNode)
        {
            var graphNode = graph.AddNode(astNode.ToString());
            graphNode.Attr.Shape = Shape.Box; // Customize node appearance

            if (parentGraphNode != null)
                graph.AddEdge(parentGraphNode.Id, graphNode.Id);

            foreach (var child in astNode.Children)
                AddAstNodeToGraph(graph, graphNode, child);
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
