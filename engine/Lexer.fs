namespace Engine
    module Tokeniser =

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
            | Sin
            | Cos
            | Tan

        let keywords = Map.empty.Add("sin", Sin).Add("cos", Cos).Add("cos", Cos)

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
        
        let rec formFractionalPart(chars: char list, accStr: string) =
            match chars with
            | c::tail when isDigit c -> formFractionalPart(tail, accStr + string c)
            | _                      -> (chars, accStr, "float")

        let rec formInt(chars: char list, accStr: string) = 
            match chars with
            | c :: tail when isDigit c    -> formInt(tail, accStr + string c)
            | '.'::tail when isFloat tail -> formFractionalPart(tail, accStr + string '.')
            | _                           -> (chars, accStr, "int")

        let rec formString(chars: char list, accStr: string) =
            match chars with
            | c::tail when isLetter c -> formString(tail, accStr + string c)
            | c::tail when isDigit c  -> formString(tail, accStr + string c)
            | _                       -> (chars, accStr)

        let tokenize (str : string) : TokenType list=
            let rec matchTokens chars =
                match chars with 
                | []          -> []
                | '+' :: tail -> Add         :: matchTokens tail
                | '-' :: tail -> Minus       :: matchTokens tail
                | '*' :: tail -> Multiply    :: matchTokens tail
                | '/' :: tail -> Divide      :: matchTokens tail
                | '(' :: tail -> LeftBracket :: matchTokens tail
                | ')' :: tail -> RightBracket:: matchTokens tail
                | '%' :: tail -> Modulus     :: matchTokens tail
                | ' ' :: tail ->                matchTokens tail
                | head :: tail when isDigit head  -> let (chars, numString, tokenType) = 
                                                        formInt(tail, string head)
                                                     if tokenType = "int" then
                                                        Int (int numString)  :: matchTokens chars
                                                     else
                                                        Float (float numString):: matchTokens chars
                | head :: tail when isLetter head -> let (chars, var) = formString(tail, string head)
                                                     if isKeyword var then 
                                                        keywords[var]:: matchTokens chars
                                                     else 
                                                        Variable var :: matchTokens chars
                | _ -> failwith "Invalid Token"
            matchTokens (strtochar str)