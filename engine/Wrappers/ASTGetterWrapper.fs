namespace Engine
    
    /// Implements IASTGetter interface.
    type ASTGetterWrapper() =
        interface IASTGetter with
            /// Get AST for the expression.
            member this.GetAST(expressione) =
                ASTGetter.getAST expressione