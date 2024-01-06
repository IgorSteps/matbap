namespace app
{
    public struct FSharpFindRootResult
    {
        public string Answer { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FSharpFindRootResult(string answer, Error err)
        {
            Answer = answer;
            Error = err;
        }
    }
    public class FSharpFindRootsWrapper : IFSharpFindRootsWrapper
    {
        public FSharpFindRootResult FindRoots(string expression, double xmin, double xmax)
        {
            return new FSharpFindRootResult(null, null);
        }

    }
}
