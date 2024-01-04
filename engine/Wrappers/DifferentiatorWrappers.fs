namespace Engine
    
    /// Implements IDifferentiator interface.
    type DifferentiatorWrapper() =
        interface IDifferentiator with
            /// Differentiate polynomial function using its AST.
            member this.Differentiate(node, var) =
                Differentiation.differentiate node var

