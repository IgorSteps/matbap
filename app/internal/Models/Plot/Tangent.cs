namespace app
{
    /// <summary>
    /// Represents a tangent line.
    /// </summary>
    public class Tangent
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Slope { get; private set; }
        public double YIntercept { get; private set; }

        /// <summary>
        /// Calculates the y-intercept based on the provided slope and y-coord of the tangent.
        /// </summary>
        public Tangent(double x, double y, double slope)
        {
            X = x;
            Y = y;
            Slope = slope;
            YIntercept = y - slope * x;
        }

        public string GetTangentEquation()
        {
            return $"{Slope} * x + {YIntercept}";
        }
    }
}
