namespace app
{
    /// <summary>
    /// Message consisting of expression, that is used to communicate between Expression and Plot view models
    /// when user enters a for-loop plot function.
    /// </summary>
    public class PlotExpressionMessage
    {
        public Expression Expression { get; private set; }

        public PlotExpressionMessage(Expression expression)
        {
            Expression = expression;
        }
    }
}
