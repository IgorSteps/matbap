namespace engine



module Tokeniser =
    type Token = {
        Type: string;
        Value: char
    }
    let CreateToken (tokenType: string) (value: char) : Token = {
        Type = tokenType
        Value = value
     }

    // Helpers.
    let private isDigit char = 
        System.Char.IsDigit char
    let private isLetter char = 
        System.Char.IsLetter char
    let private isOperator c =
        match c with
        | '+' | '-' | '*' | '/' -> true
        | _ -> false
    let private isLeftBracket char =  char = '('
    let private isRightBracket char = char = ')'
    let private isComma char = char = ','
    let private removeSpaces (str: string) =
           str.Replace(" ", "")

    // Decide type.
    let private categorizeChar char =
        match char with
        | _ when isDigit char             -> CreateToken "Digit" char
        | _ when isLetter char            -> CreateToken "Letter" char
        | _ when isOperator char          -> CreateToken "Operator" char
        | _ when isLeftBracket char       -> CreateToken "LeftBracket" char
        | _ when isRightBracket char      -> CreateToken "RightBracket" char
        | _ when isComma char             -> CreateToken "Comma" char
        | _                               -> CreateToken "Unknown" char

    // Tokenise.
    let Execute str =
        let noSpaceStr = removeSpaces str

        // Run through the expression and split into tokens, returns list.
        [for char in noSpaceStr do yield categorizeChar char]