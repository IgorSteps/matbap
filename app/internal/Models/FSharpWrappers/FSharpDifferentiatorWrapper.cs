using FSharpAST = Engine.Types.Node;

namespace app
{
    public struct FSharpDifferentiationResult
    {
        public FSharpAST AST { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpDifferentiationResult(FSharpAST ast, Error error)
        {
            AST = ast;
            Error = error;
        }
    }

    public class FSharpDifferentiatorWrapper : IFSharpDifferentiatorWrapper
    {
        private readonly Engine.IDifferentiator _fsharpDifferentiator;
        public FSharpDifferentiatorWrapper(Engine.IDifferentiator differentiator)
        {
            _fsharpDifferentiator = differentiator;
        }

        public FSharpDifferentiationResult DifferentiateExpression(FSharpAST ast, string var)
        {
            var result = _fsharpDifferentiator.Differentiate(ast, var);
            if (result.IsError)
            {
                return new FSharpDifferentiationResult(null, new Error(result.ErrorValue));
            }

            return new FSharpDifferentiationResult(result.ResultValue, null);
        }
    }
}
