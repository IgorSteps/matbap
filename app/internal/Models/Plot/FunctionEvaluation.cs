using Microsoft.FSharp.Core;
using Microsoft.Msagl.Core.Geometry.Curves;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
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
        public Error Error { get; set; }
        public readonly bool HasError => Error != null;

        public EvaluationResult(double[][] p, Error err)
        {
            Points = p;
            Error = err;
        }
    }

    public class FunctionEvaluation: IFunctionEvaluator
    {
        public EvaluationResult Evaluate(string function, double min, double max, double step)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, function);
            if (result.IsError)
            {
                return new EvaluationResult(null, new Error(result.ErrorValue));
            }
           
            return new EvaluationResult(result.ResultValue, null);
        }

        public EvaluationResult EvaluateAtPoint(double x, string function)
        {
            var result = Evaluate(function, x, x, x);
            if (result.HasError)
            {
                return new EvaluationResult(null, result.Error);
            }

            return new EvaluationResult(result.Points, null);
        }

        // @TODO: Switch with implementation in F#.
        public double TakeDerivative(double x, string function) 
        {
            double h = 1e-5; // A small number for the central difference calculation
            // Error checking is missing.
            double yPlus = Evaluate(function, x + h, x + h, 1).Points[0][1];
            double yMinus = Evaluate(function, x - h, x - h, 1).Points[0][1];

            return (yPlus - yMinus) / (2 * h);
        }
    }
}