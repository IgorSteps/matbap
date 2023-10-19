namespace Engine
    module Parser =
        // Grammar for rewrite
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= * <NR> <Topt> | / <NR> <Topt> | <empty>
        // <NR>   ::= Num <value> | (E)
        
        let parseError = System.Exception("Parse error")
        let parse tList =
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
                // Parser raises an error. Will make this method private and try/catch it
                | Tokeniser.LeftBracket :: tail -> match E tail with
                                                   | Tokeniser.RightBracket :: tail -> tail
                                                   | _ -> raise parseError
                | _ -> raise parseError
            E tList
        
        