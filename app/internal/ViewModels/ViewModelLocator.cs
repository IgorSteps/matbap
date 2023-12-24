﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace app
{
    public class ViewModelLocator
    {
        public InterpretationViewModel InterpretationViewModel => App.Current.Services.GetService<InterpretationViewModel>();
        public HelpViewModel HelpViewModel => App.Current.Services.GetService<HelpViewModel>();
        public PlotViewModel PlotViewModel => App.Current.Services.GetService<PlotViewModel>();
        public ASTViewModel ASTViewModel => App.Current.Services.GetService<ASTViewModel>();
    }
}