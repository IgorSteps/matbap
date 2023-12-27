namespace app
{
    /// <summary>
    /// Represents a plot.
    /// </summary>
    public class Plot
    {
        public string Function { get; private set; }
        public double XMin { get; private set; }
        public double XMax { get; private set; }
        public double XStep { get; private set; }
        public Tangent Tangent { get; set; }

        public Plot(string function, double xmin, double xmax, double xstep)
        {
            Function = function;
            XMin = xmin;
            XMax = xmax;
            XStep = xstep;
        }
    }
}
