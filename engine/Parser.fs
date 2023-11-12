namespace Engine
    module Parser =
        open Tokeniser
        // Grammar:
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <P> <Topt>
        // <Topt> ::= * <P> <Topt> | / <P> <Topt> | % <P> <Topt> | <empty>
        // <P>    ::= <NR> <Popt>
        // <Popt> ::= ^ <NR> <Popt> | <empty>
        // <NR>   ::= (E) | -(E) | <num> | -<num>
        // <num>  ::= <int> | <float>
        exception ParseErrorException of string
        // Define number type
        type NumType =
            | Int of int
            | Float of float
        
        let rec searchSym(varID : string) (symTable : list<string*NumType>) : bool*NumType =
            match symTable with
            | head :: tail ->   if (fst head = varID) then (true, snd head)
                                else searchSym varID tail
            | _ ->              (false, Int 0)
        // Helper function to set value within symbol table (returning new one)
        let rec setSym (varID : string) (varVal : NumType) (symTable : list<string*NumType>) : list<string*NumType> =
            match symTable with
            | head :: tail -> if (fst head) = varID then [(varID,varVal)]@tail // Return updated value, plus tail
                              else [head]@(setSym varID varVal symTable) // Continue searching if not found
            | _ -> []

        let parseEval (tList : Token list) (symTable : list<string*NumType>)  : Result<NumType,string> =
            // Recursive functions
            let rec grammarE tList =
                (grammarT >> grammarEopt) tList

            and grammarEopt (tList, (vID, inputValue)) : Token list * (string * NumType) =
                match tList with
                // Calls the function matching grammar on the tail after finding an appropriate token.
                // Then calls itself again, performing the appropriate operation.
                | Add::tail ->    let remainingTokens, (vID, valueT) = grammarT tail
                                  match (inputValue, valueT) with
                                  // Matching based on possibilities of float/int for operands
                                  | Int x, Int y     -> grammarEopt (remainingTokens, (vID, Int(x+y)))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, (vID, Float(float x+y)))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, (vID, Float(x+float y)))
                                  | Float x, Float y -> grammarEopt (remainingTokens, (vID, Float(x+y)))
                                            
                | Minus::tail ->  let remainingTokens, (vID, valueT) = grammarT tail
                                  match (inputValue, valueT) with
                                  | Int x, Int y     -> grammarEopt (remainingTokens, (vID, Int(x-y)))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, (vID, Float(float x-y)))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, (vID, Float(x-float y)))
                                  | Float x, Float y -> grammarEopt (remainingTokens, (vID, Float(x-y)))
                // If it finds nothing, means that it's the end of the Eopt
                | _ -> (tList, (vID, inputValue))
                
            and grammarT tList : Token list * (string * NumType) =
                ( grammarP >> grammarTopt ) tList

            and grammarTopt (tList, (vID, inputValue)) : Token list * (string * NumType) =
                match tList with
                // Similar to Eopt, with more operations
                | Multiply::tail ->   let remainingTokens, (vID, valueP) = grammarP tail
                                      match (inputValue, valueP) with
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (vID, Int(x*y)))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, (vID, Float(float x*y)))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, (vID, Float(x*float y)))
                                      | Float x, Float y -> grammarTopt (remainingTokens, (vID, Float(x*y)))
                                                
                | Divide::tail ->     let remainingTokens, (vID, valueP) = grammarP tail
                                      // Disallow division by 0
                                      if (valueP = Float(0) || valueP = Int(0))  then
                                        raise (ParseErrorException "Error while parsing: division by 0")
                                      match (inputValue, valueP) with
                                      // Integer division is fine, gets truncated
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (vID, Int(x/y)))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, (vID, Float(float x/y)))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, (vID, Float(x/float y)))
                                      | Float x, Float y -> grammarTopt (remainingTokens, (vID, Float(x/y)))
                                                
                | Modulus::tail ->    let remainingTokens, (vID, valueNR) = grammarNR tail
                                      match (inputValue, valueNR) with
                                      // Modulo operator should only be used with integers
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (vID, Int(x%y)))
                                      | _ -> raise (ParseErrorException ("Error while parsing: modulo cannot be used " +
                                                    "with floats"))
                                                
                | _ -> (tList, (vID, inputValue))
                
            and grammarP tList : Token list * (string * NumType) =
                ( grammarNR >> grammarPopt ) tList
                
            and grammarPopt (tList, (vID, inputValue)) : Token list * (string * NumType) =
                match tList with
                | Power::tail ->    let remainingTokens, (vID, valueP) = grammarP tail
                                    match (inputValue, valueP) with
                                    // Exponent operation can only return float.
                                    // Still need to match to get value of x and y
                                    | Int x, Int y     -> grammarPopt (remainingTokens, (vID, Float(x**y)))
                                    | Int x, Float y   -> grammarPopt (remainingTokens, (vID, Float(x**y)))
                                    | Float x, Int y   -> grammarPopt (remainingTokens, (vID, Float(x**y)))
                                    | Float x, Float y -> grammarPopt (remainingTokens, (vID, Float(x**y)))
                                                
                | _ -> (tList, (vID, inputValue))
                                                
            and grammarNR tList : Token list * (string * NumType) =
                match tList with
                // Check negative brackets before anything else (more specific)
                | Minus::LeftBracket::tail ->   let remainingTokens, (vID, valueE) = grammarE tail
                                                match remainingTokens with
                                                | RightBracket :: tail -> match valueE with
                                                                          | Float x -> tail, (vID, Float(-x))
                                                                          | Int x -> tail, (vID, Int(-x))
                                                | _ -> raise (ParseErrorException ("Error while parsing: Unexpected " +
                                                              "token or end of expression"))
                // Follows grammar for brackets
                | LeftBracket::tail ->  let remainingTokens, valueE = grammarE tail
                                        match remainingTokens with
                                        | RightBracket :: tail -> (tail, valueE)
                                        | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                // For negative numbers must return negative of the NumType
                | Minus::tail ->    let numTail, (vID, num) = grammarNum tail
                                    match num with
                                    | Float x -> numTail, (vID, Float(-x))
                                    | Int x -> numTail, (vID, Int(-x))
                | _ -> grammarNum tList
                
            and grammarNum tList : Token list * (string * NumType) =
                match tList with
                // Return number as a NumType
                | Tokeniser.Identifier vName::tail  -> let result = searchSym vName symTable
                                                       if (fst result) then (tail, ("", (snd result)))
                                                       else raise (ParseErrorException "Error while parsing: Identifier not found") 
                | Tokeniser.Float x::tail           -> (tail, ("", Float(x)))
                | Tokeniser.Int   x::tail           -> (tail, ("", Int(x)))
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")

            let varA tList = 
                match tList with 
                | Tokeniser.Identifier vName :: tail -> match tail with 
                                                        | Tokeniser.Equals :: tail -> 
                                                            let (tLst, (vID, tval)) = grammarE tail
                                                            (tLst, (vName, tval))
                                                        | _ -> grammarE tList
                | _ -> grammarE tList
            
            // Parsing function raises an exception, so catches it and returns result appropriately
            try
                // For first call, assumes it starts with an E, as it is the highest level of grammar
                let result = varA tList
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
                if (fst result).IsEmpty then
                    Ok (snd (snd result) : NumType)
                else
                    raise (ParseErrorException "Error while parsing: Could not parse all of expression")
            with
                | ParseErrorException value -> Error value