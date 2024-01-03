using FSharpAST = Engine.Types.Node;

namespace app
{
    public class Expression
    {
        public string Value { get; private set; }
        public FSharpAST FSharpAST { get; private set; }
        public ASTNode CSharpAST { get; private set; }
        public Expression(string expression)
        {
            Value = expression;
        }
    }
}
