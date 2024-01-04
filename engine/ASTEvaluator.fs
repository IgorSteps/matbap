namespace Engine
    module ASTEvaluator =
        open Types
        open System.Collections.Generic
        type SymbolTable = Dictionary<string, NumType>
        // Functions for evaluating
        let rec private topEvalTree (topNode : Node) (symTable : SymbolTable) : Result<string*NumType*SymbolTable, string> =
            match topNode with
            | VariableAssignment (varName, innerNode) -> setVar varName innerNode symTable
            | _ -> match evalNum topNode symTable with
                   | Ok num -> Ok ("", num, symTable)
                   | Error e -> Error e
            
        and private setVar (varName : string) (topNode : Node) (symTable : SymbolTable) : Result<string*NumType*SymbolTable, string> =
            match evalNum topNode symTable with
            | Ok num -> match symTable.ContainsKey varName with
                        | true  -> symTable[varName] <- num
                                   Ok (varName, num, symTable)
                        | false -> symTable.Add(varName, num)
                                   Ok (varName, num, symTable)
            | Error e -> Error e
            
        and private evalTree (node : Node) (symTable : SymbolTable) : Result<Node, string> =
            match node with
            | BinaryOperation (op, a, b)      -> match evalBinaryOp (op, a, b) symTable with
                                                 | Ok node -> Ok node
                                                 | Error e -> Error e
            | ParenthesisExpression innerNode -> match evalTree innerNode symTable with
                                                 | Ok node -> Ok node
                                                 | Error e -> Error e
            // Not going to bother checking operator for unary minus operation, assume it's there for future extension?
            | UnaryMinusOperation (_, num)    -> match evalNum num symTable with
                                                 | Ok (Int x)   -> Ok (Number (Int -x))
                                                 | Ok (Float x) -> Ok (Number (Float -x))
                                                 | Error e      -> Error e
            | Number n                        -> Ok (Number n)
            | Variable varName                -> match symTable.ContainsKey varName with
                                                 | true  -> Ok (Number symTable[varName])
                                                 | false -> Error "Variable identifier not found."
            | _ -> Error "Found an unexpected node in the syntax tree."
         
        and private evalNum (node : Node) (symTable : SymbolTable) : Result<NumType, string> =
            // Helper function to eval a node and then extract a number
            match evalTree node symTable with
            | Ok node -> match node with
                         | Number num -> Ok num
                         | _ -> Error "Evaluation result wasn't a number!"
            | Error e -> Error e
         
        and private evalBinaryOp (op : string, a : Node, b : Node) (symTable : SymbolTable) : Result<Node, string> =
            // This can probably be rewritten in a better way, but the NumType returned still needs to be different. We
            // could potentially test whether the value returned by the operator is either int or float, but that might
            // result in worse performance and the intent is arguably less clear. Any comments on this are appreciated.
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
                     // Still needs a check for division by zero!
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
            | _   -> Error "Unknown operator in AST evaluation."
        
        // Helper functions for handling evaluation
        let private getStrFromLexerError (err : Tokeniser.LexicalError) : string =
            match err with
                | Tokeniser.InvalidFloat str -> str
                | Tokeniser.InvalidToken str -> str
        let private parse (tokens: Tokeniser.Token list) : Result<Node,string> =
            match ASTParser.parse tokens with
            | Error parseError  -> Error parseError
            | Ok tokens         -> Ok tokens
            
        // Evaluation function
        let eval (exp : string) (symTable : SymbolTable) : Result<(string*NumType)*SymbolTable*Node, string> =
            match Tokeniser.tokenise exp with
            | Ok tokens -> match parse tokens with
                           | Ok tree -> match topEvalTree tree symTable with
                                        | Ok (str, num, symTable) -> Ok ((str, num), symTable, tree)
                                        | Error e            -> Error e
                           | Error e -> Error e
            | Error e -> Error (getStrFromLexerError e)