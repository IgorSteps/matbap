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
        /// <summary>
        /// Returns points in [x,y] format.
        /// </summary>
        public double[][] Points { get; set; }
        public string Error { get; set; }
        public readonly bool HasError => !string.IsNullOrEmpty(Error);

        public EvaluationResult(double[][] p, string err)
        {
            Points = p;
            Error = err;
        }
    }

    public class PlotEquationEvaluationService: IPlotEquationEvaluator
    { 
        public EvaluationResult Evaluate(double min, double max, double step, string equation)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, equation);
            return new EvaluationResult(result.ResultValue, result.ErrorValue);
        }

        // @TODO: Switch with implementation in F#.
        public double TakeDerivative(double x, string function) 
        {
            double h = 1e-5; // A small number for the central difference calculation
            double yPlus = Evaluate(x + h, x + h, 1, function).Points[0][1];
            double yMinus = Evaluate(x - h, x - h, 1, function).Points[0][1];

            return (yPlus - yMinus) / (2 * h);
        }
    }
}