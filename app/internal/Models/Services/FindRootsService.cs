namespace app
{
    public struct FindRootsServiceResult
    {
        public string Roots { get; private set; }
        public Error Error { get; private set; }
        public readonly bool HasError => Error != null;
        public FindRootsServiceResult(string roots, Error err)
        {
            Roots = roots;
            Error = err;
        }
    }
    /// <summary>
    /// FindRoots Service provides functionality to validate and find roots for a polynomial.
    /// </summary>
    public class FindRootsService : IRootFinder
    {
        private readonly IFSharpFindRootsWrapper _findRootsWrapper;
        private readonly IValidator _validator;
        public FindRootsService(IFSharpFindRootsWrapper fSharpFindRootsWrapper, IValidator validator)
        {
            _findRootsWrapper = fSharpFindRootsWrapper;
            _validator = validator;
        }

        public FindRootsServiceResult FindRoots(string expression, double xmin, double xmax)
        {
            Error err = _validator.ValidateFindRootsInput(expression, xmin, xmax);
            if (err != null)
            {
                return new FindRootsServiceResult(null, err);
            }

            var findRootsResult = _findRootsWrapper.FindRoots(expression, xmin, xmax);
            if (findRootsResult.HasError)
            {
                return new FindRootsServiceResult(null, findRootsResult.Error);
            }

            return new FindRootsServiceResult(findRootsResult.Roots, null); ;
        }
    }
}
