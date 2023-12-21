namespace Engine
    // Grammar:
    // <E>    ::= <T> <Eopt>
    // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
    // <T>    ::= <P> <Topt>
    // <Topt> ::= * <P> <Topt> | / <P> <Topt> | % <P> <Topt> | <empty>
    // <P>    ::= <NR> <Popt>
    // <Popt> ::= ^ <NR> <Popt> | <empty>
    // <NR>   ::= (E) | -(E) | <num> | -<num>
    // <num>  ::= <int> | <float>
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

        /// Parses the power <P>.
        and parsePower(tokens : Token list) : Result<(Node * Token list), string> =
            match parseNumber(tokens) with
            | Ok result -> parsePowerOperator(result)
            | Error err -> Error err

        /// Parses power operator <Popt> in an expression, handling right-associativity.
        and parsePowerOperator(term, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Power :: remainingTokens ->
                match parseNumber(remainingTokens) with
                | Ok (rightTerm, tokensAfterRightTerm) ->
                    match parsePowerOperator(rightTerm, tokensAfterRightTerm) with
                    | Ok (poweredRightTerm, remainingTokensAfterPower) -> Ok (BinaryOperation("^", term, poweredRightTerm), remainingTokensAfterPower)
                    | Error err -> Error err
                | Error err -> Error err
            | _ -> Ok (term, tokens)
        

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

        // Parse tokens.
        let parse(tokens : Token list) : Result<Node, string> =
            match parseExpression(tokens) with
            | Ok (ast, _)   -> Ok ast
            | Error err     -> Error err
