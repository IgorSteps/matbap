namespace Engine
    module Evaluator =
        open System.Collections.Generic

        let private getStrFromLexerError(err : Tokeniser.LexicalError) =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse(tokens: Tokeniser.Token list list) (symTable: Dictionary<string, Parser.NumType>) =
            match Parser.parseEval tokens symTable with
            | Error parseError                    -> Error parseError
            | Ok result ->
                match result with
                | (("", Parser.Int i), symTable)   -> Ok(string i, symTable)
                | (("", Parser.Float f), symTable) -> Ok(string f, symTable)
                | ((s, Parser.Int i), symTable)    -> Ok(string i, symTable)
                | ((s, Parser.Float f), symTable)  -> Ok(string f, symTable)

        let eval(exp : string) : Result<string * Dictionary<string, Parser.NumType>,string>  =
            let symTable = new Dictionary<string, Parser.NumType>()
            match Tokeniser.tokenise exp with
            // To keep C# app independent, we return a string by dealing with conversions in here.
            | Error e   -> Error (getStrFromLexerError(e))
            | Ok tokens -> match parse tokens symTable with
                           | Ok(result, symTable) -> Ok(result, symTable)
                           | Error e              -> Error e