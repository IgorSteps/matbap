namespace Engine
    module Tokeniser =

        type LexicalError =
            | InvalidFloat of string
            | InvalidToken of string

        type NumberToken =
            | IntToken of int
            | FloatToken of float

        type Token =
            | Identifier of string
            | Int of int
            | Float of float
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
            | Log
            | Exp
            | For
            | In
            | Comma
            | Colon
            | Range
            | EOL

        // Map stores reserved keywords, used to replace string variables
        // with the corresponding enum type when encountered in the tokenizer function
        let private keywords = Map.empty.Add("sin", Sin)
                                        .Add("cos", Cos)
                                        .Add("tan", Tan)
                                        .Add("log", Log)
                                        .Add("exp", Exp)
                                        .Add("for", For)
                                        .Add("in", In)
                                        .Add("range", Range)

        // Helpers.
        let private strToChar(str: string)   = [for c in str do yield c]
        let private charToInt(char: char)    = int char - int '0'
        let private charToFloat(char: char)  = char |> charToInt |> float
        let private isDigit(c: char)         = System.Char.IsDigit c
        let private isLetter(c: char)        = System.Char.IsLetter c
        let private isAlphaNumeric(c: char)  = System.Char.IsLetterOrDigit c
        let private isKeyword(str: string)   = keywords.ContainsKey str

        let private createInvalidFloatError(errMsg: string) =
            errMsg |> InvalidFloat |> Result.Error
                
        let private createErrorWithPosition(err: LexicalError, position: int) =
            match err with 
            | InvalidFloat errMsg -> 
                ($"Invalid Float at token position {position}: " + errMsg) |> InvalidFloat |> Result.Error
            | InvalidToken errMsg -> 
                ($"Invalid Token at token position {position}: " + errMsg) |> InvalidToken |> Result.Error

        // chars = remaining char list to be tokenized
        // acc = accumulator storing the value of the token
        // multi = stores the position of the next char in the token i.e. tenth (0.1), hundredth (0.01)
        let rec private formFloat(chars: char list, acc: float, multi: float) =
            match chars with
                                        // c*multi because 1.7 gets processed as 1.0 + 7.0*0.1
            | c::tail when isDigit c -> formFloat(tail, acc + (charToFloat c * multi), multi/10.0)
            | '.'::_              -> createInvalidFloatError "Can't have 2 decimal places in a float"
            | _                      -> Ok(chars, FloatToken acc)
        
        let rec private formInt(chars: char list, acc: int) =
            match chars with
                                            // acc is multiplied by 10 to shift numbers along each call
                                            // i.e. 11 is tokenized as 1*10 + 1
            | c :: tail when isDigit c  -> formInt(tail, (acc*10) + charToInt c)
            | '.'::c::tail              -> match isDigit c with
                                           | true  -> formFloat(c::tail, float acc, 0.1)
                                           | false -> createInvalidFloatError ("the mantissa " + 
                                                             "cannot lead with non digit")
            | _                         -> Ok(chars, IntToken acc)

        let rec private formIdentifier(chars: char list, acc: string) =
            match chars with
            | c::tail when isAlphaNumeric c -> formIdentifier(tail, acc + string c)
            | _                             -> (chars, acc)
        
        let rec private matchTokens (chars: char list) (acc: Token list) =
                match chars with
                // A line is terminated once it either it reaches a ';' or all chars are consumed if it is the final line
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
                | ',' :: tail -> matchTokens tail (Comma::acc)
                | ':' :: tail -> matchTokens tail (Colon::acc)
                | ';' :: tail -> matchTokens tail (EOL::acc)
                | ' ' :: tail -> matchTokens tail acc
                | '\n':: tail -> matchTokens tail acc
                | '\r':: tail -> matchTokens tail acc
                | head :: tail when isDigit head ->
                    match formInt(tail, charToInt head) with
                    | Ok(chars, num) -> match num with
                                        | IntToken x   -> matchTokens chars (Int x::acc)
                                        | FloatToken x -> matchTokens chars (Float x::acc)
                    | Error err -> createErrorWithPosition (err, List.length acc + 1)

                | head :: tail when isLetter head ->
                    let chars, identifier = formIdentifier(tail, string head)
                    match isKeyword identifier with
                    | true  -> matchTokens chars (keywords[identifier]::acc)
                    | false -> matchTokens chars (Identifier identifier::acc)

                | head :: _ -> createErrorWithPosition (InvalidToken $"{head}", List.length acc + 1)
 
        let tokenise(str : string): Result<Token list , LexicalError> =
            matchTokens (strToChar str) []
           