namespace Engine
    module ASTEvaluator =
        open Types
        // Functions for evaluating
        let rec private evalTree (topNode : Node) : Result<Node, string> =
            match topNode with
            | BinaryOperation (op,a,b) -> match evalBinaryOperation (op, a, b) with
                                          | Ok node -> Ok node
                                          | Error e -> Error e
            | ParenthesisExpression _  -> Ok (Number (Float 0))
            | UnaryMinusOperation  _   -> Ok (Number (Float 0))
            | VariableAssignment  _    -> Ok (Number (Float 0))
            | Number n                 -> Ok (Number n)
         
        and private extractEval (node : Node) : Result<NumType, string> =
            match evalTree node with
            | Ok node -> match node with
                         | Number num -> Ok num
                         | _ -> Error "Evaluation result wasn't a number!"
            | Error e -> Error e
            // These errors won't be seen normally since the function that calls this doesn't check the string
         
        and private evalBinaryOperation (op : string, a : Node, b : Node) : Result<Node, string> =
            // This can probably be rewritten in a better way, but the NumType returned still needs to be different. We
            // could potentially test whether the value returned by the operator is either int or float, but that might
            // result in worse performance and the intent is arguably less clear. Any comments on this are appreciated.
            let evalA = extractEval a
            let evalB = extractEval b
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
                     // Still needs a check for division by zero!
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a/b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a/b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a/float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a/b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | "%" -> match (evalA, evalB) with
                     | Ok (Int a),   Ok (Int b)   -> Ok (Number (Int   (a%b)))
                     | Ok (Int a),   Ok (Float b) -> Ok (Number (Float (float a%b)))
                     | Ok (Float a), Ok (Int b)   -> Ok (Number (Float (a%float b)))
                     | Ok (Float a), Ok (Float b) -> Ok (Number (Float (a%b)))
                     | Error e,      _            -> Error e
                     | _,            Error e      -> Error e
            | _   -> Error "Unknown operator in AST evaluation."
        
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
                           | Ok tree -> evalTree tree
                           | Error e -> Error e
            | Error e -> Error (getStrFromLexerError e)