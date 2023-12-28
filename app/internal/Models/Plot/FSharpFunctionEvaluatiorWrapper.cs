namespace app
{
    public struct FunctionEvaluationResult
    {
        /// <summary>
        /// Returns points in [x,y] format.
        /// </summary>
        public double[][] Points { get; set; }
        public Error Error { get; set; }
        public readonly bool HasError => Error != null;

        public FunctionEvaluationResult(double[][] p, Error err)
        {
            Points = p;
            Error = err;
        }
    }

    public class FSharpFunctionEvaluatiorWrapper: IFSharpFunctionEvaluatorWrapper
    {
        public FunctionEvaluationResult Evaluate(string function, double min, double max, double step)
        {
            var result = Engine.Evaluator.plotPoints(min, max, step, function);
            if (result.IsError)
            {
                return new FunctionEvaluationResult(null, new Error(result.ErrorValue));
            }
           
            return new FunctionEvaluationResult(result.ResultValue, null);
        }

        public FunctionEvaluationResult EvaluateAtPoint(double x, string function)
        {
            var result = Evaluate(function, x, x, x);
            if (result.HasError)
            {
                return new FunctionEvaluationResult(null, result.Error);
            }

            return new FunctionEvaluationResult(result.Points, null);
        }

        // @TODO: Switch with implementation in F#.
        public double TakeDerivative(double x, string function) 
        {
            double h = 1e-5; // A small number for the central difference calculation
            // Error checking is missing.
            double yPlus = Evaluate(function, x + h, x + h, 1).Points[0][1];
            double yMinus = Evaluate(function, x - h, x - h, 1).Points[0][1];

            return (yPlus - yMinus) / (2 * h);
        }
    }
}