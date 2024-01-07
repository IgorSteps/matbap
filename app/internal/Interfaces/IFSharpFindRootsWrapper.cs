
namespace app
{
    public interface IFSharpFindRootsWrapper
    {
        public FSharpFindRootResult FindRoots(string expression, double xmin, double xmax);
    }
}
