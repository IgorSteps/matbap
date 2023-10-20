namespace Engine
    module Parser =
        // Grammar for rewrite
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= <T> + <Eopt> | <T> - <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= <NR> * <Topt> | <NR> / <Topt> | <empty>
        // <NR>   ::= Num <value> | (E)
        let parseError = System.Exception("Parse error")
        let private parse tList =
            // Doesn't return anything but does parse
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
        
        let private parseEval tList =
            let rec E tList = (T >> Eopt) tList
            and Eopt (tList, inputValue) =
                match tList with
                    | Tokeniser.Add :: tail ->      let (remainingTokens, Tvalue) = T tail
                                                    Eopt (remainingTokens, inputValue+Tvalue)
                    | Tokeniser.Minus :: tail ->    let (remainingTokens, Tvalue) = T tail
                                                    Eopt (remainingTokens, inputValue+Tvalue)
                    | _ -> (tList, inputValue)
            and T tList = ( NR >> Topt ) tList
            and Topt (tList, inputValue) =
                match tList with
                | Tokeniser.Multiply :: tail -> let (remainingTokens, NRvalue) = NR tail
                                                Topt (remainingTokens, inputValue*NRvalue)
                | Tokeniser.Divide :: tail ->   let (remainingTokens, NRvalue) = NR tail
                                                Topt (remainingTokens, inputValue/NRvalue)
                | _ -> (tList, inputValue)
            and NR tList =
                match tList with
                | Tokeniser.Float value :: tail ->  (tail, value)
                | Tokeniser.Int value :: tail ->    (tail, value)
                | Tokeniser.LeftBracket :: tail ->  let (remainingTokens, Evalue) = E tail
                                                    match remainingTokens with
                                                    | Tokeniser.RightBracket :: tail -> (tail, Evalue)
                                                    | _ -> raise parseError
                | _ -> raise parseError
            
            try
                Ok (snd (E tList) : float)
            with
                | parseError -> Error "Error parsing expression."