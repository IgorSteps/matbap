﻿namespace Engine
    module Parser =
        open System.Collections.Generic
        open Tokeniser
        // Grammar:
        // <StatementList> ::= <Statement> | <Statement> <StatementList>
        // <Statement> ::= <VarA> | <E> | <ForLoop>
        // <ForLoop>   ::= "for" "(" <VarA> ";" <E> ";" <VarA> ")"  { <Statement> }
        // <varA>      ::= <varID> = <E>
        // <E>         ::= <T> <Eopt>
        // <Eopt>      ::= + <T> <Eopt> | - <T> <Eopt> | <empty>
        // <T>         ::= <P> <Topt>
        // <Topt>      ::= * <P> <Topt> | / <P> <Topt> | % <P> <Topt> | <empty>
        // <P>         ::= <NR> <Popt>
        // <Popt>      ::= ^ <NR> <Popt> | <empty>
        // <NR>        ::= <NRpt> | -<NRpt>
        // <NRpt>      ::= (<E>) | <num> | <func-call>
        // <num>       ::= <int> | <float> | <varVal>

        // <func-call> ::= <func-name>(<E>)
        // <func-name> ::= sin | cos | tan | log | plot

        // varVal is fetched from symbol table using varID
        
        // Type definitions
        exception ParseErrorException of string
        type NumType =
            | Int of int
            | Float of float
        type SymbolTable = Dictionary<string, NumType>

        let parseEval (tList : Token list) (symTable : SymbolTable) =
            // Recursive functions

            let rec grammarE tList =
                (grammarT >> grammarEopt) tList
            
            // varName must be passed along all function calls now since it is needed at the lowest level of grammar
            and grammarEopt (tList, (varName, inputValue)) : Token list * (string * NumType) =
                match tList with
                // Calls the function matching grammar on the tail after finding an appropriate token.
                // Then calls itself again, performing the appropriate operation.
                | Add::tail ->    let remainingTokens, (varName, valueT) = grammarT tail
                                  match (inputValue, valueT) with
                                  // Matching based on possibilities of float/int for operands
                                  | Int x, Int y     -> grammarEopt (remainingTokens, (varName, Int(x+y)))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, (varName, Float(float x+y)))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, (varName, Float(x+float y)))
                                  | Float x, Float y -> grammarEopt (remainingTokens, (varName, Float(x+y)))
                                            
                | Minus::tail ->  let remainingTokens, (varName, valueT) = grammarT tail
                                  match (inputValue, valueT) with
                                  | Int x, Int y     -> grammarEopt (remainingTokens, (varName, Int(x-y)))
                                  | Int x, Float y   -> grammarEopt (remainingTokens, (varName, Float(float x-y)))
                                  | Float x, Int y   -> grammarEopt (remainingTokens, (varName, Float(x-float y)))
                                  | Float x, Float y -> grammarEopt (remainingTokens, (varName, Float(x-y)))
                // If it finds nothing, means that it's the end of the Eopt
                | _ -> (tList, (varName, inputValue))
                
            and grammarT tList : Token list * (string * NumType) =
                ( grammarP >> grammarTopt ) tList

            and grammarTopt (tList, (varName, inputValue)) : Token list * (string * NumType) =
                match tList with
                // Similar to Eopt, with more operations
                | Multiply::tail ->   let remainingTokens, (varName, valueP) = grammarP tail
                                      match (inputValue, valueP) with
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (varName, Int(x*y)))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, (varName, Float(float x*y)))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, (varName, Float(x*float y)))
                                      | Float x, Float y -> grammarTopt (remainingTokens, (varName, Float(x*y)))
                                                
                | Divide::tail ->     let remainingTokens, (varName, valueP) = grammarP tail
                                      // Disallow division by 0
                                      if (valueP = Float(0) || valueP = Int(0))  then
                                        raise (ParseErrorException "Error while parsing: division by 0")
                                      match (inputValue, valueP) with
                                      // Integer division is fine, gets truncated
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (varName, Int(x/y)))
                                      | Int x, Float y   -> grammarTopt (remainingTokens, (varName, Float(float x/y)))
                                      | Float x, Int y   -> grammarTopt (remainingTokens, (varName, Float(x/float y)))
                                      | Float x, Float y -> grammarTopt (remainingTokens, (varName, Float(x/y)))
                                                
                | Modulus::tail ->    let remainingTokens, (varName, valueNR) = grammarNR tail
                                      match (inputValue, valueNR) with
                                      // Modulo operator should only be used with integers
                                      | Int x, Int y     -> grammarTopt (remainingTokens, (varName, Int(x%y)))
                                      | _ -> raise (ParseErrorException ("Error while parsing: modulo cannot be used " +
                                                    "with floats"))
                                                
                | _ -> (tList, (varName, inputValue))
                
            and grammarP tList : Token list * (string * NumType) =
                ( grammarNR >> grammarPopt ) tList
                
            and grammarPopt (tList, (varName, inputValue)) : Token list * (string * NumType) =
                match tList with
                | Power::tail ->    let remainingTokens, (varName, valueP) = grammarP tail
                                    match (inputValue, valueP) with
                                    // Exponent operation can only return float.
                                    // Still need to match to get value of x and y
                                    | Int x, Int y     -> grammarPopt (remainingTokens, (varName, Float(x**y)))
                                    | Int x, Float y   -> grammarPopt (remainingTokens, (varName, Float(x**y)))
                                    | Float x, Int y   -> grammarPopt (remainingTokens, (varName, Float(x**y)))
                                    | Float x, Float y -> grammarPopt (remainingTokens, (varName, Float(x**y)))
                                                
                | _ -> (tList, (varName, inputValue))
                                                
            and grammarNR tList : Token list * (string * NumType) =
                match tList with
                // For negative numbers must return negative of the NumType
                | Minus::tail ->    let numTail, (varName, num) = grammarNRpt tail
                                    match num with
                                    | Float x -> numTail, (varName, Float(-x))
                                    | Int x -> numTail, (varName, Int(-x))
                | _ -> grammarNRpt tList

            and grammarNRpt tList : Token list * (string * NumType) =
                match tList with
                // Follows grammar for brackets
                | LeftBracket::tail ->  let remainingTokens, (varName, valueE) = grammarE tail
                                        match remainingTokens with
                                        | RightBracket :: tail -> (tail, (varName, valueE))
                                        | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                // For negative numbers must return negative of the NumType
                | Minus::tail    -> let numTail, (varName, num) = grammarNum tail
                                    match num with
                                    | Float x -> numTail, (varName, Float(-x))
                                    | Int   x -> numTail, (varName, Int(-x))
                                    
                | Sin::LeftBracket::tail -> let remainingTokens, (varName, valueE) = grammarE tail
                                            match remainingTokens with 
                                            | RightBracket::tail  -> match valueE with
                                                                     | Float x -> (tail, (varName, Float(sin x)))
                                                                     | Int x   -> (tail, (varName, Float(sin x)))
                                            | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                | Cos::LeftBracket::tail -> let remainingTokens, (varName, valueE) = grammarE tail
                                            match remainingTokens with 
                                            | RightBracket::tail  -> match valueE with
                                                                     | Float x -> (tail, (varName, Float(cos x)))
                                                                     | Int x   -> (tail, (varName, Float(cos x)))
                                            | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                | Tan::LeftBracket::tail -> let remainingTokens, (varName, valueE) = grammarE tail
                                            match remainingTokens with 
                                            | RightBracket::tail  -> match valueE with
                                                                     | Float x -> (tail, (varName, Float(tan x)))
                                                                     | Int x   -> (tail, (varName, Float(tan x)))
                                            | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                | Log::LeftBracket::tail -> let remainingTokens, (varName, valueE) = grammarE tail
                                            match remainingTokens with 
                                            | RightBracket::tail  -> match valueE with
                                                                     | Float x -> (tail, (varName, Float(log x)))
                                                                     | Int x   -> (tail, (varName, Float(log x)))
                                            | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                | Exp::LeftBracket::tail -> let remainingTokens, (varName, valueE) = grammarE tail
                                            match remainingTokens with 
                                            | RightBracket::tail  -> match valueE with
                                                                     | Float x -> (tail, (varName, Float(exp x)))
                                                                     | Int x   -> (tail, (varName, Float(exp x)))
                                            | _ -> raise (ParseErrorException ("Error while parsing: Unexpected token " +
                                                      "or end of expression"))
                | _ -> grammarNum tList
                
            and grammarNum tList : Token list * (string * NumType) =
                match tList with
                // Return number as a NumType
                | Identifier vName::tail  -> match symTable.ContainsKey vName with
                                             | true  -> (tail, ("", symTable[vName]))
                                             | false -> raise (ParseErrorException "Error while parsing: Identifier not found")
                | Tokeniser.Float x::tail           -> (tail, ("", Float(x)))
                | Tokeniser.Int   x::tail           -> (tail, ("", Int(x)))
                | _ -> raise (ParseErrorException "Error while parsing: Unexpected token or end of expression")
            let varA tList = 
                match tList with 
                | Identifier varName :: Equals :: tail -> let tLst, (_, tVal) = grammarE tail
                                                          (tLst, (varName, tVal))
                | _ -> grammarE tList 

            let grammarStatements tList =
                let rec parseStatement tList =
                    let remainingTokens, (varName, tVal) = varA tList
                    match varName with
                    // When no assigment was done, the result will not be stored in the sym table
                    | "" -> ()
                    | _  -> match symTable.ContainsKey varName with
                            | true  -> symTable[varName] <- tVal
                            | false -> symTable.Add(varName, tVal)
                    match remainingTokens with
                    | EOL::tail -> parseStatement tail  // More statements to parse
                    | _ -> ((varName, tVal), symTable)  // End of statements
            
                parseStatement tList
            // Parsing function raises an exception, so catches it and returns result appropriately
            try
                                                                         
                // For first call, assumes it starts with an varA, as it is the highest level of grammar
                Ok(grammarStatements tList)
                // Only return second (parsing result) if the list is empty.
                // If not empty then has not parsed whole expression. E.g. possible trailing right bracket
            with
                | ParseErrorException value -> Error value