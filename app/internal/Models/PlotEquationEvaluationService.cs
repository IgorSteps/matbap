using Microsoft.FSharp.Core;
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
        public double[][] Evaluate(double min, double max, double step, string equation)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, equation);
            if (result.IsOk)
            {
                return result.ResultValue;
            }
            else
            {
                throw new InvalidOperationException($"Error: {result.ErrorValue}");
            }
        }
    }
}