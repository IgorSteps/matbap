using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public class ValidationService : IValidator
    {
        public ValidationService() { }
        public string ValidatePlotInput(double xmin, double xmax, double xstep)
        {
            if (xmin > xmax)
            {
                return "XMin can't be greater than XMax";
            }
            if (xstep == 0)
            {
                return "XStep can't be 0";
            }

            return null;
        }

        public string ValidateAddTangentInput(double x)
        {
            if (x == 0)
            {
                return "Tangent X can't be 0";
            }

            return null;
        }
    }
}
