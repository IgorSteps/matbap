namespace app
{
    /// <summary>
    /// Interface for a wrapper of the F# evaluator.
    /// </summary>
    public interface IFSharpEvaluatorWrapper
    {
        public FSharpEvaluationResult Evaluate(string expression, SymbolTable symbolTable);
    }
}
