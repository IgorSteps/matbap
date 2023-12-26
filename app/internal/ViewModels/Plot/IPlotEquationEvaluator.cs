using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public interface IPlotEquationEvaluator
    {
        public EvaluationResult Evaluate(double min, double max, double step, string equation);
        public double TakeDerivative(double x, string function);
    }
}
