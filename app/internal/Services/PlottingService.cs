using OxyPlot;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using static Engine.Types.Node;

namespace app
{
    public struct CreatePlotResult
    {
        public Plot Plot { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public CreatePlotResult(Plot plot, Error error)
        {
            Plot = plot;
            Error = error;
        }
    }

    public struct AddTangentResult
    {
        public Tangent Tangent { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public AddTangentResult(Tangent t, Error error)
        {
            Tangent = t;
            Error = error;
        }
    }
    /// <summary>
    /// PlottingService provides functionality for creating, manipulating and validating plot data.
    /// </summary>
    public class PlottingService : IPlotter
    {
        private readonly IValidator _validator;
        private readonly IOxyPlotModelManager _oxyPlotModelManager;
        private readonly IPlotManager _plotManager;
        private readonly ITangentManager _tangentManager;
        private readonly IExpressionManager _expressionManager;

        public PlottingService(
                IValidator validator,
                IOxyPlotModelManager modelManager,
                IPlotManager plotManager,
                ITangentManager tangentManager,
                IExpressionManager exprManager
            )
        {
            _validator = validator;
            _oxyPlotModelManager = modelManager;
            _plotManager = plotManager;
            _tangentManager = tangentManager;
            _expressionManager = exprManager;
        }

        /// <summary>
        /// Validate user input;
        /// Create a plot on the OxyPlot Plot Model.
        /// </summary>
        public CreatePlotResult CreatePlot(PlotModel plotModel, string function, double xmin, double xmax, double xstep)
        {
            Error err = _validator.ValidatePlotInput(xmin, xmax, xstep);
            if (err != null)
            {
                return new CreatePlotResult(null, err);
            }
            
            Plot newPlot = _plotManager.CreatePlot(function, xmin, xmax, xstep);
            var result = _plotManager.GetLineSeriesForPlot(newPlot);
            if (result.HasError)
            {
                return new CreatePlotResult(null, result.Error);
            }

            _oxyPlotModelManager.AddSeriesToPlotModel(plotModel, result.LineSeries);
            _oxyPlotModelManager.SetupAxisOnPlotModel(plotModel, xmin, xmax);

            return new CreatePlotResult(newPlot, null);
        }

        /// <summary>
        /// Create plot from expression that has Points in it after user
        /// enter a for-loop with plot function.
        /// </summary>
        public CreatePlotResult CreatePlotFromExpression(PlotModel plotModel, Expression expression)
        {
            int length = expression.Points.Length;
            double xmin = expression.Points[0][0];
            double xmax = expression.Points[length - 1][0];
            double xstep = xmin - expression.Points[0][1];
            Plot newPlot = _plotManager.CreatePlot(expression.Value, xmin, xmax, xstep);

            LineSeries lineSeries = _expressionManager.GetLineSeriesFromExpression(expression);
            _oxyPlotModelManager.AddSeriesToPlotModel(plotModel, lineSeries);
            _oxyPlotModelManager.SetupAxisOnPlotModel(plotModel, xmin, xmax);

            return new CreatePlotResult(newPlot, null);
        }

        /// <summary>
        /// Validate user input;
        /// Add Tangent at point x for a function on the OxyPlot Plot Model.
        /// </summary>
        public AddTangentResult AddTangent(PlotModel plotModel, double x, string function, double xmin, double xmax, double xstep)
        {
            Error err = _validator.ValidateAddTangentInput(x, xmin, xmax, xstep);
            if (err != null)
            {
                return new AddTangentResult(null, err);
            }

            var createResult = _tangentManager.CreateTangent(x, function);
            if (createResult.HasError)
            {
                return new AddTangentResult(null, createResult.Error); ;
            }

            var result = _tangentManager.GetTangentLineSeries(createResult.Tangent, xmin, xmax, xstep);
            if (result.HasError)
            {
                return new AddTangentResult(null, result.Error); ;
            }

            _oxyPlotModelManager.AddSeriesToPlotModel(plotModel, result.LineSeries);
            _oxyPlotModelManager.SetupAxisOnPlotModel(plotModel, xmin, xmax);

            return new AddTangentResult(createResult.Tangent, null);
        }
    }
}
