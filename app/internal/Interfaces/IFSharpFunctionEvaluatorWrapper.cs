namespace app
{
    /// <summary>
    /// Interface for wrapping F# engine's function evaluation, and other mathematical operations.
    /// </summary>
    public interface IFSharpFunctionEvaluatorWrapper
    {
        public FunctionEvaluationResult Evaluate(string equation, double min, double max, double step);
        public FunctionEvaluationResult EvaluateAtPoint(double x, string function);
    }
}
