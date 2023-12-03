namespace Engine
    module Evaluator =
        open System.Collections.Generic

        let private getStrFromLexerError(err : Tokeniser.LexicalError) =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str

        let private parse(tokens: Tokeniser.Token list) (symTable: Dictionary<string, Parser.NumType>) =
            match Parser.parseEval tokens symTable with
            | Error parseError                    -> Error parseError
            | Ok result ->
                match result with
                // Parsing a line returns a varName if it was an assigment and "" if it was an expression. 
                | ("", Parser.Int i), symTable          -> Ok(string i, symTable)
                | ("", Parser.Float f), symTable        -> Ok(string f, symTable)
                | (varName, Parser.Int i), symTable     -> Ok((varName + " = " + string i), symTable)
                | (varName, Parser.Float f), symTable   -> Ok((varName + " = " + string f), symTable)

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
                    Error "Can't use assignment in plotting mode"
                else match result with
                     | (_, Parser.Int i), symTable      -> Ok(float i, symTable)
                     | (_, Parser.Float f), symTable    -> Ok(f, symTable)
                
        let private plotEval(exp : string) (symTable: Dictionary<string, Parser.NumType>): Result<float * Dictionary<string, Parser.NumType>,string>  =
            match Tokeniser.tokenise exp with
            | Error e   -> Error (getStrFromLexerError(e))
            | Ok tokens -> match plotParse tokens symTable with // HACK: change when AP-76 is merged
                           | Ok(result, symTable) -> Ok(result, symTable)
                           | Error e              -> Error e
                           
        // Returns a list of points to plot based on a given minimum, maximum, and step. Step is forced to be positive,
        // and min/max are treated as "start point" and "end point"
        // Expression input in the form: y = <exp>
        let plotPoints (min: float) (max: float) (step: float) (exp : string) : Result<float array array, string> =
            // Definitions
            let mutable points = ResizeArray<float array>()
            let mutable x = float min
            let mutable gotError = None
            let trueStep = abs(step)
            
            // Create symbol table containing X
            let symTable = Dictionary<string, Parser.NumType>()
            symTable.Add("x", Parser.Float x)
                
            // Calculation loop. While within range of max
            while (x <= max) do
                // Set x and calculate
                symTable["x"] <- Parser.Float x
                let result = plotEval exp symTable
                
                match result with
                | Ok (y, _) -> points.Add([|x; y|])
                // If we get an error, needs to be returned instead of the list of plots.
                // gotError holds this and is checked once we leave the loop. X is set to max in order to break the loop
                // NOTE: if it's division by zero, skips and does not plot the point, to plot equations such as 1/x
                | Error e   ->  if not (e = "Error while parsing: division by 0") then
                                    gotError <- Some e
                                    x <- max
                // Increment x for loop
                x <- x + trueStep
                
            match gotError with
            | None -> Ok (points.ToArray())
            | Some e -> Error e