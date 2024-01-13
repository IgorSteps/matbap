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

        public Error ValidateExpressionInputIsNotNull(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Error("Expression is null or empty.");
            }

            return null;
        }

        public Error ValidateFindRootsInput(string input, double xmin, double xmax)
        {
            if(string.IsNullOrEmpty(input))
            {
                return new Error("Expression cannot be null or empty");
            }
            if (xmin > xmax)
            {
                return xMinGreaterXMax;
            }

            return null;
        }

        public Error ValidateShowAreaUnderCurveInput(string input, double min, double max, double step)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Error("Input expression cannot be null or empty");
            }
            if(min > max)
            {
                return xMinGreaterXMax;
            }
            if(step == 0 )
            {
                return xStepZeroMsg;
            }

            return null;
        }
    }
}
