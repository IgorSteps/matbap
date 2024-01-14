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
        private readonly Engine.IEvaluator _fsharpEvaluator;
        
        public FSharpFunctionEvaluatiorWrapper(Engine.IEvaluator fsharpEvaluator)
        {
            _fsharpEvaluator = fsharpEvaluator;
        }

        public FunctionEvaluationResult Evaluate(string function, double min, double max, double step)
        {
            var result = _fsharpEvaluator.PlotPoints(min, max, step, function);
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
    }
}