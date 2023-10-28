namespace Engine
    module Parser =
        // Grammar:
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= * <NR> <Topt> | / <NR> <Topt>
        //          | ^ <NR> <Topt> | % <NR> <Topt> | <empty>
        // <NR>   ::= <num> | (E)
        // <num>  ::= <int> | <float>
        exception ParseErrorException of string
        // Define number type
        type NumType =
            | Int of int
            | Float of float
        
        let parseEval (tList : Tokeniser.Token list) : Result<NumType,string> =
            // Recursive functions
            // For first call, assumes it starts with a T as part of an Eopt
            let rec grammarE tList = (grammarT >> grammarEopt) tList
            and grammarEopt (tList, inputValue) : Tokeniser.Token list * NumType =
                match tList with
                // Calls the function matching grammar on the tail after finding an appropriate token.
                // Then calls itself again, performing the appropriate operation.
                | Tokeniser.Add::tail ->    let remainingTokens, valueT = grammarT tail
                                            match (inputValue, valueT) with
                                            // Matching based on possibilities of float/int for operands
                                            | Int x, Int y     -> grammarEopt (remainingTokens, Int(x+y))
                                            | Int x, Float y   -> grammarEopt (remainingTokens, Float(float x+y))
                                            | Float x, Int y   -> grammarEopt (remainingTokens, Float(x+float y))
                                            | Float x, Float y -> grammarEopt (remainingTokens, Float(x+y))
                                            
                | Tokeniser.Minus::tail ->  let remainingTokens, valueT = grammarT tail
                                            match (inputValue, valueT) with
                                            | Int x, Int y     -> grammarEopt (remainingTokens, Int(x-y))
                                            | Int x, Float y   -> grammarEopt (remainingTokens, Float(float x-y))
                                            | Float x, Int y   -> grammarEopt (remainingTokens, Float(x-float y))
                                            | Float x, Float y -> grammarEopt (remainingTokens, Float(x-y))
                // If it finds nothing, means that it's the end of the Eopt
                | _ -> (tList, inputValue)
            and grammarT tList : Tokeniser.Token list * NumType =
                ( grammarNR >> grammarTopt ) tList
            and grammarTopt (tList, inputValue) : Tokeniser.Token list * NumType =
                match tList with
                // Similar to Eopt, with more operations
                | Tokeniser.Multiply::tail ->   let remainingTokens, valueNR = grammarNR tail
                                                match (inputValue, valueNR) with
                                                | Int x, Int y     -> grammarTopt (remainingTokens, Int(x*y))
                                                | Int x, Float y   -> grammarTopt (remainingTokens, Float(float x*y))
                                                | Float x, Int y   -> grammarTopt (remainingTokens, Float(x*float y))
                                                | Float x, Float y -> grammarTopt (remainingTokens, Float(x*y))
                                                
                | Tokeniser.Divide::tail ->     let remainingTokens, valueNR = grammarNR tail
                                                // Disallow division by 0
                                                if (valueNR = Float(0) || valueNR = Int(0))  then
                                                    raise (ParseErrorException "Error while parsing: division by 0")
                                                match (inputValue, valueNR) with
                                                // Integer division is fine, gets truncated
                                                | Int x, Int y     -> grammarTopt (remainingTokens, Int(x/y))
                                                | Int x, Float y   -> grammarTopt (remainingTokens, Float(float x/y))
                                                | Float x, Int y   -> grammarTopt (remainingTokens, Float(x/float y))
                                                | Float x, Float y -> grammarTopt (remainingTokens, Float(x/y))
                                                
                | Tokeniser.Power::tail ->      let remainingTokens, valueNR = grammarNR tail
                                                match (inputValue, valueNR) with
                                                // Exponent operation can only return float.
                                                // Still need to match to get value of x and y
                                                | Int x, Int y     -> grammarTopt (remainingTokens, Float(x**y))
                                                | Int x, Float y   -> grammarTopt (remainingTokens, Float(x**y))
                                                | Float x, Int y   -> grammarTopt (remainingTokens, Float(x**y))
                                                | Float x, Float y -> grammarTopt (remainingTokens, Float(x**y))
                                                
                | Tokeniser.Modulus::tail ->    let remainingTokens, valueNR = grammarNR tail
                                                match (inputValue, valueNR) with
                                                // Modulo operator should only be used with integers
                                                | Int x, Int y     -> grammarEopt (remainingTokens, Int(x%y))
                                                | _ -> failwith "Error while parsing: modulo cannot be used with floats"
                | _ -> (tList, inputValue)
            and grammarNR tList : Tokeniser.Token list * NumType =
                match tList with
                // Return number as a NumType
                | Tokeniser.Float value :: tail ->  (tail, Float(value))
                | Tokeniser.Int value :: tail ->    (tail, Int(value))
                // Follows grammar for brackets
                | Tokeniser.LeftBracket :: tail ->  let remainingTokens, valueE = grammarE tail
                                                    match remainingTokens with
                                                    | Tokeniser.RightBracket :: tail -> (tail, valueE)
                                                    | _ -> raise (ParseErrorException "Error while parsing: Unexpected token
                                                                  or end of expression")
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")
            
            // Parsing function raises an exception, so catches it and returns result appropriately
            try
                let result = grammarE tList
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
                if (fst result).IsEmpty then
                    Ok (snd result : NumType)
                else
                    raise (ParseErrorException "Error while parsing: Could not parse all of expression")
            with
                | ParseErrorException value -> Error value