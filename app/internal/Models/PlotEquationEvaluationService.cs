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
        public float[,] Evaluate(float min, float max, float step, string equation)
        {
            var result = plotPoints(min, max, step, equation);
            if (result.IsOk)
            {
                return result.ResultValue;
            }
            else
            {
                throw new InvalidOperationException($"Error: {result.ErrorValue}");
            }
        }

        private FSharpResult<float[,], string> plotPoints(float min, float max, float step, string exp) {
            return null;
        }
    }
}

public interface IStub
{
    public FSharpResult<float [,], string> plotPoints(float min, float max, float step, string exp);
}-