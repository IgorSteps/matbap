namespace Engine
    module Tokeniser =

        type LexicalError =
            | InvalidFloat
            | InvalidToken

        type TokenType =
            | Variable of string
            | Int of int
            | Float of float
            | Add
            | Minus
            | Multiply
            | Divide
            | LeftBracket
            | RightBracket
            | Modulus
            | Equals
            | Sin
            | Cos
            | Tan

        let keywords = Map.empty.Add("sin", Sin).Add("cos", Cos).Add("cos", Cos)

        // Helpers.
        let strtochar (str: string) : char list = [for c in str do yield c]
        let isDigit(c: char)         = System.Char.IsDigit c
        let isLetter(c: char)        = System.Char.IsLetter c
        let noStartingSpace(c: char) = if c <> ' ' then true else false
        let isKeyword (str: string)  = if keywords.ContainsKey str then true else false

        //returns an error if the float is formatted wrong
        let isFloat(chars: char list) : Result<unit, LexicalError> =

            let firstCharValid = if noStartingSpace chars.Head && not (isLetter chars.Head) then
                                    Ok ()
                                 else
                                    Error InvalidFloat

            let rec postFirstChar chars =
                match chars with
                    | c::tail when isDigit c -> postFirstChar tail
                    | '.'::_                 -> Error InvalidFloat
                    | _                      -> Ok ()

            match firstCharValid with
            | Ok ()     -> postFirstChar chars
            | Error err -> Error err

        //returns a Result as it may error if the number is an incorrectly formatted string
        let rec formFractionalPart(chars: char list, accStr: string) =
            match chars with
            | c::tail when isDigit c -> formFractionalPart(tail, accStr + string c)
            | _                      -> Ok (chars, accStr, "float")
        
        //returns a Result as it may error if the number is an incorrectly formatted string
        let rec formInt(chars: char list, accStr: string) = 
            match chars with
            | c :: tail when isDigit c    -> formInt(tail, accStr + string c)
            | '.'::tail                   -> match isFloat tail with
                                             | Ok ()     -> formFractionalPart(tail, accStr + string '.')
                                             | Error err -> Error err
            | _                           -> Ok (chars, accStr, "int")

        let rec formString(chars: char list, accStr: string) =
            match chars with
            | c::tail when isLetter c -> formString(tail, accStr + string c)
            | c::tail when isDigit c  -> formString(tail, accStr + string c)
            | _                       -> (chars, accStr)
        
        //tail recursive because it has to be wrapped in a result
        let tokenize (str : string): Result<TokenType list, LexicalError> =
            let rec matchTokens chars acc =
                match chars with 
                | []          -> Ok(List.rev acc)
                | '+' :: tail -> matchTokens tail (Add::acc)
                | '-' :: tail -> matchTokens tail (Minus::acc)
                | '*' :: tail -> matchTokens tail (Multiply::acc)
                | '/' :: tail -> matchTokens tail (Divide::acc)
                | '(' :: tail -> matchTokens tail (LeftBracket::acc)
                | ')' :: tail -> matchTokens tail (RightBracket::acc)
                | '%' :: tail -> matchTokens tail (Modulus::acc)
                | '=' :: tail -> matchTokens tail (Equals::acc)
                | ' ' :: tail -> matchTokens tail acc
                | head :: tail when isDigit head ->
                    match formInt(tail, string head) with
                    | Ok (chars, numString, tokenType) ->
                        if tokenType = "int" then
                            matchTokens chars (Int (int numString)::acc)
                        else
                            matchTokens chars (Float (float numString)::acc)
                    | Error err -> Error err
                | head :: tail when isLetter head ->
                    let (chars, var) = formString(tail, string head)
                    if isKeyword var then 
                        matchTokens chars (keywords[var]::acc)
                    else 
                        matchTokens chars (Variable var::acc)
                | _ -> Error InvalidToken
            matchTokens(strtochar str) []