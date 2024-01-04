
namespace app
{
    public interface IValidator
    {
        public Error ValidatePlotInput(double xmin, double xmax, double xstep);
        public Error ValidateAddTangentInput(double x, double xmin, double xmax, double xstep);
        public Error ValidateExpressionInputIsNotNull(string input);
    }
}
