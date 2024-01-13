using FSharpAST = Engine.Types.Node;

namespace app
{
    public class Expression
    {
        public string Value { get; private set; }
        public FSharpAST FSharpAST { get; set; }
        public ASTNode CSharpAST { get; set; }
        public double[][] Points { get; set; }
        public Expression(string expression)
        {
            Value = expression;
        }
    }
}
