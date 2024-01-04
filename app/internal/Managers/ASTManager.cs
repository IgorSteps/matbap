﻿using Microsoft.Msagl.Drawing;
using FSharpASTNode = Engine.Types.Node; // Alias F# node type for better readiblity.
using FSharpNumType = Engine.Types.NumType; // Alias F# num type for better readiblity.

namespace app
{
    /// <summary>
    /// Convert F# AST Node to C# AST Node.
    /// </summary>
    public class ASTManager : IASTConverter
    {
        public struct ConvertionResult
        {
            public ASTNode AST { get; private set; }
            public Error Error { get; private set; }
            public readonly bool HasError => Error != null;
            public ConvertionResult(ASTNode ast, Error err)
            {
                AST = ast;
                Error = err;
            }
        }

        /// <summary>
        /// Match against F# node types to constuct C# node types for an AST.
        /// </summary>
        public ConvertionResult Convert(FSharpASTNode fSharpNode)
        {
            switch (fSharpNode)
            {
                case FSharpASTNode.Number node:
                    return ConvertFSharpNumber(node.Item);

                case FSharpASTNode.BinaryOperation node:
                    var leftOperandConversion = Convert(node.Item2);
                    if (leftOperandConversion.HasError)
                    {
                        return leftOperandConversion;
                    }

                    var rightOperandConversion = Convert(node.Item3);
                    if (rightOperandConversion.HasError)
                    {
                        return rightOperandConversion;
                    }

                    var binaryOpNode = new BinaryOperationNode(node.Item1, leftOperandConversion.AST, rightOperandConversion.AST);
                    return new ConvertionResult(binaryOpNode, null);

                case FSharpASTNode.ParenthesisExpression node:
                    var bracketedNode = Convert(node.Item);
                    if (bracketedNode.HasError)
                    {
                        return bracketedNode;
                    }

                    var parenthesisNode = new ParenthesisExpressionNode(bracketedNode.AST);
                    return new ConvertionResult(parenthesisNode, null);

                case FSharpASTNode.Variable node:
                    var variableNode = new VariableNode(node.Item);
                    return new ConvertionResult(variableNode, null);

                default:
                    return new ConvertionResult(null, new Error("Failed to convert F# AST - unknown node type."));
            }
        }

        public string ConvertToString(ASTNode root)
        {
            return root.ToString();
        }

        public Graph ConvertAstToGraph(ASTNode root)
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

        private static ConvertionResult ConvertFSharpNumber(FSharpNumType numType)
        {
            return numType switch
            {
                FSharpNumType.Int intVal => new ConvertionResult(new NumberNode<int>(intVal.Item), null),
                // F#'s float is a System.Double, hence C# type is a double.
                FSharpNumType.Float floatVal => new ConvertionResult(new NumberNode<double>(floatVal.Item), null),
                _ => new ConvertionResult(null, new Error("Unknown number type in F# AST.")),
            };
        }
    }
}
