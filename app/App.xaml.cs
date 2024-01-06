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

            // F# Evaluator Wrappers.
            //services.AddTransient<Engine.IEvaluator, Engine.EvaluatorWrapper>();
            services.AddTransient<Engine.IDifferentiator, Engine.DifferentiatorWrapper>();
            services.AddTransient<Engine.IASTGetter, Engine.ASTGetterWrapper>();   

            // Managers.
            services.AddSingleton<IOxyPlotModelManager, OxyPlotModelManager>();
            services.AddSingleton<ISymbolTableManager, SymbolTableManager>(); // Must be singleton, because manages symbol table.
            services.AddSingleton<IExpressionManager, ExpressionManager>();
            services.AddSingleton<IPlotManager, PlotManager>();
            services.AddSingleton<ITangentManager, TangentManager>();

            // C# Wrappers.
            services.AddSingleton<IFSharpFunctionEvaluatorWrapper, FSharpFunctionEvaluatiorWrapper>();
            services.AddSingleton<IFSharpDifferentiatorWrapper, FSharpDifferentiatorWrapper>();
            services.AddSingleton<IFSharpASTGetterWrapper, FSharpASTGetterWrapper>();

            // Services.
            services.AddSingleton<IASTConverter, ASTManager>();
            services.AddSingleton<IPlotter, PlottingService>();
            services.AddSingleton<IValidator, ValidationService>();
            services.AddSingleton<IFSharpEvaluatorWrapper, FSharpEvaluatorWrapper>();
            services.AddSingleton<IEvaluator, ExpressionEvaluatingService>(); 

            // ViewModels.
            services.AddTransient<ExpressionViewModel>();
            services.AddTransient<HelpViewModel>();
            services.AddTransient<PlotViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
