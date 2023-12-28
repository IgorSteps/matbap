﻿using OxyPlot.Series;
using OxyPlot;

namespace app
{
    public struct CreateTangentResult
    {
        public Tangent Tangent { get; private set;}
        public Error Error { get; private set;}
        public readonly bool HasError => Error != null;
        public CreateTangentResult(Tangent t, Error err)
        {
            Tangent = t;
            Error = err;
        }
    }
    public struct GetTangentLineSeriesResult
    {
        public LineSeries LineSeries { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public GetTangentLineSeriesResult(LineSeries l, Error err)
        {
            LineSeries = l;
            Error = err;
        }
    }

    /// <summary>
    /// Handles the creation and visualisation of tangent.
    /// </summary>
    public class TangentManager : ITangentManager
    {
        private readonly IFunctionEvaluator _functionEvaluator;

        public TangentManager(IFunctionEvaluator functionEvaluator)
        {
            _functionEvaluator = functionEvaluator;
        }

        public CreateTangentResult CreateTangent(double x, string function)
        {
            EvaluationResult result = _functionEvaluator.EvaluateAtPoint(x, function);
            if (result.HasError)
            {
                return new CreateTangentResult(null, result.Error);
            }
            double y = result.Points[0][1];
            double slope = _functionEvaluator.TakeDerivative(x, function);

            return new CreateTangentResult(new Tangent(x, y, slope), null);
        }

        public GetTangentLineSeriesResult GetTangentLineSeries(Tangent tangent, double xmin, double xmax, double xstep)
        {
            string tangentEquation = tangent.GetTangentEquation();
            var result = _functionEvaluator.Evaluate(tangentEquation, xmin, xmax, xstep);

            if (result.HasError)
            {
                return new GetTangentLineSeriesResult(null, result.Error);
            }

            LineSeries tangentLineSeries = new LineSeries
            {
                Title = $"Tangent at x = {tangent.X}",
                LineStyle = LineStyle.Dash
            };

            foreach (var point in result.Points)
            {
                tangentLineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }

            return new GetTangentLineSeriesResult(tangentLineSeries, null); ;
        }
    }
}