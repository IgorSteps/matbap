namespace Engine
    module Evaluator =
        // Helper function to extract string from tokeniser LexicalError
        let private getStrFromLexererror(err : Tokeniser.LexicalError) =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse(tokens: Tokeniser.Token list) =
            match Parser.parseEval tokens with
            | Error parseError -> parseError
            | Ok result ->
                match result with
                | Parser.Int i -> string i
                | Parser.Float f -> string f

        // Evaluator function
        let eval(exp : string) : string =
            match Tokeniser.tokenise exp with
            // If tokeniser returns an error need to get string from it
            | Error e -> getStrFromLexererror(e)
            | Ok tokens -> parse(tokens)
                
                

