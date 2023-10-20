namespace Engine
    module Parser =
        // Grammar for rewrite
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <NR> <Topt>
        // <Topt> ::= * <NR> <Topt> | / <NR> <Topt> | <empty>
        // <NR>   ::= Num <value> | (E)
        type Ast =
            | Ident of string
            | Val of System.Double
            | Multi of Ast * Ast
            | Div of Ast * Ast
            | Plus of Ast * Ast
            | Minus of Ast * Ast
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
        
        let parseEval tList =
            let rec E tList = (T >> Eopt) tList
            and Eopt (tList, inval) =
                match tList with
                    | Tokeniser.Add :: tail ->      let (ts1, Tvalue) = T tail
                                                    Eopt (ts1, inval+Tvalue)
                    | Tokeniser.Minus :: tail ->    let (ts1, Tvalue) = T tail
                                                    Eopt (ts1, inval+Tvalue)
                    | _ -> (tList, inval)
            and T tList = ( NR >> Topt ) tList
            and Topt (tList, inval) =
                match tList with
                | Tokeniser.Multiply :: tail -> let (ts1, NRvalue) = NR tail
                                                Topt (ts1, inval*NRvalue)
                | Tokeniser.Divide :: tail ->   let (ts1, NRvalue) = NR tail
                                                Topt (ts1, inval/NRvalue)
                | _ -> (tList, inval)
            and NR tList =
                match tList with
                | Tokeniser.Float value :: tail ->  (tail, value)
                | Tokeniser.Int value :: tail ->    (tail, value)
                | Tokeniser.LeftBracket :: tail ->  let (ts1, Evalue) = E tail
                                                    match ts1 with
                                                    | Tokeniser.RightBracket :: tail -> (tail, Evalue)
                                                    | _ -> raise parseError
                | _ -> raise parseError
            E tList