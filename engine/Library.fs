namespace Engine
    module Tokeniser =
        type TokenType =
            | Digit
            | Letter
            | Operator
            | LeftBracket
            | RightBracket
            | Comma
            | Unknown

        type Token = {
            Type: TokenType
            Value: char
        }

        let createToken(tokenType: TokenType) (value: char) : Token = {
            Type = tokenType
            Value = value
         }

        // Helpers.
        let private isDigit(c: char) = 
            System.Char.IsDigit c
        let private isLetter(c: char) = 
            System.Char.IsLetter c
        let private isOperator(c: char) =
            match c with
            | '+' | '-' | '*' | '/' -> true
            | _ -> false
        let private isLeftBracket(c: char) =  c = '('
        let private isRightBracket(c: char) = c = ')'
        let private isComma (c: char) = c = ','
        let private removeSpaces (str: string) =
               str.Replace(" ", "")

        // Decide type.
        let private categorizeChar c =
            match c with
            | _ when isDigit c             -> createToken Digit c
            | _ when isLetter c            -> createToken Letter c
            | _ when isOperator c          -> createToken Operator c
            | _ when isLeftBracket c       -> createToken LeftBracket c
            | _ when isRightBracket c      -> createToken RightBracket c
            | _ when isComma c             -> createToken Comma c
            | _                            -> createToken Unknown c

        // Tokenise.
        let tokenise (str: string) : Result<Token list, string> =
            let noSpaceStr = removeSpaces str
            let tokens = [for c in noSpaceStr do yield categorizeChar c]

            // Return error if any Unknown tokens are found.
            match List.tryFind(fun t -> t.Type = Unknown) tokens with
            | Some token    -> Error(sprintf "Unknown token: %c" token.Value)
            | None          -> Ok tokens
    
    module Parser =
        let parse ( tokens : Tokeniser.Token list )  =
            0