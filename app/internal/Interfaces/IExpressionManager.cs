namespace app
{
    public interface IExpressionManager
    {
        public Expression CreateExpression(string expression);
        public FSharpDifferentiationResult Differentiate(Expression expression);
    }
}
