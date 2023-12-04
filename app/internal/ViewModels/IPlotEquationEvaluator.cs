using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public interface IPlotEquationEvaluator
    {
        public float[,] Evaluate(float min, float max, float step, string equation);
    }
}
