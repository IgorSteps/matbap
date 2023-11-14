namespace Engine
    module Parser =
        open System.Collections.Generic
        open Tokeniser
        // Grammar:
        // <varA> ::= <varID> = <E>
        // <E>    ::= <T> <Eopt>
        // <Eopt> ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>    ::= <P> <Topt>
        // <Topt> ::= * <P> <Topt> | / <P> <Topt> | % <P> <Topt> | <empty>
        // <P>    ::= <NR> <Popt>
        // <Popt> ::= ^ <NR> <Popt> | <empty>
        // <NR>   ::= <NRpt> | -<NRpt>
        // <NRpt> ::= (E) | <num>
        // <num>  ::= <int> | <float> | <varVal>
        // varVal is fetched from symbol table using varID
        exception ParseErrorException of string
        // Define number type
        type NumType =
            | Int of int
            | Float of float

        let parseEval (tList : Token list list) (symTable : Dictionary<string, NumType>) =
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
                // For negative numbers must return negative of the NumType
                | Minus::tail ->    let numTail, (vID, num) = grammarNRpt tail
                                    match num with
                                    | Float x -> numTail, (vID, Float(-x))
                                    | Int x -> numTail, (vID, Int(-x))
                | _ -> grammarNRpt tList

            and grammarNRpt tList : Token list * (string * NumType) =
                match tList with
                // Follows grammar for brackets
                | LeftBracket::tail ->  let remainingTokens, (vID, valueE) = grammarE tail
                                        match remainingTokens with
                                        | RightBracket :: tail -> (tail, (vID, valueE))
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
                | Tokeniser.Identifier vName::tail  -> match symTable.ContainsKey vName with
                                                       | true  -> (tail, (vName, symTable.[vName]))
                                                       | false -> raise (ParseErrorException "Error while parsing: Identifier not found")
                | Tokeniser.Float x::tail           -> (tail, ("", Float(x)))
                | Tokeniser.Int   x::tail           -> (tail, ("", Int(x)))
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")

            let varA tList : Token list * (string * NumType) = 
                match tList with 
                | Tokeniser.Identifier vName :: tail -> match tail with 
                                                        | Tokeniser.Equals :: tail -> 
                                                            let (tLst, (_, tval)) = grammarE tail
                                                            (tLst, (vName, tval))
                                                        | _ -> grammarE tList
                | _ -> grammarE tList 

            let parseLines tList =
                let parseLine tList =
                    let (tList, (vID, tval)) = varA tList
                    match List.isEmpty tList with
                    | true -> match symTable.ContainsKey vID with
                              | true  -> symTable.[vID] <- tval
                              | false -> symTable.Add(vID, tval)
                              ((vID, tval), symTable)
                    | false  -> raise (ParseErrorException "Error while parsing: could not parse all of expression")
                let results = List.map parseLine tList
                List.last results

            // Parsing function raises an exception, so catches it and returns result appropriately
            try
                                                                         
                // For first call, assumes it starts with an varA, as it is the highest level of grammar
                Ok(parseLines tList)
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
            with
                | ParseErrorException value -> Error value