namespace Engine
    module Parser =
        // Current grammar
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
            | Int of int
            | Exp of Exp
 
        // Recursive parse functions
        let rec parseExp tokens =
            match tokens with
            | [] -> failwith "Input ended unexpectedly"
            | Tokeniser.Add :: restTokens ->
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
            | Tokeniser.Int(int) :: restTokens -> Int(int), restTokens
            | Tokeniser.Float(float) :: restTokens -> Float(float), restTokens
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