
using OxyPlot;

namespace app
{

    public class IntegrationService
    {
        private readonly FSharpIntegratorWrapper _integratorWrapper;
        private readonly TrapeziumManager _trapeziumManager;
        private readonly IOxyPlotModelManager _oxyPlotModelManager;

        public IntegrationService(FSharpIntegratorWrapper integratorWrapper, TrapeziumManager trapeziumManager, IOxyPlotModelManager oxyPlotModelManager)
        {
            _integratorWrapper = integratorWrapper;
            _trapeziumManager = trapeziumManager;
            _oxyPlotModelManager = oxyPlotModelManager;
        }

        public Error ShowAreaUnderCurve(PlotModel plotModel, string function, double min, double max, double step)
        {
            var integrationresult = _integratorWrapper.CalculateAreaUnderCurve(function, min, max, step);
            if (integrationresult.HasError)
            {
                return integrationresult.Error;
            }

            foreach (var vertices in integrationresult.Vertices)
            {
                // Assuming each trapezium is represented by 4 vertices
                _trapeziumManager.CreateTrapezium(
                    vertices[0][0], vertices[0][1],
                    vertices[1][0], vertices[1][1],
                    vertices[2][0], vertices[2][1],
                    vertices[3][0], vertices[3][1]
                    );
            }
            foreach (var series in _trapeziumManager.GetAllTrapeziumSeries())
            {
                _oxyPlotModelManager.AddSeriesToPlotModel(plotModel, series);
            }

            return null;
        }
    }
}
