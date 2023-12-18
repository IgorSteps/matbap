namespace Engine
    // Grammar:
    // <E>    ::= <T> <Eopt>
    // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
    // <T>    ::= <P> <Topt>
    // <Topt> ::= * <P> <Topt> | / <P> <Topt>
    //          | % <P> <Topt> | <empty>
    // <P>    ::= <NR> <Popt>
    // <Popt> ::= ^ <NR> <Popt> | <empty>
    // <NR>   ::= <num> | (E)
    // <num>  ::= <int> | <float>
    module ASTParser =
        open Types
        open Tokeniser

        /// Parses a Number or a Parenthesis expression (<NR>).
        let rec parseNumber(tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Tokeniser.Int     intNumber   :: remainingTokens -> Ok (Number (NumType.Int    intNumber), remainingTokens)
            | Tokeniser.Float   floatNumber :: remainingTokens -> Ok (Number (NumType.Float  floatNumber), remainingTokens)
            | LeftBracket         :: remainingTokens ->
                match parseExpression remainingTokens with
                | Ok (expr, RightBracket :: remainingTokens)    -> Ok (ParenthesisExpression expr, remainingTokens)
                | Ok (_)                                        -> Error "Missing closing bracket"
                | Error err                                     -> Error err
            | _                           -> Error "Expected number or '('"

        /// Parses the power <P>.
        and parsePower(tokens : Token list) : Result<(Node * Token list), string> =
            match parseNumber tokens with
            | Ok result -> parsePowerOperator result
            | Error err -> Error err

        /// Parses power operations in an expression, handling right-associativity.
        /// This function takes a term and a list of tokens and checks for a power operator.
        /// If a power operator is found, it recursively parses the right-hand side (RHS) term of the operator,
        /// ensuring that power operations are evaluated from right to left (right-associative).
        /// For example, in an expression "a ^ b ^ c", it interprets as "a ^ (b ^ c)".
        /// 
        /// Parameters:
        ///   term: The left-hand side (LHS) term of the power operation.
        ///   tokens: The remaining list of tokens to be parsed.
        /// 
        /// The function first parses the immediate RHS of the power operator.
        /// If further power operators are found on the RHS, it recursively calls itself,
        /// ensuring right-associative evaluation, before combining the LHS term with the powered RHS term.
        /// 
        /// Returns: A Result containing either the parsed power expression (as a BinaryOperation node) 
        /// and the remaining unparsed tokens, or an error message if parsing fails.
        and parsePowerOperator(term, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Power :: remainingTokens ->
                match parseNumber remainingTokens with
                | Ok (rightTerm, tokensAfterRightTerm) ->
                    match parsePowerOperator(rightTerm, tokensAfterRightTerm) with
                    | Ok (poweredRightTerm, remainingTokensAfterPower) -> 
                        Ok (BinaryOperation ("^", term, poweredRightTerm), remainingTokensAfterPower)
                    | Error err -> Error err
                | Error err -> Error err
            | _ -> Ok (term, tokens)
        

        /// Parses the Term <T>, which is a Number or Parenthesis Expression followed by optional arithemtic operations.
        and parseTerm(tokens : Token list) : Result<(Node * Token list), string> =
            match parsePower tokens with
            | Ok result  -> parseTermOperators result
            | Error err  -> Error err

        /// Parses the operators in a term <Topt>.
        and parseTermOperators(term, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Multiply :: remainingTokens ->
                match parsePower remainingTokens with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators (BinaryOperation ("*", term, nextTerm), remainingTokens)
                | Error err                 -> Error err
            | Divide :: remainingTokens ->
                match parsePower remainingTokens with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators (BinaryOperation ("/", term, nextTerm), remainingTokens)
                | Error err                 -> Error err
            | Modulus :: remainingTokens ->
                match parsePower remainingTokens with
                | Ok (nextTerm, remainingTokens) -> parseTermOperators (BinaryOperation ("%", term, nextTerm), remainingTokens)
                | Error err -> Error err
            | _ -> Ok (term, tokens)

        /// Parses an Expression <E>, which can include + or - operators.
        and parseExpression(tokens : Token list) : Result<(Node * Token list), string> =
            match parseTerm tokens with
            | Ok    result  -> parseExpressionOperators result
            | Error err     -> Error err

        /// Parses the addition and subtraction operators in an expression <Eopt>.
        and parseExpressionOperators(expr, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Add :: remainingTokens ->
                match parseTerm remainingTokens with
                | Ok (t, remainingTokens)   -> parseExpressionOperators (BinaryOperation ("+", expr, t), remainingTokens)
                | Error err                 -> Error err
            | Minus :: remainingTokens ->
                match parseTerm remainingTokens with
                | Ok (t, remainingTokens)   -> parseExpressionOperators (BinaryOperation ("-", expr, t), remainingTokens)
                | Error err                 -> Error err
            | _ -> Ok (expr, tokens)

        // Parse tokens.
        let parse(tokens : Token list) : Result<Node, string> =
            match parseExpression tokens with
            | Ok (ast, _)   -> Ok ast
            | Error err     -> Error err
