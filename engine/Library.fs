namespace Engine
    module Tokeniser =

        type TokenType =
            | Variable
            | Plus
            | Minus
            | Multiply
            | Divide
            | LeftBracket
            | RightBracket
            | Comma //currently not needed but may be used if we choose to make the interpreter turing complete.
            | Number

        type Token = {
            Type: TokenType
            Value: string option //not all tokens have a value so it's optional
        }

        let createToken(tokenType: TokenType, value: string option) : Token = {
            Type = tokenType
            Value = value
        }

        // Helpers.
        let strtochar (str: string) : char list = [for c in str do yield c]
        let isDigit(c: char) = 
            System.Char.IsDigit c
        let isLetter(c: char) = 
            System.Char.IsLetter c
        let isComma (c: char) = c = ','

        let rec isFloat(chars: char list) = 
            match chars with 
            | c::tail when isDigit c   -> isFloat tail
            | c::tail when isLetter c  -> false
            | '.'::tail                -> false
            | _ -> true 

        let rec formFractionalPart(chars: char list, accStr : string) =
            match chars with
            | c::tail when isDigit c -> formFractionalPart(tail, accStr + string c)
            | _                      -> (chars, accStr)

        let rec formInt(chars: char list, accStr: string) = 
            match chars with
            | c :: tail when isDigit c    -> formInt(tail, accStr + string c)
            | '.'::tail when isFloat tail -> formFractionalPart(tail, accStr + string('.'))
            | _                           -> (chars, accStr)

        let rec formString(chars: char list, accStr: string) =
            match chars with
            | c::tail when isLetter c -> formString(tail, accStr + string c)
            | c::tail when isDigit c  -> formString(tail, accStr + string c)
            | _                       -> (chars, accStr)

        let tokenize (str : string) : Token list =
            let rec matchTokens chars = 
                match chars with 
                | []          -> []
                | '+' :: tail -> createToken(Plus, None)        :: matchTokens tail
                | '-' :: tail -> createToken(Minus, None)       :: matchTokens tail
                | '*' :: tail -> createToken(Multiply, None)    :: matchTokens tail
                | '/' :: tail -> createToken(Divide, None)      :: matchTokens tail
                | '(' :: tail -> createToken(LeftBracket, None) :: matchTokens tail
                | ')' :: tail -> createToken(RightBracket, None):: matchTokens tail
                | ' ' :: tail ->                                   matchTokens tail
                | head :: tail when isDigit head  -> let (chars, numString) = formInt(tail, string head)
                                                     createToken (Number, Some numString) 
                                                     :: matchTokens chars
                | head :: tail when isLetter head -> let (chars, var) = formString(tail, string head)
                                                     createToken (Variable, Some var) 
                                                     :: matchTokens chars
                | _ -> failwith "Invalid Token"
            matchTokens (strtochar str)