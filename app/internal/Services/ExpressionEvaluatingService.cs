using Microsoft.Msagl.Drawing;
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

    public struct VisualiseASTResult
    {
        public Graph AST { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public VisualiseASTResult(Graph ast, Error err)
        {
            AST = ast;
            Error = err;
        }
    }

    public class ExpressionEvaluatingService : IEvaluator
    {
        private readonly IFSharpASTGetterWrapper _astGetter;
        private readonly IValidator _validator;
        private readonly ISymbolTableManager _symbolTableManager;
        private readonly IFSharpEvaluatorWrapper _expressionEvaluator;
        private readonly IExpressionManager _expressionManager;
        private readonly IASTConverter _astConverter;

        public ExpressionEvaluatingService(
                IFSharpASTGetterWrapper astGetter,
                IValidator validator,
                ISymbolTableManager symbolTableManager,
                IFSharpEvaluatorWrapper expressionEvaluator,
                IExpressionManager manager,
                IASTConverter converter
            )
        {
            _astGetter = astGetter;
            _validator = validator;
            _symbolTableManager = symbolTableManager;
            _expressionEvaluator = expressionEvaluator;
            _expressionManager = manager;
            _astConverter = converter;
        }

        public ExpressionEvaluatingServiceResult Evaluate(string input)
        {
            Error err = _validator.ValidateExpressionInputIsNotNull( input );
            if (err != null)
            {
                return new ExpressionEvaluatingServiceResult(null, err);
            }

            Expression expression = _expressionManager.CreateExpression(input);
            SymbolTable symbolTable = _symbolTableManager.GetSymbolTable();

            var result = _expressionEvaluator.Evaluate(expression.Value, symbolTable);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, result.Error);
            }

            expression.FSharpAST = TemporarySetAst(input);

            return new ExpressionEvaluatingServiceResult(result.EvaluationResult, null);
        }

        public ExpressionEvaluatingServiceResult Differentiate(string input) 
        {
            Error err = _validator.ValidateExpressionInputIsNotNull(input);
            if (err != null)
            {
                return new ExpressionEvaluatingServiceResult(null, err);
            }

            Expression expression = _expressionManager.CreateExpression(input);

            expression.FSharpAST = TemporarySetAst(input);

            var result = _expressionManager.Differentiate(expression);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, result.Error);
            }

            expression.FSharpAST = result.AST;
            expression.CSharpAST = _astConverter.Convert(expression.FSharpAST);
            string derivative = _astConverter.ConvertToString(expression.CSharpAST);

            return new ExpressionEvaluatingServiceResult(derivative, null);
        }

        public VisualiseASTResult VisualiseAST(string expression)
        {
            Error err = _validator.ValidateExpressionInputIsNotNull(expression);
            if (err != null)
            {
                return new VisualiseASTResult(null, err);
            }

            var result = _astGetter.GetAST(expression);
            if(result.HasError)
            {
                return new VisualiseASTResult(null, result.Error);
            }

            var convertedAST = _astConverter.Convert(result.AST);
            var graphAST = _astConverter.ConvertAstToGraph(convertedAST);

            return new VisualiseASTResult(graphAST, null);
        }

        // @TODO: Refactor once Evaluator is switched to AST Evaluator.
        private FSharpAST TemporarySetAst(string input)
        {
            var tokens = Engine.Tokeniser.tokenise(input);
            return Engine.ASTParser.parse(tokens.ResultValue).ResultValue;
        }
    }
}
