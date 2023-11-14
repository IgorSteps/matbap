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
                // Parsing a line returns a varName if it was an assigment and "" if it was an expression. 
                | (("", Parser.Int i), symTable)   -> Ok(string i, symTable)
                | (("", Parser.Float f), symTable) -> Ok(string f, symTable)
                | ((varName, Parser.Int i), symTable)    -> Ok(string i, symTable)
                | ((varName, Parser.Float f), symTable)  -> Ok(string f, symTable)

        let eval(exp : string) (symTable: Dictionary<string, Parser.NumType>): Result<string * Dictionary<string, Parser.NumType>,string>  =
            match Tokeniser.tokenise exp with
            // Must return a result to C# app as the return type is different whether it fails or succeeds.
            | Error e   -> Error (getStrFromLexerError(e))
            | Ok tokens -> match parse tokens symTable with
                           | Ok(result, symTable) -> Ok(result, symTable)
                           | Error e              -> Error e