namespace Engine
    module Parser =
        // Grammar:
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= * <NR> <Topt> | / <NR> <Topt> | <empty>
        // <NR>   ::= <int> | <float> | (E)
        exception ParseErrorException of string
        
        let parseEval (tList : Tokeniser.Token list) : Result<float,string> =
            // Recursive functions
            // For first call, assumes it starts with a T as part of an Eopt
            let rec grammarE tList = (grammarT >> grammarEopt) tList
            and grammarEopt (tList, inputValue) =
                match tList with
                // Calls the function matching grammar on the tail after finding an appropriate token.
                // Then calls itself again, performing the appropriate operation.
                | Tokeniser.Add :: tail ->      let remainingTokens, valueT = grammarT tail
                                                grammarEopt (remainingTokens, inputValue+valueT)
                | Tokeniser.Minus :: tail ->    let remainingTokens, valueT = grammarT tail
                                                grammarEopt (remainingTokens, inputValue-valueT)
                | _ -> (tList, inputValue)
            and grammarT tList = ( grammarNr >> grammarTopt ) tList
            and grammarTopt (tList, inputValue) =
                match tList with
                // Same as Eopt
                | Tokeniser.Multiply :: tail -> let remainingTokens, valueNR = grammarNr tail
                                                grammarTopt (remainingTokens, inputValue*valueNR)
                | Tokeniser.Divide :: tail ->   let remainingTokens, valueNR = grammarNr tail
                                                // Disallow division by 0. Must be a float to match valueNR
                                                if valueNR = float 0 then
                                                    raise (ParseErrorException "Error while parsing: division by 0")
                                                grammarTopt (remainingTokens, inputValue/valueNR)
                | _ -> (tList, inputValue)
            and grammarNr tList =
                match tList with
                // Floats and ints are both treated as floats for the sake of evaluation
                | Tokeniser.Float value :: tail ->  (tail, value)
                | Tokeniser.Int value :: tail ->    (tail, float value)
                // Follows grammar for brackets
                | Tokeniser.LeftBracket :: tail ->  let remainingTokens, valueE = grammarE tail
                                                    match remainingTokens with
                                                    | Tokeniser.RightBracket :: tail -> (tail, valueE)
                                                    | _ -> raise (ParseErrorException "Error while parsing: Unexpected token
                                                                  or end of expression")
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")
            
            // Parsing function raises an exception, so catches it and returns result appropriately
            // TODO: return more detailed errors
            try
                let result = grammarE tList
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
                if (fst result).IsEmpty then
                    Ok (snd result : float)
                else
                    raise (ParseErrorException "Error while parsing: Could not parse all of expression")
            with
                | ParseErrorException value -> Error value