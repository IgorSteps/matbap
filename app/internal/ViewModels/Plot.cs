using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

namespace app
{
    public class PlotViewModel : ObservableObject
    {

        public PlotViewModel()
        {
            MyPlotModel = new PlotModel { Title = "Example Plot" };
            this.MyPlotModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }
        public PlotModel MyPlotModel { get; private set; }
    }
}
