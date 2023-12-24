using Microsoft.Extensions.DependencyInjection;
using System;

namespace app
{
    public class ViewModelLocator
    {
        public InterpretationViewModel InterpretationViewModel => App.Current.Services.GetService<InterpretationViewModel>();
        public TokenHelpViewModel TokenHelpViewModel => App.Current.Services.GetService<TokenHelpViewModel>();
        public PlotViewModel PlotViewModel => App.Current.Services.GetService<PlotViewModel>();
        public ASTViewModel ASTViewModel => App.Current.Services.GetService<ASTViewModel>();
    }
}
