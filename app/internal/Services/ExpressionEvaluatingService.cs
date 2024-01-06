﻿using Microsoft.Msagl.Drawing;
using OxyPlot;
using static app.ASTManager;
using FSharpAST = Engine.Types.Node;

namespace app
{
    public struct ExpressionEvaluatingServiceResult
    {
        public string Result { get; private set; }
        public Expression Expression { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public ExpressionEvaluatingServiceResult(string res, Expression exp, Error err) 
        {
            Result = res;
            Expression = exp;
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
        private readonly IFSharpFindRootsWrapper _findRootsWrapper;

        public ExpressionEvaluatingService(
                IFSharpASTGetterWrapper astGetter,
                IValidator validator,
                ISymbolTableManager symbolTableManager,
                IFSharpEvaluatorWrapper expressionEvaluator,
                IExpressionManager manager,
                IASTConverter converter,
                IFSharpFindRootsWrapper findrootWrapper
            )
        {
            _astGetter = astGetter;
            _validator = validator;
            _symbolTableManager = symbolTableManager;
            _expressionEvaluator = expressionEvaluator;
            _expressionManager = manager;
            _astConverter = converter;
            _findRootsWrapper = findrootWrapper;
        }

        public ExpressionEvaluatingServiceResult Evaluate(string input)
        {
            Error err = _validator.ValidateExpressionInputIsNotNull( input );
            if (err != null)
            {
                return new ExpressionEvaluatingServiceResult(null, null, err);
            }

            Expression expression = _expressionManager.CreateExpression(input);
            SymbolTable symbolTable = _symbolTableManager.GetSymbolTable();

            var result = _expressionEvaluator.Evaluate(expression.Value, symbolTable);
            if (result.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, null, result.Error);
            }

            if (result.Points.Length != 0)
            {
                expression.Points = result.Points;
            }

            expression.FSharpAST = result.FSharpAST;

            return new ExpressionEvaluatingServiceResult(result.Answer, expression, null);
        }

        public ExpressionEvaluatingServiceResult Differentiate(string input) 
        {
            Error err = _validator.ValidateExpressionInputIsNotNull(input);
            if (err != null)
            {
                return new ExpressionEvaluatingServiceResult(null, null, err);
            }

            Expression expression = _expressionManager.CreateExpression(input);

            var getASTResult = _astGetter.GetAST(expression.Value);
            if (getASTResult.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, null, getASTResult.Error);
            }
            expression.FSharpAST = getASTResult.AST;

            var diffResult = _expressionManager.Differentiate(expression);
            if (diffResult.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, null, diffResult.Error);
            }

            expression.FSharpAST = diffResult.AST;
            var convertionResult = _astConverter.Convert(expression.FSharpAST);
            if (convertionResult.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, null, convertionResult.Error);
            }

            expression.CSharpAST = convertionResult.AST;
            string derivative = _astConverter.ConvertToString(expression.CSharpAST);

            return new ExpressionEvaluatingServiceResult(derivative, expression, null);
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

            var convetionResult = _astConverter.Convert(result.AST);
            if (convetionResult.HasError)
            {
                return new VisualiseASTResult(null, convetionResult.Error);
            }
            var graphAST = _astConverter.ConvertAstToGraph(convetionResult.AST);

            return new VisualiseASTResult(graphAST, null);
        }

        public ExpressionEvaluatingServiceResult FindRoots(string expression, double xmin, double xmax)
        {
            Error err = _validator.ValidateFindRootsInput(expression, xmin, xmax);
            if (err != null)
            {
                return new ExpressionEvaluatingServiceResult(null, null, err);
            }

            var findRootsResult = _findRootsWrapper.FindRoots(expression, xmin, xmax);
            if (findRootsResult.HasError)
            {
                return new ExpressionEvaluatingServiceResult(null, null, findRootsResult.Error);
            }

            return new ExpressionEvaluatingServiceResult(findRootsResult.Answer, null, null); ;
        }
    }
}
