namespace app
{
    public interface IFSharpIntegratorWrapper
    {
        public FSharpIntegratorResult CalculateAreaUnderCurve(string function, double min, double max, double step);
    }
}
