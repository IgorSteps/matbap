
using OxyPlot;

namespace app
{
    public struct CalculateAreaUnderCurveResult
    { 
        public double Area { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public CalculateAreaUnderCurveResult(double area, Error err)
        {
            Area = area;
            Error = err;
        }
    }

    public class IntegrationService : IAreaUnderCurveShower
    {
        private readonly IFSharpIntegratorWrapper _integratorWrapper;
        private readonly ITrapeziumManager _trapeziumManager;
        private readonly IOxyPlotModelManager _oxyPlotModelManager;
        private readonly IValidator _validator;

        public IntegrationService(
            IFSharpIntegratorWrapper integratorWrapper,
            ITrapeziumManager trapeziumManager,
            IOxyPlotModelManager oxyPlotModelManager,
            IValidator validator
            )
        {
            _integratorWrapper = integratorWrapper;
            _trapeziumManager = trapeziumManager;
            _oxyPlotModelManager = oxyPlotModelManager;
            _validator = validator;
        }

        /// <summary>
        /// Adds trapeziums to the OxyPlot plot model and returns total area or error.
        /// </summary>
        public CalculateAreaUnderCurveResult ShowAreaUnderCurve(PlotModel plotModel, Plot plot, double step)
        {
            Error err = _validator.ValidateShowAreaUnderCurveInput(plot.Function, plot.XMin, plot.XMax, step);
            if (err != null)
            {
                return new CalculateAreaUnderCurveResult(0, err);
            }

            var integrationResult = _integratorWrapper.CalculateAreaUnderCurve(plot.Function, plot.XMin, plot.XMax, step);
            if (integrationResult.HasError)
            {
                return new CalculateAreaUnderCurveResult(0, integrationResult.Error);
            }

            if (integrationResult.Vertices.Length == 0)
            {
                return new CalculateAreaUnderCurveResult(0, new Error("No vertices found for given input, adjust min, max or step size."));
            }

            err = CreateTrapeziums(integrationResult);
            if (err != null)
            {
                return new CalculateAreaUnderCurveResult(0, err);
            }

            // Add series ot OxyPlot Plot Model.
            foreach (var series in _trapeziumManager.GetAllTrapeziumSeries())
            {
                _oxyPlotModelManager.AddSeriesToPlotModel(plotModel, series);
            }

            return new CalculateAreaUnderCurveResult(integrationResult.Area, null);
        }

        public void ClearTrapeziumList()
        {
            _trapeziumManager.Trapeziums.Clear();
        }

        private Error CreateTrapeziums(FSharpIntegratorResult integrationResult)
        {
            // Create trapeziums.
            for (int i = 0; i < integrationResult.Vertices.Length; i += 4)
            {
                // Check there are enough vertices to form a trapezium(4).
                // If the range (min to max) is not perfectly divisible by the step,
                // we might end up with a scenario where the last segment doesn't form complete trapezium.
                // ie. range is 0 to 10 with a step 3, our last segment will only reach 9.
                if (i + 3 < integrationResult.Vertices.Length)
                {
                    var v1 = integrationResult.Vertices[i];
                    var v2 = integrationResult.Vertices[i + 1];
                    var v3 = integrationResult.Vertices[i + 2];
                    var v4 = integrationResult.Vertices[i + 3];

                    _trapeziumManager.CreateTrapezium(
                        v1[0], v1[1],
                        v2[0], v2[1],
                        v3[0], v3[1],
                        v4[0], v4[1]
                        );
                }
                else
                {
                    return new Error("Not enough vertices to form a trapezium, please adjust your step size.");
                }
            }

            return null;
        }
    }
}
