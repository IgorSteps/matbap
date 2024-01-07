namespace Engine
    /// Interface for finding roots of polynomials.
    type IRootFinder =
        abstract member FindRoots: float * float * string -> Result<float array, string>
