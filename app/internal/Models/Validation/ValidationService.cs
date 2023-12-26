using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public class ValidationService : IValidator
    {
        const string tangentXZeroMsg = "Tangent's X can't be 0";
        const string xMinGreaterXMax = "XMin can't be greater than XMax";
        const string tangentXRangeMsg = "Tangent's X must be in the range [XMin, XMax]";
        const string xStepZeroMsg = "XStep can't be 0";

        public ValidationService() { }
        public string ValidatePlotInput(double xmin, double xmax, double xstep)
        {
            if (xmin > xmax)
            {
                return xMinGreaterXMax;
            }
            if (xstep == 0)
            {
                return xStepZeroMsg;
            }

            return null;
        }

        public string ValidateAddTangentInput(double x, double xmin, double xmax, double xstep)
        {
            if (x == 0)
            {
                return tangentXZeroMsg;
            }
            if (xmin > xmax)
            {
                return xMinGreaterXMax;
            }
            if (x < xmin || x > xmax) 
            {
                return tangentXRangeMsg;
            }
            if (xstep == 0)
            {
                return xStepZeroMsg;
            }
            

            return null;
        }
    }
}
