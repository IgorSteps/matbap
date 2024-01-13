
namespace app
{
    public class Trapezium
    {
        public double X1 { get; private set; }
        public double Y1 { get; private set; }
        public double X2 { get; private set; }
        public double Y2 { get; private set; }
        public double X3 { get; private set; }
        public double Y3 { get; private set; }
        public double X4 { get; private set; }
        public double Y4 { get; private set; }
        public Trapezium(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4
            )
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
            X4 = x4;
            Y4 = y4;
        }
    }

}
