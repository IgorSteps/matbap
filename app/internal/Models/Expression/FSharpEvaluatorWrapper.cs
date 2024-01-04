using FSharpASTNode = Engine.Types.Node;

namespace app
{
    public struct FSharpEvaluationResult
    {
        public string Answer { get; private set; }
        public FSharpASTNode FSharpAST { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpEvaluationResult(string answer, FSharpASTNode ast, Error err)
        {
            Answer = answer;
            FSharpAST = ast;
            Error = err;
        }
    }

    /// <summary>
    /// Handles evaluation of the expression using F# engine.
    /// </summary>
    public class FSharpEvaluatorWrapper : IFSharpEvaluatorWrapper
    {
        private readonly Engine.IEvaluator _fsharpEvaluator;

        public FSharpEvaluatorWrapper(Engine.IEvaluator fsharpEvaluator)
        {
            _fsharpEvaluator = fsharpEvaluator;
        }

        public FSharpEvaluationResult Evaluate(string expression, SymbolTable symbolTable )
        {
            var result = _fsharpEvaluator.Eval(expression, symbolTable.Table);
            if (result.IsError)
            {
                return new FSharpEvaluationResult(null, null, new Error(result.ErrorValue));
            }
           
            return new FSharpEvaluationResult(result.ResultValue.Item1, result.ResultValue.Item3, null);
        }
    }
}
