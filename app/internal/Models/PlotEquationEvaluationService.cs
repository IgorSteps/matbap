using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace app
{

    public class PlotEquationEvaluationService: IPlotEquationEvaluator
    {
        public string Evaluate(string equation)
        {
            // Call to F# enginge to do its magic.
            return "";
        }
    }
}
