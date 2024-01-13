using FSharpVertices = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;

namespace app
{
    public struct FSharpIntegratorResult 
    {
        public double Area { get; private set; }
        public double[][] Vertices { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpIntegratorResult(double area, double[][] vertices, Error error)
        {
            Area = area;
            Vertices = vertices;
            Error = error;
        }

    }

    public class FSharpIntegratorWrapper : IFSharpIntegratorWrapper
    {
        private readonly Engine.IIntegrator _integrator;

        public FSharpIntegratorWrapper(Engine.IIntegrator integrator)
        {
            _integrator = integrator;
        }

        public FSharpIntegratorResult CalculateAreaUnderCurve(string function, double min, double max, double step)
        {
            var integrationResult = _integrator.Integrate(function, min, max, step);
            if(integrationResult.IsError)
            {
                return new FSharpIntegratorResult(0, null, new Error(integrationResult.ErrorValue));
            }

            return new FSharpIntegratorResult(integrationResult.ResultValue.Item1, ConvertFSharpVertices(integrationResult.ResultValue.Item2), null);
        }

        private double[][] ConvertFSharpVertices(FSharpVertices vertices)
        {
            double[][] convertedVertices = new double[vertices.Length][];
            int index = 0;
            foreach (var tuple in vertices)
            {
                convertedVertices[index] = new double[] { tuple.Item1, tuple.Item2 };
                index++;
            }

            return convertedVertices;
        }
    }
}
