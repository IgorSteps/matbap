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

            // Models/Services.
            services.AddSingleton<IInterpretModel, InterpretationModel>();
            services.AddSingleton<IPlotEquationEvaluator, PlotEquationEvaluationService>();
            services.AddSingleton<IASTConverter, ASTConversionService>();

            // Viewmodels.
            services.AddTransient<InterpretationViewModel>();
            services.AddTransient<HelpViewModel>();
            services.AddTransient<PlotViewModel>();
            services.AddTransient<ASTViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
