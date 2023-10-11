namespace engine



module Tokeniser =
    type Token = {
        Type: string;
        Value: char
    }
    let createToken (tokenType: string) (value: char) : Token = {
        Type = tokenType
        Value = value
     }

    // Helpers.
    let isDigit char = 
        System.Char.IsDigit char
    let isLetter char = 
        System.Char.IsLetter char
    let isOperator c =
        match c with
        | '+' | '-' | '*' | '/' -> true
        | _ -> false
    let isLeftBracket char =  char = '('
    let isRightBracket char = char = ')'
    let isComma char = char = ','
    let removeSpaces (str: string) =
           str.Replace(" ", "")

    // Decide type.
    let categorizeChar char =
        match char with
        | _ when isDigit char             -> createToken "Digit" char
        | _ when isLetter char            -> createToken "Letter" char
        | _ when isOperator char          -> createToken "Operator" char
        | _ when isLeftBracket char       -> createToken "LeftBracket" char
        | _ when isRightBracket char      -> createToken "RightBracket" char
        | _ when isComma char             -> createToken "Comma" char
        | _                               -> createToken "Unknown" char

    // Tokenise.
    let tokenise str =
        let noSpaceStr = removeSpaces str

        // Run through the expression and split into tokens, returns list.
        [for char in noSpaceStr do yield categorizeChar char]

    // Test(for now).
    //let results = tokenise "11 + sin(20.4)"
    //for item in results do
    //    printfn "Type: %s, Value: %c" item.Type item.Value