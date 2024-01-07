using System;

namespace app
{
    public struct FSharpFindRootResult
    {
        public string Roots { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpFindRootResult(string answer, Error err)
        {
            Roots = answer;
            Error = err;
        }
    }
    public class FSharpFindRootsWrapper : IFSharpFindRootsWrapper
    {
        private readonly Engine.IRootFinder _rootFinder;

        public FSharpFindRootsWrapper(Engine.IRootFinder rootFinder)
        {
            _rootFinder = rootFinder;
        }

        public FSharpFindRootResult FindRoots(string expression, double xmin, double xmax)
        {
            var result = _rootFinder.findRoots(xmin, xmax, expression);
            if (result.IsError)
            {
                return new FSharpFindRootResult(null, new Error(result.ErrorValue));
            }

            return new FSharpFindRootResult(ConvertArrayToString(result.ResultValue), null);
        }

        private string ConvertArrayToString(double[] doubleArray)
        {
            return string.Join(", ", doubleArray);
        }
    }
}
