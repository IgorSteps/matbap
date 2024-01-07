namespace app
{
    /// <summary>
    /// Interface for providing functionality to find roots for polynomial expression.
    /// Implemented by FindRootsService.
    /// </summary>
    public interface IRootFinder
    {
        public FindRootsServiceResult FindRoots(string expression, double xmin, double xmax);
    }
}
