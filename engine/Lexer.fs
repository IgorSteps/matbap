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
            | Number
            | Sin
            | Cos
            | Tan

        let keywords = Map.empty.Add("sin", Sin).Add("cos", Cos).Add("cos", Cos)

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

        let isDigit(c: char)         = System.Char.IsDigit c
        let isLetter(c: char)        = System.Char.IsLetter c
        let isComma (c: char)        = c = ','
        let noStartingSpace(c: char) = if c <> ' ' then true else false
        let isKeyword (str: string)  = if keywords.ContainsKey str then true else false

        let rec isFloat(chars: char list): bool =

            let firstChar = if noStartingSpace chars.Head && not (isLetter chars.Head) then
                                true 
                            else
                                failwith "Invalid float"

            let rec postFirstChar chars =
                match chars with
                    | c::tail when isDigit c -> postFirstChar tail
                    | '.'::_                 -> false
                    | _                      -> true
    
            postFirstChar chars

        let rec formNum(chars: char list, accStr: string) = 
            match chars with
            | c :: tail when isDigit c    -> formNum(tail, accStr + string c)
            | '.'::tail when isFloat tail -> formNum(tail, accStr + string '.')
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
                | head :: tail when isDigit head  -> let (chars, numString) = formNum(tail, string head)
                                                     createToken (Number, Some numString) 
                                                     :: matchTokens chars
                | head :: tail when isLetter head -> let (chars, var) = formString(tail, string head)
                                                     if isKeyword var then 
                                                        createToken(keywords[var], None)
                                                        :: matchTokens chars
                                                     else 
                                                        createToken (Variable, Some var) 
                                                        :: matchTokens chars
                | _ -> failwith "Invalid Token"
            matchTokens (strtochar str)