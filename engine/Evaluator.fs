namespace Engine
    module Evaluator =
        // Helper function to extract string from tokeniser LexicalError
        let private getStr (v : Tokeniser.LexicalError) =
            match v with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str
        // Evaluator function
        let eval (exp : string) : Result<float, string> =
            match Tokeniser.tokenise exp with
            // If tokeniser returns an error need to get string from it
            | Error e -> Error (getStr e)
            | Ok x ->   match Parser.parseEval x with
                        | Ok x -> Ok x
                        | Error e -> Error e