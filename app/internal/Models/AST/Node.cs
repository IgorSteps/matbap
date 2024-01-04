using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Msagl.Core.Layout;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace app
{
    public enum ASTNodeType
    {
        Number,
        BinaryOperation,
        ParenthesisExpression,
        Variable
    }

    public abstract class ASTNode : ObservableObject
    {
        public ASTNode Parent { get; set; }
        public List<ASTNode> Children = new List<ASTNode>();
        private readonly ASTNodeType _nodeType; // Node types are immutable once created.

        protected ASTNode(ASTNodeType type)
        {
            _nodeType = type;
        }
        public ASTNodeType NodeType => _nodeType;

        public new abstract string ToString();

        public void AddChild(ASTNode child)
        {
            if (child != null)
            {
                child.Parent = this;
                Children.Add(child);
            }
        }

    }

    public class NumberNode<T> : ASTNode
    {
        private readonly T _value;

        public NumberNode(T value) : base(ASTNodeType.Number)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public T Value => _value;
    }

    public class BinaryOperationNode : ASTNode
    {
        private readonly string _operator;
        private readonly ASTNode _left;
        private readonly ASTNode _right;
        public BinaryOperationNode(string oper, ASTNode left, ASTNode right) : base(ASTNodeType.BinaryOperation)
        {
            _operator = oper;
            _left = left;
            _right = right;
            AddChild(_left);
            AddChild(_right);
        }

        public override string ToString()
        {
            return $"{_left.ToString()}{_operator}{_right.ToString()}";
        }

        public string Operator => _operator;

        public ASTNode Left => _left;
        public ASTNode Right => _right;
    }
     
    public class ParenthesisExpressionNode : ASTNode
    {
        private readonly ASTNode _bracketedNode;
        public ParenthesisExpressionNode(ASTNode bracketedNode) : base(ASTNodeType.ParenthesisExpression)
        {
            _bracketedNode = bracketedNode;
            AddChild(_bracketedNode);
        }

        public override string ToString()
        {
            return $"({_bracketedNode.ToString()})";
        }
    }

    public class VariableNode : ASTNode
    {
        private readonly string _name;
        public VariableNode(string variable) : base(ASTNodeType.Variable)
        {
            _name = variable;
        }
        public override string ToString()
        {
            return _name.ToString();
        }
    }

}
