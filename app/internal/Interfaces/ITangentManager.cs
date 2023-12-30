namespace app
{
    /// <summary>
    /// Interface for managing tangents.
    /// </summary>
    public interface ITangentManager
    {
        public CreateTangentResult CreateTangent(double x, string function);
        public GetTangentLineSeriesResult GetTangentLineSeries(Tangent tangent, double xmin, double xmax, double xstep);
    }
}
