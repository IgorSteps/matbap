
namespace app
{
    public interface IValidator
    {
        public Error ValidatePlotInput(double xmin, double xmax, double xstep);
        public Error ValidateAddTangentInput(double x, double xmin, double xmax, double xstep);
        public Error ValidateExpressionInputIsNotNull(string input);
        public Error ValidateFindRootsInput(string input, double xmin, double xmax);
        public Error ValidateShowAreaUnderCurveInput(string input, double min, double max, double step);

    }
}
