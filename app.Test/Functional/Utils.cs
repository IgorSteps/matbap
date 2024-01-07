﻿
namespace app.Test.Functional
{
    public class Utils
    {
        public static PlottingService CreatePlottingService()
        {
            // F# wrappers.
            Engine.EvaluatorWrapper engineEvaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            FSharpFunctionEvaluatiorWrapper functionEvaluatorWrapper = new FSharpFunctionEvaluatiorWrapper(engineEvaluatorWrapper);
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(engineEvaluatorWrapper);

            var expessionManager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var expressionEvaluatorService = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, expessionManager, converter);

            PlotManager plotManager = new PlotManager(functionEvaluatorWrapper);
            TangentManager tangentManager = new TangentManager(functionEvaluatorWrapper, expressionEvaluatorService);
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager, expessionManager);
            return plotter;
        }

        public static ExpressionEvaluatingService CreateExpressionEvalutingService()
        {
            // F# wrappers.
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(evaluatorWrapper);

            var manager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var service = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, manager, converter);
            return service;
        }

        public static ExpressionViewModel CreateExpressionViewModel()
        {
            // F# wrappers.
            Engine.EvaluatorWrapper evaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var fsharpEvalWrapper = new FSharpEvaluatorWrapper(evaluatorWrapper);

            var expressionManager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var astManager = new ASTManager();
            var validator = new ValidationService();

            var service = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, fsharpEvalWrapper, expressionManager, astManager);
            
            var viewModel = new ExpressionViewModel(service, CreateFindRootsService());
            return viewModel;
        }

        public static FindRootsService CreateFindRootsService()
        {
            var validator = new ValidationService();
            var FSharpFindRootsWrapper = new FSharpFindRootsWrapper();

            var service = new FindRootsService(FSharpFindRootsWrapper, validator);
            return service;
        }

        public static PlotViewModel CreaePlotViewModel()
        {
            // F# wrappers.
            Engine.EvaluatorWrapper engineEvaluatorWrapper = new Engine.EvaluatorWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();

            // C# wrappers.
            FSharpFunctionEvaluatiorWrapper functionEvaluatorWrapper = new FSharpFunctionEvaluatiorWrapper(engineEvaluatorWrapper);
            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);
            var evaluator = new FSharpEvaluatorWrapper(engineEvaluatorWrapper);

            var expessionManager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var symTableManager = new SymbolTableManager();
            var converter = new ASTManager();
            var validator = new ValidationService();

            var expressionEvaluatorService = new ExpressionEvaluatingService(fSharpASTGetterWrapper, validator, symTableManager, evaluator, expessionManager, converter);

            PlotManager plotManager = new PlotManager(functionEvaluatorWrapper);
            TangentManager tangentManager = new TangentManager(functionEvaluatorWrapper, expressionEvaluatorService);
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager, expessionManager);
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager);
            return plotViewModel;
        }
    }
}
