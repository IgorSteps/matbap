namespace Engine
    module ASTEvaluator =
        open Types
        // Functions for evaluating
        let rec private topEvalTree (topNode : Node) (symTable : SymbolTable) (plot: bool): Result<string*NumType*SymbolTable*Points, string> =
            match topNode with
            | VariableAssignment (varName, innerNode) -> match plot with
                                                         | true  -> Error "Evaluation error: can't assign variables while in plot mode."
                                                         | false ->
                                                             match setVar varName innerNode symTable with
                                                             | Ok(str, num, symTable) -> Ok(str, num, symTable, [])
                                                             | Error err              -> Error err
            | ForLoop(VariableAssignment(varName, innerNode), xMax, step, expr) ->
                match setVar varName innerNode symTable with
                | Ok(_, _, symTable) -> match evalForLoop varName xMax step expr symTable [] with
                                        | Ok(points) -> Ok("", Int(0), SymbolTable(), points)
                                        | Error err  -> Error err
                | Error err -> Error err
            | _ -> match evalNum topNode symTable with
                   | Ok num -> Ok ("", num, symTable, [])
                   | Error e -> Error e
            
        and private setVar (varName : string) (topNode : Node) (symTable : SymbolTable) : Result<string*NumType*SymbolTable, string> =
            match evalNum topNode symTable with
            | Ok num -> match symTable.ContainsKey varName with
                        | true  -> symTable[varName] <- num
                                   Ok (varName, num, symTable)
                        | false -> symTable.Add(varName, num)
                                   Ok (varName, num, symTable)
            | Error e -> Error e
        
        and private evalForLoop (varName: string) (xMax: Node) (xStep: Node) (expr: Node) (symTable: SymbolTable) (points : Points) =
            let currentX = match symTable[varName] with
                           | Int   x -> float x
                           | Float x -> x
            // xmax is a node to be evaluated
            let max     =  match evalNum xMax symTable with
                           | Ok(Int (x))   -> float x
                           | Ok(Float (x)) -> x
                           | _                    -> 0.0
            // xstep is always a positive float number node
            let step    =  match xStep with
                           | Number (Float x) -> x
                           | _                -> 0.0
            match currentX > max with
            | true  -> Ok(points)
            | false -> match topEvalTree expr symTable false with
                       | Ok(_, num, _, _) -> let y = match num with
                                                     | Int x   -> float x
                                                     | Float x -> x
                                             symTable[varName] <- Float(currentX + step)
                                             evalForLoop varName xMax xStep expr symTable (points@[(currentX, y)])
                       | Error err     -> Error err
                      
        and private evalTree (node : Node) (symTable : SymbolTable) : Result<Node, string> =
            match node with
            | BinaryOperation (op, a, b)      -> match evalBinaryOp (op, a, b) symTable with
                                                 | Ok node -> Ok node
                                                 | Error e -> Error e
            | ParenthesisExpression innerNode -> match evalTree innerNode symTable with
                                                 | Ok node -> Ok node
                                                 | Error e -> Error e
            | UnaryMinusOperation (_, num)    -> match evalNum num symTable with
                                                 | Ok (Int x)   -> Ok (Number (Int -x))
                                                 | Ok (Float x) -> Ok (Number (Float -x))
                                                 | Error e      -> Error e
            | Number n                        -> Ok (Number n)
            | Variable varName                -> match symTable.ContainsKey varName with
                                                 | true  -> Ok (Number symTable[varName])
                                                 | false -> Error "Evaluation error: variable identifier not found."
            | Function(funcName, expr)        -> evalFunction funcName expr symTable
            | _ -> Error "Evaluation error: unexpected node found."
         
        and private evalNum (node : Node) (symTable : SymbolTable) : Result<NumType, string> =
            // Helper function to eval a node and then extract a number
            match evalTree node symTable with
            | Ok node -> match node with
                         | Number num -> Ok num
                         | _ -> Error "Evaluation error: operation result wasn't a number!"
            | Error e -> Error e

        and private evalFunction (funcName: string) (expr: Node) (symTable: SymbolTable) =
            let num = match evalTree expr symTable with
                      | Ok (Number (Int n))   -> Ok (float n)
                      | Ok (Number (Float n)) -> Ok (float n)
                      | Error err             -> Error err
                      | _                     -> Error "Evaluation error: unexpected error at function evaluation"
            match num with
            | Ok n -> 
                match funcName with
                | "sin" -> Ok (Number(Float(sin n)))
                | "cos" -> Ok (Number(Float(cos n)))
                | "tan" -> Ok (Number(Float(tan n)))
                | "log" -> Ok (Number(Float(log n)))
                | "exp" -> Ok (Number(Float(exp n)))
                | _     -> Error "Evaluation error: Not a supported function"
            | Error err                                  -> Error err

        and private evalBinaryOp (op : string, a : Node, b : Node) (symTable : SymbolTable) : Result<Node, string> =
            // This can probably be rewritten in a better way, but the NumType returned still needs to be different. We
            // could potentially test whether the value returned by the operator is either int or float, but that might
            // result in worse performance and the intent is arguably less clear.
            let evalA = evalNum a symTable
            let evalB = evalNum b symTable
            match op with
            | "+" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a+b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a+b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a+float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a+b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "-" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a-b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a-b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a-float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a-b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "*" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a*b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a*b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a*float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a*b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "/" -> match (evalA, evalB) with
                     // Easier to check for zero this way than to write a function to extract number from result
                     | Ok (Int 0),     _ -> Error "Evaluation error: division by 0."
                     | Ok (Float 0.0), _ -> Error "Evaluation error: division by 0."
                     | _, Ok (Int 0)     -> Error "Evaluation error: division by 0."
                     | _, Ok (Float 0.0) -> Error "Evaluation error: division by 0."
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a/b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a/b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a/float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a/b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "^" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Float (a**b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (a**b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a**b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a**b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "%" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a%b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a%b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a%float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a%b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | _   -> Error "Evaluation error: unknown operator."
        
        // Helper functions for handling evaluation
        let private getStrFromLexerError (err : Tokeniser.LexicalError) : string =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str
        let private getFloatFromNum (num : NumType) : float =
            match num with
                | Int x   -> float x
                | Float x -> x
        let private parse (tokens: Tokeniser.Token list) : Result<Node,string> =
            match ASTParser.parse tokens with
            | Ok tokens         -> Ok tokens
            | Error parseError  -> Error parseError
            
        // Evaluation function. Does not return a string for C# - use evalToString for that
        let eval (exp : string) (symTable : SymbolTable) (plot : bool) : Result<(string*NumType)*Points*SymbolTable*Node, string> =
            match Tokeniser.tokenise exp with
            | Ok tokens -> match parse tokens with
                           | Ok tree -> match topEvalTree tree symTable plot with
                                        | Ok (str, num, symTable, points) -> Ok ((str, num), points, symTable, tree)
                                        | Error e            -> Error e
                           | Error e -> Error e
            | Error e -> Error (getStrFromLexerError e)
            
        // Returns evaluation result as a string
        let evalToString (exp : string) (symTable : SymbolTable) : Result<string*Points*SymbolTable*Node, string> =
            match eval exp symTable false with
            | Ok (("", Int num), points, symTable, tree)        -> Ok (string num, points, symTable, tree)
            | Ok (("", Float num), points, symTable, tree)      -> Ok (string num, points, symTable, tree)
            | Ok ((varName, Int num), points, symTable, tree)   -> Ok (varName+" = "+string num, points, symTable, tree)
            | Ok ((varName, Float num), points, symTable, tree) -> Ok (varName+" = "+string num, points, symTable, tree)
            | Error e -> Error e
            
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
            let symTable = SymbolTable()
            symTable.Add("x", Float x)
                
            // Calculation loop. While within range of max
            while (x <= max) do
                // Set x and calculate
                symTable["x"] <- Float x
                let result = eval exp symTable true
                
                match result with
                | Ok ((_, y), _, _, _) -> points.Add([|x; getFloatFromNum y|])
                // If we get an error, needs to be returned instead of the list of plots.
                // gotError holds this and is checked once we leave the loop. X is set to max in order to break the loop
                // NOTE: if it's division by zero, skips and does not plot the point, to plot equations such as 1/x
                | Error e   ->  if e <> "Evaluation error: division by 0." then
                                    gotError <- Some e
                                    x <- max
                // Increment x for loop
                x <- x + trueStep
                
            match gotError with
            | None -> Ok (points.ToArray())
            | Some e -> Error e
            
        // Helper function for root finding
        let private evalToFloat (exp : string) (symTable : SymbolTable) : float =
            match eval exp symTable true with
            | Ok ((_, Int int), _, _, _) -> float int
            | Ok ((_, Float float), _, _, _) -> float
            | Error _ -> infinity // Happens if division by 0
            
        // Recursive function implementing bisection method.
        // Uses accuracy to determine floating point accuracy
        let rec private bisectionRoots (exp : string) (pos : float) (neg : float) (accuracy : int) (depth : int) : Result<float,string> =
            let mid = (pos+neg) / 2.0
            
            // a = result of pos. b = result of neg. c = result of mid.
            let symTable = SymbolTable()
            symTable.Add("x", Float mid)
            let y = evalToFloat exp symTable
                
            // If we're in too deep, or if we found the root (within floating point accuracy)
            // 1000 depth is a bit of an arbitrary choice, but higher values would be much slower. 
            if (depth > 1000 || System.Math.Round(y, accuracy) = 0) then
                Ok mid
            else if (y >= 0) then // If not negative
                bisectionRoots exp mid neg accuracy (depth+1)
            else if (y < 0) then // If negative
                bisectionRoots exp pos mid accuracy (depth+1)
            else // Should never happen if root is in the range somewhere
                Error "Unknown error during root finding"
            
        // Root finding (where y=0) function for expression in form y = <exp>
        // Returns an array containing the estimated x value of each root. 
        let findRoots (min: float) (max: float) (exp: string) : Result<float array, string> =
            // High calculation accuracy is not required so 0.1 is used as a middling value. 
            let points = plotPoints min max 0.1 exp
            let mutable i = 1
            let mutable roots = ResizeArray<float>()
            
            // Floating point accuracy, could be an argument, user-defined?
            // Numbers get rounded to this many decimal places when checked against 0
            let accuracy = 10

            match points with
            | Ok arr -> let mutable gotError = None
                        let mutable last = arr[0]
                        // For each point returned in the array - we want pairs of adjacent points.
                        while (i < arr.Length) do
                            // [0] is x and [1] is y
                            let this = arr[i]
                            // This point COULD be a root, so check if it is, within floating point accuracy range
                            if (System.Math.Round(this[1], 14) = 0) then
                                roots.Add (this[0])
                                i <- i + 1 // Skip next, since they won't form a pair
                            // Otherwise compare to last point. The pairs we want have one positive and one negative.
                            else if (this[1] > 0) then // positive - check if last was negative
                                if (last[1] < 0) then
                                    // Add the root between the two to the array of roots
                                    let root = bisectionRoots exp this[0] last[0] accuracy 0
                                    match root with
                                    | Ok float -> roots.Add float
                                    | Error e -> gotError <- Some e
                            else // must be negative - check if last was not negative
                                if (last[1] >= 0) then // not negative
                                    let root = bisectionRoots exp last[0] this[0] accuracy 0
                                    match root with
                                    | Ok float -> roots.Add float
                                    | Error e -> gotError <- Some e
                            // Update the last element and increment i
                            last <- arr[i]
                            i <- i + 1
                        match gotError with
                        | None   -> Ok (roots.ToArray())
                        | Some e -> Error e
            // Return error if point list is an error
            | Error e -> Error e