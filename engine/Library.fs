namespace engine



module Tokeniser =
    type Token = {
        Type: string;
        Value: char
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
    let isLeftParenthesis char =  char = '('
    let isRightParenthesis char = char = ')'
    let isComma char = char = ','
    let removeSpaces (str: string) =
           str.Replace(" ", "")

    // Decide type.
    let categorizeChar char =
        match char with
        | _ when isDigit char             -> { Type = "Literal"; Value = char }
        | _ when isLetter char            -> { Type = "Variable"; Value = char }
        | _ when isOperator char          -> { Type = "Operator"; Value = char }
        | _ when isLeftParenthesis char   -> { Type = "LeftParenthesis"; Value = char }
        | _ when isRightParenthesis char  -> { Type = "RightParenthesis"; Value = char }
        | _ when isComma char             -> { Type = "FunctionArgumentSeparator"; Value = char }
        | _                               -> { Type = "Unknown"; Value = char }

    // Tokenise.
    let tokenise str =
        let noSpaceStr = removeSpaces str
        
        [for char in noSpaceStr do
            yield
                categorizeChar char]

    // Test(for now).
    let results = tokenise "a+1,(b-2)"
    for item in results do
        printfn "Type: %s, Value: %c" item.Type item.Value
    
    
    
        