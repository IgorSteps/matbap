﻿namespace Engine
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
        
        let parseEval (tList : Token list) : Result<NumType,string> =
            // Recursive functions
            let rec grammarE tList =
                (grammarT >> grammarEopt) tList

            and grammarEopt (tList, inputValue) : Token list * NumType =
                match tList with
                // Calls the function matching grammar on the tail after finding an appropriate token.
                // Then calls itself again, performing the appropriate operation.
                | Add::tail ->    let remainingTokens, valueT = grammarT tail
                                  match (inputValue, valueT) with
                                  // Matching based on possibilities of float/int for operands
                                  | Int x, Int y     -> grammarEopt (remainingTokens, Int(x+y))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, Float(float x+y))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, Float(x+float y))
                                  | Float x, Float y -> grammarEopt (remainingTokens, Float(x+y))
                                            
                | Minus::tail ->  let remainingTokens, valueT = grammarT tail
                                  match (inputValue, valueT) with
                                  | Int x, Int y     -> grammarEopt (remainingTokens, Int(x-y))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, Float(float x-y))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, Float(x-float y))
                                  | Float x, Float y -> grammarEopt (remainingTokens, Float(x-y))
                // If it finds nothing, means that it's the end of the Eopt
                | _ -> (tList, inputValue)
                
            and grammarT tList : Token list * NumType =
                ( grammarP >> grammarTopt ) tList

            and grammarTopt (tList, inputValue) : Token list * NumType =
                match tList with
                // Similar to Eopt, with more operations
                | Multiply::tail ->   let remainingTokens, valueP = grammarP tail
                                      match (inputValue, valueP) with
                                      | Int x, Int y     -> grammarTopt (remainingTokens, Int(x*y))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, Float(float x*y))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, Float(x*float y))
                                      | Float x, Float y -> grammarTopt (remainingTokens, Float(x*y))
                                                
                | Divide::tail ->     let remainingTokens, valueP = grammarP tail
                                      // Disallow division by 0
                                      if (valueP = Float(0) || valueP = Int(0))  then
                                        raise (ParseErrorException "Error while parsing: division by 0")
                                      match (inputValue, valueP) with
                                      // Integer division is fine, gets truncated
                                      | Int x, Int y     -> grammarTopt (remainingTokens, Int(x/y))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, Float(float x/y))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, Float(x/float y))
                                      | Float x, Float y -> grammarTopt (remainingTokens, Float(x/y))
                                                
                | Modulus::tail ->    let remainingTokens, valueNR = grammarNR tail
                                      match (inputValue, valueNR) with
                                      // Modulo operator should only be used with integers
                                      | Int x, Int y     -> grammarTopt (remainingTokens, Int(x%y))
                                      | _ -> raise (ParseErrorException ("Error while parsing: modulo cannot be used " +
                                                    "with floats"))
                                                
                | _ -> (tList, inputValue)
                
            and grammarP tList : Token list * NumType =
                ( grammarNR >> grammarPopt ) tList
                
            and grammarPopt (tList, inputValue) : Token list * NumType =
                match tList with
                | Power::tail ->    let remainingTokens, valueP = grammarP tail
                                    match (inputValue, valueP) with
                                    // Exponent operation can only return float.
                                    // Still need to match to get value of x and y
                                    | Int x, Int y     -> grammarPopt (remainingTokens, Float(x**y))
                                    | Int x, Float y   -> grammarPopt (remainingTokens, Float(x**y))
                                    | Float x, Int y   -> grammarPopt (remainingTokens, Float(x**y))
                                    | Float x, Float y -> grammarPopt (remainingTokens, Float(x**y))
                                                
                | _ -> (tList, inputValue)
                                                
            and grammarNR tList : Token list * NumType =
                match tList with
                // Check negative brackets before anything else (more specific)
                | Minus::LeftBracket::tail ->   let remainingTokens, valueE = grammarE tail
                                                match remainingTokens with
                                                | RightBracket :: tail -> match valueE with
                                                                          | Float x -> tail, Float(-x)
                                                                          | Int x -> tail, Int(-x)
                                                | _ -> raise (ParseErrorException ("Error while parsing: Unexpected " +
                                                              "token or end of expression"))
                // Follows grammar for brackets
                | LeftBracket::tail ->  let remainingTokens, valueE = grammarE tail
                                        match remainingTokens with
                                        | RightBracket :: tail -> (tail, valueE)
                                        | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                // For negative numbers must return negative of the NumType
                | Minus::tail ->    let numTail, num = grammarNum tail
                                    match num with
                                    | Float x -> numTail, Float(-x)
                                    | Int x -> numTail, Int(-x)
                | _ -> grammarNum tList
                
            and grammarNum tList : Token list * NumType =
                match tList with
                // Return number as a NumType
                | Tokeniser.Float x :: tail ->  (tail, Float(x))
                | Tokeniser.Int x :: tail ->    (tail, Int(x))
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")
            
            // Parsing function raises an exception, so catches it and returns result appropriately
            try
                // For first call, assumes it starts with an E, as it is the highest level of grammar
                let result = grammarE tList
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
                if (fst result).IsEmpty then
                    Ok (snd result : NumType)
                else
                    raise (ParseErrorException "Error while parsing: Could not parse all of expression")
            with
                | ParseErrorException value -> Error value