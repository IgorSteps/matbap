namespace Engine
    module Tokeniser =

        type LexicalError =
            | InvalidFloat
            | InvalidToken

        type NumberToken =
            | IntToken of int
            | FloatToken of float

        type Token =
            | Variable of string
            | NumberToken of NumberToken
            | Add
            | Minus
            | Multiply
            | Divide
            | LeftBracket
            | RightBracket
            | Modulus
            | Power
            | Equals
            | Sin
            | Cos
            | Tan

        // Map stores reserved keywords, used to replace string variables
        // with the corresponding enum type when encountered in the tokenizer function
        let private keywords = Map.empty.Add("sin", Sin).Add("cos", Cos).Add("tan", Tan)

        // Helpers.
        let private strTochar(str: string) : char list = [for c in str do yield c]
        let private charToInt(char: char) = int char - int '0'

        let private isDigit(c: char)         = System.Char.IsDigit c
        let private isLetter(c: char)        = System.Char.IsLetter c
        let private noStartingSpace(c: char) = c <> ' '
        let private isKeyword(str: string)   = keywords.ContainsKey str

        // Takes fractional part of a float and checks that it doesn't lead with a space or character
        // then checks that no aditional decimal points are among the digits,
        // returns no error if it passed both checks, else it returns an InvalidFloat LexicalError
        let private isFloat(chars: char list) : Result<unit, LexicalError> =

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

        let rec private formFractionalPart(chars: char list, acc: float) =
            match chars with
            | c::tail when isDigit c -> formFractionalPart(tail, acc + float ((charToInt c)/10))
            | _                      -> Ok (chars, FloatToken acc)
        
        
        let rec private formInt(chars: char list, acc: int) =
            match chars with
            | c :: tail when isDigit c -> formInt(tail, (acc*10) + charToInt c)
            | '.'::tail                -> match isFloat tail with
                                          | Ok ()     -> formFractionalPart(tail, float acc)
                                            
                                          | Error err -> Error err
            | _                        -> Ok (chars, IntToken acc)

        let rec private formVariable(chars: char list, accStr: string) =
            match chars with
            | c::tail when isLetter c -> formVariable(tail, accStr + string c)
            | c::tail when isDigit c  -> formVariable(tail, accStr + string c)
            | _                       -> (chars, accStr)
        
        // Tail recursive because it has to be wrapped in a result
        let tokenise(str : string): Result<Token list, LexicalError> =
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
                | '^' :: tail -> matchTokens tail (Power::acc)
                | '=' :: tail -> matchTokens tail (Equals::acc)
                | ' ' :: tail -> matchTokens tail acc

                | head :: tail when isDigit head ->
                    match formInt(tail, int head) with
                    | Ok (chars, num) -> matchTokens chars ( NumberToken num::acc)
                    | Error err -> Error err

                | head :: tail when isLetter head ->
                    let (chars, var) = formVariable(tail, string head)
                    if isKeyword var then 
                        matchTokens chars (keywords[var]::acc)
                    else 
                        matchTokens chars (Variable var::acc)
                | _ -> Error InvalidToken

            matchTokens(strTochar str) []