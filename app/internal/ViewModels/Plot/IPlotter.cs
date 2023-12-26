using OxyPlot;
using System.Collections.Generic;

namespace app
{
    public interface IPlotter
    {
        public PlotResult CreatePlot(string function, double xmin, double xmax, double xstep);

        public void ClearPlots();

        public PlotResult AddTangent(double x, string function, double xmin, double xmax, double xstep);
    }
}
