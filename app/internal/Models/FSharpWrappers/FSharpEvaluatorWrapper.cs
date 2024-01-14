using FSharpASTNode = Engine.Types.Node;
using FSharpPoints = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;

namespace app
{
    public struct FSharpEvaluationResult
    {
        public string Answer { get; private set; }
        public FSharpASTNode FSharpAST { get; private set; }
        public double[][] Points { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpEvaluationResult(string answer, double[][] points , FSharpASTNode ast, Error err)
        {
            Answer = answer;
            Points = points;
            FSharpAST = ast;
            Error = err;
        }
    }

    /// <summary>
    /// Handles evaluation of the expression using F# engine.
    /// </summary>
    public class FSharpEvaluatorWrapper : IFSharpEvaluatorWrapper
    {
        private readonly Engine.IEvaluator _fsharpEvaluator;

        public FSharpEvaluatorWrapper(Engine.IEvaluator fsharpEvaluator)
        {
            _fsharpEvaluator = fsharpEvaluator;
        }

        public FSharpEvaluationResult Evaluate(string expression, SymbolTable symbolTable )
        {
            var result = _fsharpEvaluator.Eval(expression, symbolTable.RawSymbolTable);
            if (result.IsError)
            {
                return new FSharpEvaluationResult(null, null, null, new Error(result.ErrorValue));
            }
           
            return new FSharpEvaluationResult(result.ResultValue.Item1, ConvertFSharpPoints(result.ResultValue.Item2), result.ResultValue.Item4, null);
        }

        private double[][] ConvertFSharpPoints(FSharpPoints points)
        {
            double[][] convertedPoints = new double[points.Length][];
            int index = 0;
            foreach (var tuple in points)
            {
                convertedPoints[index] = new double[] { tuple.Item1, tuple.Item2 };
                index++;
            }

            return convertedPoints;
        }
    }
}
