namespace Engine
    module Evaluator =
        let private getStrFromLexerError(err : Tokeniser.LexicalError) =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse(tokens: Tokeniser.Token list) =
            match Parser.parseEval tokens [] with
            | Error parseError -> parseError
            | Ok result ->
                match result with
                | Parser.Int i -> string i
                | Parser.Float f -> string f

        let eval(exp : string) : string =
            match Tokeniser.tokenise exp with
            // To keep C# app independent, we return a string by dealing with conversions in here.
            | Error e -> getStrFromLexerError(e)
            | Ok tokens -> parse(tokens)