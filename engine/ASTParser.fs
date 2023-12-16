namespace Engine
    // Grammar:
    // <E>    ::= <T> <Eopt>
    // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
    // <T>    ::= <NR> <Topt>
    // <Topt> ::= * <NR> <Topt> | / <NR> <Topt> | <empty>
    // <NR>   ::= <int> | <float> | (E)
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

        /// Parses the Term <T>, which is a Number or Parenthesis Expression followed by optional * or / operations
        and parseTerm(tokens : Token list) : Result<(Node * Token list), string> =
            match parseNumber tokens with
            | Ok result  -> parseTermOperators result
            | Error err  -> Error err

        /// Parses the multiplication and division operators in a term <Topt>.
        and parseTermOperators(term, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Multiply :: remainingTokens ->
                match parseNumber remainingTokens with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators (BinaryOperation ("*", term, nextTerm), remainingTokens)
                | Error err                 -> Error err
            | Divide :: remainingTokens ->
                match parseNumber remainingTokens with
                | Ok (nextTerm, remainingTokens)  -> parseTermOperators (BinaryOperation ("/", term, nextTerm), remainingTokens)
                | Error err                 -> Error err
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
