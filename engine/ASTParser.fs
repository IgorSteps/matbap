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
            | Tokeniser.Int     x :: tail -> Ok (Number (NumType.Int x), tail)
            | Tokeniser.Float   x :: tail -> Ok (Number (NumType.Float x), tail)
            | LeftBracket :: tail ->
                match parseExpression tail with
                | Ok (expr, RightBracket :: remainingTokens)    -> Ok (ParenthesisExpression expr, remainingTokens)
                | Ok (_, _)                                     -> Error "Missing closing bracket"
                | Error err                                     -> Error err
            | _ -> Error "Expected number or '('"

        /// Parses the Term <T>, which is a Number or Parenthesis Expression followed by optional * or / operations
        and parseTerm (tokens : Token list) : Result<(Node * Token list), string> =
            match parseNumber tokens with
            | Ok result -> parseTermOperators result
            | Error err -> Error err

        /// Parses the multiplication and division operators in a term <Topt>.
        and parseTermOperators (expr, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Multiply :: tail ->
                match parseNumber tail with
                | Ok (nr, remainingTokens)  -> parseTermOperators (BinaryOperation ("*", expr, nr), remainingTokens)
                | Error err                 -> Error err
            | Divide :: tail ->
                match parseNumber tail with
                | Ok (nr, remainingTokens)  -> parseTermOperators (BinaryOperation ("/", expr, nr), remainingTokens)
                | Error err                 -> Error err
            | _ -> Ok (expr, tokens)

        /// Parses an Expression <E>, which can include + or - operators.
        and parseExpression (tokens : Token list) : Result<(Node * Token list), string> =
            match parseTerm tokens with
            | Ok result -> parseExpressionOperators result
            | Error err -> Error err

        /// Parses the addition and subtraction operators in an expression <Eopt>.
        and parseExpressionOperators (expr, tokens : Token list) : Result<(Node * Token list), string> =
            match tokens with
            | Add :: tail ->
                match parseTerm tail with
                | Ok (t, remainingTokens)   -> parseExpressionOperators (BinaryOperation ("+", expr, t), remainingTokens)
                | Error err                 -> Error err
            | Minus :: tail ->
                match parseTerm tail with
                | Ok (t, remainingTokens)   -> parseExpressionOperators (BinaryOperation ("-", expr, t), remainingTokens)
                | Error err                 -> Error err
            | _ -> Ok (expr, tokens)

        // Parse tokens.
        let parse (tokens : Token list) : Result<Node, string> =
            match parseExpression tokens with
            | Ok (ast, _) -> Ok ast
            | Error err -> Error err
