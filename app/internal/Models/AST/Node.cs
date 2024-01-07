using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;


namespace app
{
    public enum ASTNodeType
    {
        Number,
        BinaryOperation,
        ParenthesisExpression,
        Variable,
        UnaryMinusOperation,
        Function,
        VariableAssginemnt,
        ForLoop
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
            AddChild(_right);
            AddChild(_left);
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

    public class VariableAssignmentNode : ASTNode
    {
        private readonly string _variable;
        private readonly ASTNode _assignee;

        public VariableAssignmentNode(string variable, ASTNode assigneeNode) : base(ASTNodeType.VariableAssginemnt)
        {
            _variable = variable;
            _assignee = assigneeNode;
            AddChild(assigneeNode);
            AddChild(new VariableNode(variable));
        }

        public override string ToString()
        {
            return $"{_variable}={_assignee.ToString()}";
        }
    }

    public class UnaryMinusNode : ASTNode 
    {
        private readonly ASTNode _expression;
        
        public UnaryMinusNode(ASTNode expression) : base(ASTNodeType.UnaryMinusOperation)
        {
            _expression = expression;
            AddChild(expression);
        }

        public override string ToString()
        {
            return $"-{_expression.ToString()}";
        }
    }

    public class FunctionNode : ASTNode
    {
        private readonly string _function;
        private readonly ASTNode _expression;

        public FunctionNode(string function, ASTNode expression) : base(ASTNodeType.Function)
        { 
            _function = function;
            _expression = expression;
            AddChild(_expression);
        }

        public override string ToString()
        {
            return $"{_function}({_expression.ToString()})";
        }
    }

    public class ForLoopNode : ASTNode
    {
        private readonly ASTNode _variableAssignment;
        private readonly ASTNode _xmin;
        private readonly ASTNode _xmax;
        private readonly ASTNode _xstep;

        public ForLoopNode(ASTNode variableAssignment, ASTNode xmin, ASTNode xmax, ASTNode xstep) : base(ASTNodeType.ForLoop)
        {
            _variableAssignment = variableAssignment;
            _xmin = xmin;
            _xmax = xmax;
            _xstep = xstep;
            AddChild(xstep);
            AddChild(xmax);
            AddChild(xmin);
            AddChild(variableAssignment);
        }

        public override string ToString()
        {
            return $"for {_variableAssignment.ToString()} in range({_xmin.ToString()},{_xmax.ToString()},{_xstep.ToString()})";
        }
    }


}
