namespace app
{
    /// <summary>
    /// Interface for a serice to evaluate mathematical expressions.
    /// </summary>
    public interface IEvaluator
    {
        public ExpressionEvaluatingServiceResult Evaluate(string input);
        public ExpressionEvaluatingServiceResult Differentiate(string input);
    }
}
