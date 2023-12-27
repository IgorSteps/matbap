using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public class ValidationService : IValidator
    {
        Error tangentXZeroMsg = new Error("Tangent's X can't be 0");
        Error xMinGreaterXMax = new Error("XMin can't be greater than XMax");
        Error tangentXRangeMsg = new Error("Tangent's X must be in the range [XMin, XMax]");
        Error xStepZeroMsg = new Error("XStep can't be 0");

        public ValidationService() { }
        public Error ValidatePlotInput(double xmin, double xmax, double xstep)
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

        public Error ValidateAddTangentInput(double x, double xmin, double xmax, double xstep)
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
