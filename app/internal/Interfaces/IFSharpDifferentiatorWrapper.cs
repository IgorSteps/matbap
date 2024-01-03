using FSharpAST = Engine.Types.Node;

namespace app
{
    public interface IFSharpDifferentiatorWrapper
    {
        public FSharpDifferentiationResult DifferentiateExpression(FSharpAST ast, string var);
    }
}
