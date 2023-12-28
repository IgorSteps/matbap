using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    /// <summary>
    /// Interface for managing plots.
    /// </summary>
    public interface IPlotManager
    {
        public Plot CreatePlot(string function, double xmin, double xmax, double xstep);
        public GetLineSeriesResult GetLineSeriesForPlot(Plot plot);
    }
}
