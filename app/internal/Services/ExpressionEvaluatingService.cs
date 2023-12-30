namespace app
{
    public struct ExpressionEvaluatingServiceResult
    {
        public string Result { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public ExpressionEvaluatingServiceResult(string res, Error err) 
        {
            Result = res;
            Error = err;
        }
    }
    public class ExpressionEvaluatingService : IEvaluator
    {
        private readonly ISymbolTableManager _symbolTableManager;
        private readonly IFSharpEvaluatorWrapper _expressionEvaluator;
        private readonly IExpressionManager _expressionManager;

        public ExpressionEvaluatingService(
                ISymbolTableManager symbolTableManager,
                IFSharpEvaluatorWrapper expressionEvaluator,
                IExpressionManager manager 
            )
        {
            _symbolTableManager = symbolTableManager;
            _expressionEvaluator = expressionEvaluator;
            _expressionManager = manager;
        }

        public ExpressionEvaluatingServiceResult Evaluate(string input)
        {
            Expression expression = _expressionManager.CreateExpression(input);
            SymbolTable symbolTable = _symbolTableManager.GetSymbolTable();

            var result = _expressionEvaluator.Evaluate(expression.Value, symbolTable);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, result.Error);
            }

            return new ExpressionEvaluatingServiceResult(result.EvaluationResult, null);
        }
    }
}
