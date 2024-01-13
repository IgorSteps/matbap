namespace app
{
    public struct DifferentiationServiceResult
    {
        public string Derivative { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public DifferentiationServiceResult(string res, Error err)
        {
            Derivative = res;
            Error = err;
        }
    }

    /// <summary>
    /// Differentiation service consolidates functionality to differentiate an expression.
    /// </summary>
    public class DifferentiationService : IDifferentiator
    {
        private readonly IValidator _validator;
        private readonly IASTConverter _astConverter;
        private readonly IExpressionManager _expressionManager;
        private readonly IFSharpASTGetterWrapper _astGetter;

        public DifferentiationService(
                IFSharpASTGetterWrapper astGetter,
                IValidator validator,
                IExpressionManager manager,
                IASTConverter converter
            ) 
        {
            _astGetter = astGetter;
            _validator = validator;
            _expressionManager = manager;
            _astConverter = converter;
        }

        public DifferentiationServiceResult Differentiate(string input)
        {
            // Validate.
            Error err = _validator.ValidateExpressionInputIsNotNull(input);
            if (err != null)
            {
                return new DifferentiationServiceResult(null, err);
            }

            // Create expression.
            Expression expression = _expressionManager.CreateExpression(input);

            // Get expression's AST and set expression's FSharpAST.
            var getASTResult = _astGetter.GetAST(expression.Value);
            if (getASTResult.HasError)
            {
                return new DifferentiationServiceResult(null, getASTResult.Error);
            }
            expression.FSharpAST = getASTResult.AST;

            // Differentiate expression and set expression's AST to derived AST..
            var diffResult = _expressionManager.Differentiate(expression);
            if (diffResult.HasError)
            {
                return new DifferentiationServiceResult(null, diffResult.Error);
            }
            expression.FSharpAST = diffResult.AST;
            
            // Convert to C# AST and set expression's C# AST.
            var convertionResult = _astConverter.Convert(expression.FSharpAST);
            if (convertionResult.HasError)
            {
                return new DifferentiationServiceResult(null, convertionResult.Error);
            }
            expression.CSharpAST = convertionResult.AST;

            // Convert derivative AST to string.
            string derivative = _astConverter.ConvertToString(expression.CSharpAST);

            return new DifferentiationServiceResult(derivative, null);
        }
    }
}
