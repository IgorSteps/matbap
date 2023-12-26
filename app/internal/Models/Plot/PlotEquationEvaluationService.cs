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
    public struct EvaluationResult
    {
        public EvaluationResult(double[][] p, string err)
        {
            Points = p;
            Error = err;
        }
        public double[][] Points { get; set; }
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }

    public class PlotEquationEvaluationService: IPlotEquationEvaluator
    { 
        public EvaluationResult Evaluate(double min, double max, double step, string equation)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, equation);
            return new EvaluationResult(result.ResultValue, result.ErrorValue);
        }

        public double TakeDerivative(double x, string function) 
        {
            throw new NotImplementedException();
        }
    }
}