
namespace app
{
    public interface IValidator
    {
        public string ValidatePlotInput(double xmin, double xmax, double xstep);
        public string ValidateAddTangentInput(double x, double xmin, double xmax, double xstep);
    }
}
