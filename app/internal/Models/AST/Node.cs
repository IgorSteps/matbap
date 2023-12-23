using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace app
{
    public enum ASTNodeType
    {
        Number,
        BinaryOperation,
        ParenthesisExpression
    }

    public abstract class ASTNode : ObservableObject
    {
        private ASTNode _parent;
        private readonly ASTNodeType _nodeType; // Node types are immutable once created.
        private ObservableCollection<ASTNode> _children = new ObservableCollection<ASTNode>();

        protected ASTNode(ASTNodeType type)
        {
            _nodeType = type;
        }

        public new abstract string ToString();

        public void AddChild(ASTNode child)
        {
            if (child != null)
            {
                child.Parent = this;
                Children.Add(child);
            }
        }

        public ASTNode Parent
        {
            get => _parent;
            set => SetProperty(ref _parent, value);
        }

        public ASTNodeType NodeType => _nodeType;

        public ObservableCollection<ASTNode> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
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
            return $"({_left.ToString()} {_operator} {_right.ToString()})";
        }
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
}
