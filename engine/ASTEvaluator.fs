namespace Engine
    module ASTEvaluator =
        // Functions for evaluating
        let rec private evalTree (topNode : Types.Node) : Types.Node =
            match topNode with
            | Types.BinaryOperation (op,a,b) -> evalTree a
            | Types.ParenthesisExpression _  -> Types.Number (Types.Float 0)
            | Types.UnaryMinusOperation  _   -> Types.Number (Types.Float 0)
            | Types.VariableAssignment  _    -> Types.Number (Types.Float 0)
            | Types.Number n                 -> Types.Number n
            // The above could also be written as _ -> _, but is more clear this way
        
        // Functions for handling evaluation
        let private getStrFromLexerError(err : Tokeniser.LexicalError) =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse(tokens: Tokeniser.Token list) =
            match ASTParser.parse tokens with
            | Error parseError  -> Error parseError
            | Ok result         -> Ok result

        let eval(exp : string) =
            match Tokeniser.tokenise exp with
            | Ok tokens -> match parse tokens with
                           | Ok tree -> Ok (evalTree tree)
                           | Error e -> Error e
            | Error e -> Error (getStrFromLexerError e)