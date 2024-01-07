namespace Engine
    /// Interface for finding roots of polynomials.
    type IRootFinder =
        abstract member findRoots: float * float * string -> Result<float array, string>
