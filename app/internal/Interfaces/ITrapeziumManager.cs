using OxyPlot.Series;
using System.Collections.Generic;

namespace app
{
    public interface ITrapeziumManager
    {
        List<Trapezium> Trapeziums { get; }
        public Trapezium CreateTrapezium(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);
        public List<LineSeries> GetAllTrapeziumSeries();
    }
}
