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
        // Grammar defined as follows:
        // <expr> ::= <term>
        // | <expr> + <term>
        // | <expr> - <term>
        // <term> ::= <factor>
        // | <term> * <factor>
        // | <term> / <factor>
        // <factor> ::= <float> | (<expr>)
        type Exp =
            | Term of Term
            | Add of Exp * Term
            | Subtract of Exp * Term
        and Term =
            | Factor of Factor
            | Multiply of Term * Factor
            | Divide of Term * Factor
        and Factor =
            | Float of float
            | Exp of Exp
 
        // Recursive parse functions
        let rec parseExp tokens =
            match tokens with
            | [] -> failwith "Input ended unexpectedly"
            | Tokeniser.Plus :: restTokens ->
                let term, remainingTokens = parseTerm restTokens
                let exp, finalTokens = parseExp remainingTokens
                Add(exp, term), finalTokens
            | Tokeniser.Minus :: restTokens ->
                let term, remainingTokens = parseTerm restTokens
                let exp, finalTokens = parseExp remainingTokens
                Subtract(exp, term), finalTokens
            | _ :: restTokens ->
                let term, finalTokens = parseTerm restTokens
                Term term, finalTokens
        and parseTerm tokens =
            match tokens with
            | [] -> failwith "Input ended unexpectedly"
            | Tokeniser.Multiply :: restTokens ->
                let factor, remainingTokens = parseFactor restTokens
                let term, finalTokens = parseTerm remainingTokens
                Multiply(term, factor), finalTokens
            | Tokeniser.Divide :: restTokens ->
                let factor, remainingTokens = parseFactor restTokens
                let term, finalTokens = parseTerm remainingTokens
                Divide(term, factor), finalTokens
            | _ :: restTokens ->
                let factor, finalTokens = parseFactor restTokens
                Factor factor, finalTokens
        and parseFactor tokens =
            match tokens with
            | [] -> failwith "Input ended unexpectedly"
            | Tokeniser.Number(float) :: restTokens -> Float(float), restTokens
            | Tokeniser.LeftBracket :: restTokens ->
                let exp, remainingTokens = parseExp restTokens
                match remainingTokens with
                | Tokeniser.RightBracket :: finalTokens -> Exp(exp), finalTokens
                | _ -> failwith "Could not find closing parenthesis"
            | _ -> failwith "Found an invalid factor"

        let parse input =
            let ast, remainingTokens = parseExp input
            if remainingTokens = [] then
                Some ast
            else
                None