using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using static Engine.Evaluator;

namespace app
{
    public class EvaluationResult
    {
        public EvaluationResult(double[][] pPositive, double[][] pNegative, string err)
        {
            PositiveSegment = pPositive;
            NegativeSegment = pNegative;
            Error = err;
        }

        public double[][] PositiveSegment { get; set; }
        public double[][] NegativeSegment { get; set; }
        public string Error { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }

    public class PlotEquationEvaluationService : IPlotEquationEvaluator
    {
        public EvaluationResult Evaluate(double min, double max, double step, string equation)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, equation);

            if (result.IsOk)
            {
                switch (result.ResultValue)
                {
                    case EvaluationSegment.SingleSegment singleSegment:
                        return new EvaluationResult(singleSegment.Item, null, null);
                    case EvaluationSegment.MultipleSegments multipleSegments:
                        var segments = multipleSegments.Item;
                        if (segments.Length == 2)
                        {
                            return new EvaluationResult(segments[0], segments[1], null);
                        }
                        else
                        {
                            return new EvaluationResult(null, null, "Unexpected number of segments");
                        }
                    default:
                        return new EvaluationResult(null, null, "Unknown segment type");
                }
            }
            else
            {
                return new EvaluationResult(null, null, result.ErrorValue);
            }
        }
    }
    
}