using Microsoft.Extensions.DependencyInjection;
using System;

namespace app
{
    /// <summary>
    /// Singleton locator for view models.
    /// Instantied as a Resource in the Application.Resources.
    /// </summary>
    public class ViewModelLocator
    {
        public ExpressionViewModel ExpressionViewModel => App.Current.Services.GetService<ExpressionViewModel>();
        public HelpViewModel HelpViewModel => App.Current.Services.GetService<HelpViewModel>();
        public PlotViewModel PlotViewModel => App.Current.Services.GetService<PlotViewModel>();
    }
}
