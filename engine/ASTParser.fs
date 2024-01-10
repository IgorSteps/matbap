namespace Engine
    // Grammar:
    // ForLoop ::= "for" <varID> "in" "range(<E>,<E>)" ":" <E>
    //           | "for" <varID> "in" "range(<E>,<E>,<float>)" ":" <E>
    // <varA> ::= <varID> = <E>
    // <E>    ::= <T> <Eopt>
    // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
    // <T>    ::= <P> <Topt>
    // <Topt> ::= * <P> <Topt> | / <P> <Topt> | % <P> <Topt> | <empty>
    // <P>    ::= <NR> <Popt>
    // <Popt> ::= ^ <NR> <Popt> | <empty>
    // <NR>   ::= <num> | (E) | <func-call>
    // <num>  ::= <int> | <float> | <varVal>

    // <func-call> ::= <func-name>(<E>)
    // <func-name> ::= sin | cos | tan | log

    // varVal is fetched from symbol table using varID

    module ASTParser =
        open Types
        open Tokeniser

        /// Parses a Number or a Parenthesis expression with/without Unary minus (<NR>).
        let rec parseNumber(tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Int     intNumber   :: remainingTokens -> Ok (Number(NumType.Int(intNumber)), remainingTokens)
            | Float   floatNumber :: remainingTokens -> Ok (Number(NumType.Float(floatNumber)), remainingTokens)
            | Minus               :: remainingTokens -> parseUnaryMinusOperation(remainingTokens)
            | LeftBracket         :: remainingTokens -> parseBracketedExpression(remainingTokens)
            | Sin :: LeftBracket  :: remainingTokens -> parseFunctionCall("sin", remainingTokens)
            | Cos :: LeftBracket  :: remainingTokens -> parseFunctionCall("cos", remainingTokens)
            | Tan :: LeftBracket  :: remainingTokens -> parseFunctionCall("tan", remainingTokens)
            | Log :: LeftBracket  :: remainingTokens -> parseFunctionCall("log", remainingTokens)
            | Exp :: LeftBracket  :: remainingTokens -> parseFunctionCall("exp", remainingTokens)
            | Identifier identifierName :: remainingTokens -> Ok (Variable(identifierName), remainingTokens)
            | _                                      -> Error "Expected number, '(' or '-'."

        and parseUnaryMinusOperation(tokens : Token list) : Result<(Node * Token list), string> =
              match parseNumber(tokens) with
                | Ok (number, remainingTokens)  -> Ok (UnaryMinusOperation("-", number), remainingTokens)
                | Error err -> Error err

        and parseBracketedExpression(tokens : Token list) : Result<(Node * Token list), string> =
             match parseExpression(tokens) with
                | Ok (expr, RightBracket :: remainingTokens)    -> Ok (ParenthesisExpression(expr), remainingTokens)
                | Ok _                                          -> Error "Missing closing bracket"
                | Error err                                     -> Error err
        
        and parseFunctionCall(funcName: string, tokens: Token list) : Result<(Node * Token list), string> =
            match parseExpression(tokens) with
                | Ok (expr, RightBracket :: remainingTokens)    -> Ok (Function(funcName, expr), remainingTokens)
                | Ok _                                          -> Error "Missing closing bracket on function call"
                | Error err                                     -> Error err

        /// Parses the power <P>.
        and parsePower(tokens : Token list) : Result<(Node * Token list), string> =
            match parseNumber(tokens) with
            | Ok result -> parsePowerOperator(result)
            | Error err -> Error err

        /// Parses power operator <Popt> in an expression, handling right-associativity by
        /// first parsing tokens right after power operator(immediateRhsTerm), where if it
        /// finds more power operators on the right hand side, it will recursevily call
        /// itself to ensure right-associativity.
        and parsePowerOperator(lhsTerm, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Power :: remainingTokens ->
                match parseNumber(remainingTokens) with
                | Ok (immediateRhsTerm, tokensAfterImmediateRhs) ->
                    match parsePowerOperator(immediateRhsTerm, tokensAfterImmediateRhs) with
                    | Ok (poweredRhsTerm, remainingTokensAfterAllPowerOperations) -> Ok (BinaryOperation("^", lhsTerm, poweredRhsTerm), remainingTokensAfterAllPowerOperations)
                    | Error err -> Error err
                | Error err -> Error err
            | _ -> Ok (lhsTerm, tokens)
        

        /// Parses the Term <T>, which is a Number or Parenthesis Expression followed by optional arithemtic operations.
        and parseTerm(tokens : Token list) : Result<(Node * Token list), string> =
            match parsePower(tokens) with
            | Ok result  -> parseTermOperators result
            | Error err  -> Error err

        /// Parses the operators in a term <Topt>.
        and parseTermOperators(term, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Multiply :: remainingTokens ->
                match parsePower(remainingTokens) with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators(BinaryOperation("*", term, nextTerm), remainingTokens)
                | Error err                       -> Error err
            | Divide :: remainingTokens ->
                match parsePower(remainingTokens) with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators(BinaryOperation("/", term, nextTerm), remainingTokens)
                | Error err                       -> Error err
            | Modulus :: remainingTokens ->
                match parsePower(remainingTokens) with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators(BinaryOperation("%", term, nextTerm), remainingTokens)
                | Error err                       -> Error err
            | _ -> Ok (term, tokens)

        /// Parses an Expression <E>, which can include + or - operators.
        and parseExpression(tokens : Token list) : Result<(Node * Token list), string> =
            match parseTerm(tokens) with
            | Ok    result  -> parseExpressionOperators(result)
            | Error err     -> Error err

        /// Parses the addition and subtraction operators in an expression <Eopt>.
        and parseExpressionOperators(expr, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Add :: remainingTokens ->
                match parseTerm(remainingTokens) with
                | Ok (t, remainingTokens)   -> parseExpressionOperators(BinaryOperation("+", expr, t), remainingTokens)
                | Error err                 -> Error err
            | Minus :: remainingTokens ->
                match parseTerm(remainingTokens) with
                | Ok (t, remainingTokens)   -> parseExpressionOperators(BinaryOperation("-", expr, t), remainingTokens)
                | Error err                 -> Error err
            | _ -> Ok (expr, tokens)

        and parseForLoop(tokens: Token list) : Result<(Node * Token list), string> =
            match tokens with
            | For :: Identifier varName :: In :: Range :: LeftBracket :: tail ->
                match parseNumber tail with
                | Ok (xmin, remainingTokens) ->
                     match remainingTokens with
                        | Comma :: tail -> 
                            match parseNumber tail with
                            | Ok (xmax, remainingTokens) ->  
                                match remainingTokens with
                                | Comma :: Int step :: RightBracket :: Colon :: tail   -> 
                                    match parseExpression tail with
                                    | Ok(expr, remainingTokens) -> 
                                        Ok(ForLoop(VariableAssignment(varName, xmin), xmax, Number(NumType.Float(float step)), expr), remainingTokens)
                                    | Error err -> Error err
                                | Comma :: Float step :: RightBracket :: Colon :: tail ->
                                    match parseExpression tail with
                                    | Ok(expr, remainingTokens) -> 
                                        Ok(ForLoop(VariableAssignment(varName, xmin), xmax, Number(NumType.Float(step)), expr), remainingTokens)
                                    | Error err -> Error err
                                | RightBracket :: Colon :: tail                        ->
                                    match parseExpression tail with
                                    | Ok(expr, remainingTokens) -> 
                                        Ok(ForLoop(VariableAssignment(varName, xmin), xmax, Number(NumType.Float(1.0)), expr), remainingTokens)
                                    | Error err -> Error err
                                | _ -> Error "Incorrect for-loop declaration, either the step or closing bracket is missing"
                            | Error err -> Error (sprintf "Error parsing xMax value in range: %s" err)
                        | _ -> Error "Incorrect for-loop declaration, must be in form: \"for <varID> in range(<E>,<E>): <E>\""
                | Error err -> Error (sprintf "Error parsing xMin value in range: %s" err)
            | _ -> Error "Incorrect for-loop declaration, must be in form: \"for <varID> in range(<E>,<E>): <E>\""
               
        /// Parses a potential variable assignment, if not it will default to parse an expression
        and parseVariableAssignment(tokens: Token list) : Result<(Node * Token list), string> = 
            match tokens with
            | Identifier varName :: Equals :: remainingTokens ->
                match remainingTokens.IsEmpty with
                | true  -> Error "A variable assignment was attempted without assigning a value"
                | false -> match parseExpression remainingTokens with
                           | Ok (expr, remainingTokens)    -> Ok (VariableAssignment (varName, expr), remainingTokens)
                           | Error err                     -> Error err
            | Equals :: _ -> Error "A variable assignment was attempted without giving a variable name"
            | For :: _    -> parseForLoop tokens
            | _ -> parseExpression tokens

        // Parse tokens.
        let parse(tokens : Token list) : Result<Node, string> =
            match parseVariableAssignment tokens with
            | Ok (ast, remainingTokens)   -> match remainingTokens with
                                             | [] -> Ok(ast)
                                             | Identifier x :: _ ->
                                                Error (sprintf "Unable to parse token at end of expression: %s" x)
                                             | _  -> Error "Unable to parse the end of the expression"
            | Error err     -> Error err
