using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public interface IPlotter
    {
        public PlotResult CreatePlot(string function, double xmin, double xmax, double xstep);
        public void ClearPlots();
    }
}
