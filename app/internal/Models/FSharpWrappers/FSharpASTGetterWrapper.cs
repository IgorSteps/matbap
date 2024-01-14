using FSharpAST = Engine.Types.Node;

namespace app
{
    public struct FSharpASTGetterResult
    { 
        public FSharpAST AST { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpASTGetterResult(FSharpAST ast, Error error)
        {
            AST = ast;
            Error = error;
        }
    }
    public class FSharpASTGetterWrapper: IFSharpASTGetterWrapper
    {
        private readonly Engine.IASTGetter _astGetter;
        public FSharpASTGetterWrapper(Engine.IASTGetter astGetter)
        {
            _astGetter = astGetter;
        }

        public FSharpASTGetterResult GetAST(string expression)
        {
            var result = _astGetter.GetAST(expression);
            if (result.IsError)
            {
                return new FSharpASTGetterResult(null, new Error(result.ErrorValue));
            }

            return new FSharpASTGetterResult(result.ResultValue, null);
        }
    }
}
