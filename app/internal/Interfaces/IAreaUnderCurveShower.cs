using OxyPlot;

namespace app
{
    public interface IAreaUnderCurveShower
    {
        public CalculateAreaUnderCurveResult ShowAreaUnderCurve(PlotModel plotModel, Plot plot, double step);
        public void ClearTrapeziumList();
    }
}
