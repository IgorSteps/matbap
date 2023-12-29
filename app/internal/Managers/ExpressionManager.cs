namespace app
{
    public class ExpressionManager : IExpressionManager
    {
        public Expression CreateExpression(string expression)
        {
            return new Expression(expression);
        }
    }
}
