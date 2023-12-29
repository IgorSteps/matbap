namespace app
{
    public struct FSharpEvaluationResult
    {
        public string EvaluationResult { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpEvaluationResult(string answer, Error err)
        {
            EvaluationResult = answer;
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
                return new FSharpEvaluationResult(null, new Error(result.ErrorValue));
            }
           

            return new FSharpEvaluationResult(result.ResultValue.Item1, null);
        }
    }
}
