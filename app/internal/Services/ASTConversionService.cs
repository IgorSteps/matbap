using System;
using FSharpASTNode = Engine.Types.Node; // Alias F# node type for better readiblity.
using FSharpNumType = Engine.Types.NumType; // Alias F# num type for better readiblity.

namespace app
{
    /// <summary>
    /// Convert F# AST Node to C# AST Node.
    /// </summary>
    public class ASTConversionService : IASTConverter
    {
        /// <summary>
        /// Match against F# node types to constuct C# node types for an AST.
        /// </summary>
        public ASTNode Convert(FSharpASTNode fSharpNode)
        {
            return fSharpNode switch
            {
                FSharpASTNode.Number num => ConvertFSharpNumber(num.Item),
                FSharpASTNode.BinaryOperation node => new BinaryOperationNode(node.Item1, Convert(node.Item2), Convert(node.Item3)),
                FSharpASTNode.ParenthesisExpression node => new ParenthesisExpressionNode(Convert(node.Item)),
                _ => throw new InvalidOperationException("Unknown C# node type in F# AST."),
            };
        }

        public string ConvertToString(ASTNode root)
        {
            return root.ToString();
        }

        private static ASTNode ConvertFSharpNumber(FSharpNumType numType)
        {
            return numType switch
            {
                FSharpNumType.Int intVal => new NumberNode<int>(intVal.Item),
                // F#'s float is a System.Double, hence C# type is a double.
                FSharpNumType.Float floatVal => new NumberNode<double>(floatVal.Item),
                _ => throw new InvalidOperationException("Unknown number type in F# NumType."),
            };
        }
    }
}
