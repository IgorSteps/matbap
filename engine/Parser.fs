namespace Engine
    module Parser =
        // Grammar for rewrite
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= * <NR> <Topt> | / <NR> <Topt> | <empty>
        // <NR>   ::= Num <value> | (E)
        let parseError = System.Exception("Parse error")
        // Doesn't return anything but does parse. Keeping code in case it's used to generate an AST in future,
        // and is a useful skeleton
        (*
        let private parse tList =
            let rec E tList = (T >> Eopt) tList
            and Eopt tList =
                match tList with
                | Tokeniser.Add :: tail -> (T >> Eopt) tail
                | Tokeniser.Minus :: tail -> (T >> Eopt) tail
                | _ -> tList
            and T tList = (NR >> Topt) tList
            and Topt tList =
                match tList with
                | Tokeniser.Multiply :: tail -> (NR >> Topt) tail
                | Tokeniser.Divide :: tail -> (NR >> Topt) tail
                | _ -> tList
            and NR tList =
                match tList with
                | Tokeniser.Int value :: tail -> tail
                | Tokeniser.Float value :: tail -> tail
                | Tokeniser.LeftBracket :: tail -> match E tail with
                                                   | Tokeniser.RightBracket :: tail -> tail
                                                   | _ -> raise parseError
                | _ -> raise parseError
            E tList
        *)
        
        let parseEval tList =
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
                                                grammarEopt (remainingTokens, inputValue+valueT)
                | _ -> (tList, inputValue)
            and grammarT tList = ( grammarNr >> grammarTopt ) tList
            and grammarTopt (tList, inputValue) =
                match tList with
                // Same as Eopt
                | Tokeniser.Multiply :: tail -> let remainingTokens, valueNR = grammarNr tail
                                                grammarTopt (remainingTokens, inputValue*valueNR)
                | Tokeniser.Divide :: tail ->   let remainingTokens, valueNR = grammarNr tail
                                                grammarTopt (remainingTokens, inputValue/valueNR)
                | _ -> (tList, inputValue)
            and grammarNr tList =
                match tList with
                // Floats and ints are both treated as floats for the sake of evaluation
                | Tokeniser.Float value :: tail ->  (tail, value)
                | Tokeniser.Int value :: tail ->    (tail, value)
                // Follows grammar for brackets
                | Tokeniser.LeftBracket :: tail ->  let remainingTokens, valueE = grammarE tail
                                                    match remainingTokens with
                                                    | Tokeniser.RightBracket :: tail -> (tail, valueE)
                                                    | _ -> raise parseError
                | _ -> raise parseError
            
            // Parsing function raises an exception, so catches it and returns result appropriately
            // TODO: return more detailed errors
            try
                let result = grammarE tList
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
                if (fst result).IsEmpty then
                    raise parseError
                else
                    Ok (snd result : float)
            with
                | parseError -> Error "Error parsing expression."