namespace app
{
    /// <summary>
    /// Interface for managing plot function evaluation, and other mathematical operations.
    /// </summary>
    public interface IFunctionEvaluator
    {
        public EvaluationResult Evaluate(string equation, double min, double max, double step);
        public EvaluationResult EvaluateAtPoint(double x, string function);
        public double TakeDerivative(double x, string function);
    }
}
