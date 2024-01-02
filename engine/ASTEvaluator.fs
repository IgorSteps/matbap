namespace Engine
    module ASTEvaluator =
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
            // Must return a result to C# app as the return type is different whether it fails or succeeds.
            | Ok tokens -> parse tokens
            | Error e -> Error (getStrFromLexerError e)