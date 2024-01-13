using OxyPlot.Series;
using OxyPlot;

namespace app
{
    public class ExpressionManager : IExpressionManager
    {
        private readonly IFSharpDifferentiatorWrapper _differentiator;
        public ExpressionManager(IFSharpDifferentiatorWrapper differentiator)
        {
            _differentiator = differentiator;
        }

        public Expression CreateExpression(string expression)
        {
            return new Expression(expression);
        }

        public FSharpDifferentiationResult Differentiate(Expression expression)
        {
            var result = _differentiator.DifferentiateExpression(expression.FSharpAST, "x");
            if (result.HasError)
            {
                return new FSharpDifferentiationResult(null, result.Error);
            }

            return new FSharpDifferentiationResult(result.AST, null);
        }

        /// <summary>
        /// Get line series from an expression with Points when
        /// user enters for-loop with plot function expression.
        /// </summary>
        public LineSeries GetLineSeriesFromExpression(Expression expression) 
        {
            LineSeries lineSeries = new LineSeries
            {
                Title = expression.Value.ToString(),
            };

            foreach (var point in expression.Points)
            {
                lineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            return lineSeries;
        }
    }
}
