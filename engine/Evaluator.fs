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
                | ((varName, Parser.Int i), symTable)    -> Ok((varName + " = " + string i), symTable)
                | ((varName, Parser.Float f), symTable)  -> Ok((varName + " = " + string f), symTable)

        let eval(exp : string) (symTable: Dictionary<string, Parser.NumType>): Result<string * Dictionary<string, Parser.NumType>,string>  =
            match Tokeniser.tokenise exp with
            // Must return a result to C# app as the return type is different whether it fails or succeeds.
            | Error e   -> Error (getStrFromLexerError(e))
            | Ok tokens -> match parse tokens symTable with
                           | Ok(result, symTable) -> Ok(result, symTable)
                           | Error e              -> Error e
                           
                           
        // Private functions for plotting to avoid casting to string and adding variable names
        let private plotParse(tokens: Tokeniser.Token list) (symTable: Dictionary<string, Parser.NumType>) =
            match Parser.parseEval tokens symTable with
            | Error parseError                    -> Error parseError
            | Ok result -> // Should search for assignment token here. Need to add function to do that
                if (List.contains Tokeniser.Equals tokens)  then
                    Error "Can't use assignment within"
                else match result with
                     | ((_, Parser.Int i), symTable)   -> Ok(float i, symTable)
                     | ((_, Parser.Float f), symTable) -> Ok(f, symTable)
                
        let plotEval(exp : string) (symTable: Dictionary<string, Parser.NumType>): Result<float * Dictionary<string, Parser.NumType>,string>  =
            match Tokeniser.tokenise exp with
            | Error e   -> Error (getStrFromLexerError(e))
            | Ok tokens -> match plotParse tokens symTable with
                           | Ok(result, symTable) -> Ok(result, symTable)
                           | Error e              -> Error e
                           
        // Returns a list of points to plot based on a given minimum, maximum, and step. Step is forced to be positive
        // For now hooks into the evaluation engine so it's possible to do something such as "y=x=x+1" and it will crash,
        // so in future it should check for cases such as this while parsing.
        // Expression input in the form: y = <exp>
        let plotPoints (min: float) (max: float) (step: float) (exp : string) (symTable: Dictionary<string, Parser.NumType>) =
            let mutable points = ResizeArray<float list>()
            let trueStep = abs(step)
            let mutable x = float min
            let mutable error = None
            match symTable.ContainsKey "x" with
                | true  -> symTable["x"] <- Parser.Float x
                | false -> symTable.Add("x", Parser.Float x)
            while (x <= max) do
                symTable["x"] <- Parser.Float x
                let result = plotEval exp symTable
                match result with
                | Ok (y, _) -> points.Add([x; y])
                | Error e        -> error <- Some e
                                    x <- max
                x <- x + trueStep
            match error with
            | None -> Ok (points.ToArray())
            | Some e -> Error e