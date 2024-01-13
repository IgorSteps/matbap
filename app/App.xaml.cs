using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace app
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current App instance in use.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the IServiceProvider instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            /*
             IMPORTANT NOTE ON DEPENDECY INJECTION:
                Because View Models are retained when the UI element is first loaded,
                they will NOT be re-instantiated unless explicitly done so.
                We don't explicetly reinitilise ViewModels, so whatever you set services to be here,
                they will act as Singletons.
             */

            // F# Evaluator Wrappers.
            services.AddTransient<Engine.IEvaluator, Engine.EvaluatorWrapper>();
            services.AddTransient<Engine.IDifferentiator, Engine.DifferentiatorWrapper>();
            services.AddTransient<Engine.IASTGetter, Engine.ASTGetterWrapper>();   
            services.AddTransient<Engine.IRootFinder, Engine.RootFinderWrapper>();  
            services.AddTransient<Engine.IIntegrator, Engine.IntegratorWrapper>();

            // Managers.
            services.AddTransient<IOxyPlotModelManager, OxyPlotModelManager>();
            services.AddSingleton<ISymbolTableManager, SymbolTableManager>(); // Must be singleton, because manages symbol table.
            services.AddSingleton<IExpressionManager, ExpressionManager>();
            services.AddSingleton<IPlotManager, PlotManager>();
            services.AddSingleton<ITangentManager, TangentManager>();
            services.AddSingleton<IASTConverter, ASTManager>();
            services.AddTransient<ITrapeziumManager, TrapeziumManager>(); 

            // C# Wrappers.
            services.AddSingleton<IFSharpFunctionEvaluatorWrapper, FSharpFunctionEvaluatiorWrapper>();
            services.AddSingleton<IFSharpDifferentiatorWrapper, FSharpDifferentiatorWrapper>();
            services.AddSingleton<IFSharpASTGetterWrapper, FSharpASTGetterWrapper>();
            services.AddSingleton<IFSharpEvaluatorWrapper, FSharpEvaluatorWrapper>();
            services.AddSingleton<IFSharpFindRootsWrapper, FSharpFindRootsWrapper>();
            services.AddTransient<IFSharpIntegratorWrapper, FSharpIntegratorWrapper>();

            // Services.
            services.AddSingleton<IPlotter, PlottingService>();
            services.AddSingleton<IDifferentiator, DifferentiationService>();
            services.AddTransient<IValidator, ValidationService>();
            services.AddSingleton<IEvaluator, ExpressionEvaluatingService>(); 
            services.AddSingleton<IRootFinder, FindRootsService>();
            services.AddTransient<IAreaUnderCurveShower, IntegrationService>();

            // ViewModels.
            services.AddTransient<ExpressionViewModel>();
            services.AddTransient<HelpViewModel>();
            services.AddTransient<PlotViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
