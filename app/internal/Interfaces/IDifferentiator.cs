namespace app
{
    /// <summary>
    /// Interface that provides functionality to differentiate an expression.
    /// Implemented by DifferentiationService.
    /// </summary>
    public interface IDifferentiator
    {
        public DifferentiationServiceResult Differentiate(string input);
    }
}
