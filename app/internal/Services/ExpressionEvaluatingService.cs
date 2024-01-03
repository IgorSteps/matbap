using FSharpAST = Engine.Types.Node;

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

        public ExpressionEvaluatingServiceResult Evaluate(Expression expression, string input)
        {
            expression = _expressionManager.CreateExpression(input);
            SymbolTable symbolTable = _symbolTableManager.GetSymbolTable();

            var result = _expressionEvaluator.Evaluate(expression.Value, symbolTable);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, result.Error);
            }

            expression.FSharpAST = TemporarySetAst(input);

            return new ExpressionEvaluatingServiceResult(result.EvaluationResult, null);
        }

        public ExpressionEvaluatingServiceResult Differentiate(Expression expression) 
        {
            var result = _expressionManager.Differentiate(expression);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, result.Error);
            }

            // @TODO:
            // Convert AST to C# AST;
            // Convert AST to expression;

            return new ExpressionEvaluatingServiceResult("BOOM", null);
        }

        // @TODO: Refactor once Evaluator is switched to AST Evaluator.
        private FSharpAST TemporarySetAst(string input)
        {
            var tokens = Engine.Tokeniser.tokenise(input);
            return Engine.ASTParser.parse(tokens.ResultValue).ResultValue;
        }
    }
}
