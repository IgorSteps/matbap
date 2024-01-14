
using Engine;

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
            TangentManager tangentManager = new TangentManager(functionEvaluatorWrapper, CreateDifferentiationService());
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager, expessionManager);
            return plotter;
        }
        public static DifferentiationService CreateDifferentiationService()
        {
            Engine.ASTGetterWrapper astGetter = new Engine.ASTGetterWrapper();
            Engine.DifferentiatorWrapper differentiatorWrapper = new Engine.DifferentiatorWrapper();

            var fsharpDifferentiatorWrapper = new FSharpDifferentiatorWrapper(differentiatorWrapper);
            var fSharpASTGetterWrapper = new FSharpASTGetterWrapper(astGetter);

            var expessionManager = new ExpressionManager(fsharpDifferentiatorWrapper);
            var converter = new ASTManager();
            var validator = new ValidationService();

            var service = new DifferentiationService(fSharpASTGetterWrapper, validator, expessionManager,converter);

            return service;
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
            
            var viewModel = new ExpressionViewModel(service, CreateFindRootsService(), CreateDifferentiationService(), symTableManager);
            return viewModel;
        }

        public static FindRootsService CreateFindRootsService()
        {
            var validator = new ValidationService();
            Engine.RootFinderWrapper rootWrapper = new Engine.RootFinderWrapper();
            var FSharpFindRootsWrapper = new FSharpFindRootsWrapper(rootWrapper);

            var service = new FindRootsService(FSharpFindRootsWrapper, validator);
            return service;
        }

        public static IntegrationService CreateIntegrationService()
        {
            var validator = new ValidationService();
            Engine.IntegratorWrapper wrapper = new Engine.IntegratorWrapper();
            var fSharpIntegratorWrapper = new FSharpIntegratorWrapper(wrapper);
            var trapeziumManager = new TrapeziumManager();
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();

            var service = new IntegrationService(fSharpIntegratorWrapper, trapeziumManager, oxyPlotModelManager, validator);
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
            TangentManager tangentManager = new TangentManager(functionEvaluatorWrapper, CreateDifferentiationService());
            OxyPlotModelManager oxyPlotModelManager = new OxyPlotModelManager();
            PlottingService plotter = new PlottingService(validator, oxyPlotModelManager, plotManager, tangentManager, expessionManager);
            
            PlotViewModel plotViewModel = new PlotViewModel(plotter, oxyPlotModelManager, CreateIntegrationService());
            return plotViewModel;
        }
    }
}
