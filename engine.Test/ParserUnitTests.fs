namespace Engine.Tests

open Engine
open Engine.Parser
open NUnit.Framework
open System.Collections.Generic


type ParserTestCase = {
    Args: Tokeniser.Token list;
    Expected: Result<((string * NumType) * Dictionary<string, NumType>), string>;
}

module Helper = 
    let createDictionary (vName: string) (tval: NumType) = dict [vName, tval] |> Dictionary
                                                       
[<TestFixture>]
type ParserTests () =
    static member parserTestCases: ParserTestCase list = [
       {
            // Basic addition
            Args = [Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 10]
            Expected = Ok (("", Int 12),  Dictionary<string, Parser.NumType>())
       }
       {
            // Basic subtraction
            Args = [Tokeniser.Int 7; Tokeniser.Minus; Tokeniser.Int 3]
            Expected = Ok (("", Int 4),  Dictionary<string, Parser.NumType>())
       }
       {
            // Basic multiplication
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.Int 7]
            Expected = Ok (("", Int 35),  Dictionary<string, Parser.NumType>())
       }
       {
            // Basic division, with integer truncation
            Args = [Tokeniser.Int 12; Tokeniser.Divide; Tokeniser.Int 8]
            Expected = Ok (("", Int 1),  Dictionary<string, Parser.NumType>())
       }  
       {
            // Using integers with floats
            Args = [Tokeniser.Int 5; Tokeniser.Add; Tokeniser.Float 2.3]
            Expected = Ok (("", Float 7.3),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 5; Tokeniser.Minus; Tokeniser.Float 2.3]
            Expected = Ok(("", Float 2.7),  Dictionary<string, Parser.NumType>())
       }
       {
            // As above but for a different grammar
            Args = [Tokeniser.Float 4.62; Tokeniser.Multiply; Tokeniser.Int 9]
            Expected = Ok (("", Float 41.58),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 49; Tokeniser.Divide; Tokeniser.Float 7]
            Expected = Ok (("", Float 7),  Dictionary<string, Parser.NumType>())

       }
       {
            // Using multiple operations together
            Args = [Tokeniser.Float 11.53; Tokeniser.Minus; Tokeniser.Int 23; Tokeniser.Add; Tokeniser.Int 612]
            Expected = Ok (("", Float 600.53),  Dictionary<string, Parser.NumType>())
       }
       {
            // Testing proper order of operations
            Args = [Tokeniser.Float 13.56; Tokeniser.Minus; Tokeniser.Int 6; Tokeniser.Add; Tokeniser.Int 14;
                    Tokeniser.Multiply; Tokeniser.Float 20.1]
            Expected = Ok (("", Float 288.96),  Dictionary<string, Parser.NumType>())

       }
       {
            // Using brackets with an expression inside
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Int 3; Tokeniser.Add;
                    Tokeniser.Float 0.5; Tokeniser.RightBracket]
            Expected = Ok (("", Float 17.5),  Dictionary<string, Parser.NumType>())

       }
       {
            // Order of operations using brackets, and operations after brackets
            Args = [Tokeniser.Int 5; Tokeniser.Minus; Tokeniser.Float 2.5; Tokeniser.Divide; Tokeniser.LeftBracket;
                    Tokeniser.Float 6; Tokeniser.Add; Tokeniser.Float 6.5; Tokeniser.RightBracket; Tokeniser.Add;
                    Tokeniser.Int(1)]
            Expected = Ok (("", Float 5.8),  Dictionary<string, Parser.NumType>())

       }
       {
            Args = [Tokeniser.Float 2.5; Tokeniser.Add; Tokeniser.LeftBracket; Tokeniser.LeftBracket;
                    Tokeniser.Float 2.5; Tokeniser.RightBracket; Tokeniser.RightBracket; Tokeniser.Multiply;
                    Tokeniser.Int 3]
            Expected = Ok (("", Float 10),  Dictionary<string, Parser.NumType>())

       }
       {
            // Return a value on its own without operations
            Args = [Tokeniser.Int 9999]
            Expected = Ok (("", Int 9999),  Dictionary<string, Parser.NumType>())

       }
       {
            // As above, but inside brackets
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 0; Tokeniser.RightBracket]
            Expected = Ok (("", Int 0),  Dictionary<string, Parser.NumType>())

       }
       {
            // Negative addition
            Args = [Tokeniser.Int 3; Tokeniser.Add; Tokeniser.Minus; Tokeniser.Int 2]
            Expected = Ok (("", Int 1),  Dictionary<string, Parser.NumType>())

       }
       {
            // Negative subtraction
            Args = [Tokeniser.Int 4; Tokeniser.Minus; Tokeniser.Minus; Tokeniser.Int 2]
            Expected = Ok (("", Int 6),  Dictionary<string, Parser.NumType>())

       }
       {
            // Negative multiplication
            Args = [Tokeniser.Minus; Tokeniser.Int 3; Tokeniser.Multiply; Tokeniser.Minus; Tokeniser.Int 9]
            Expected = Ok (("", Int 27),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 6; Tokeniser.Multiply; Tokeniser.Minus; Tokeniser.Float 10.5]
            Expected = Ok (("", Float -63),  Dictionary<string, Parser.NumType>())

       }
       {
            // Negative division
            Args = [Tokeniser.Int 8; Tokeniser.Divide; Tokeniser.Minus; Tokeniser.Int 2]
            Expected = Ok (("", Int -4),  Dictionary<string, Parser.NumType>())

       }
       {
            Args = [Tokeniser.Minus; Tokeniser.Int 320; Tokeniser.Divide; Tokeniser.Minus; Tokeniser.Int 64]
            Expected = Ok (("", Int 5),  Dictionary<string, Parser.NumType>())

       }
       {
            // Negative brackets
            Args = [Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Int 6; Tokeniser.Add; Tokeniser.Int 11;
                    Tokeniser.RightBracket]
            Expected = Ok (("", Int -17),  Dictionary<string, Parser.NumType>())

       }
       {
            Args = [Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Int 8; Tokeniser.Add; Tokeniser.LeftBracket;
                    Tokeniser.Minus; Tokeniser.Int 24; Tokeniser.RightBracket; Tokeniser.RightBracket]
            Expected = Ok (("", Int 16),  Dictionary<string, Parser.NumType>())

       }
       {
            // Test for exponent
            Args = [Tokeniser.Int 2; Tokeniser.Power; Tokeniser.Int 8]
            Expected = Ok (("", Float 256),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 6; Tokeniser.Power; Tokeniser.Float 2.3]
            Expected = Ok (("", Float 61.6237149387),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 25; Tokeniser.Power; Tokeniser.Float 0.5]
            Expected = Ok (("", Float 5),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Float 4; Tokeniser.Power; Tokeniser.Int -1]
            Expected = Ok (("", Float 0.25),  Dictionary<string, Parser.NumType>())
       }
       {
            // Testing order of operations with exponent
            Args = [Tokeniser.Int 2; Tokeniser.Multiply; Tokeniser.Float 3; Tokeniser.Power; Tokeniser.Int 2]
            Expected = Ok (("", Float 18),  Dictionary<string, Parser.NumType>())
       }
       {
            // Subsequent exponent
            Args = [Tokeniser.Int 4; Tokeniser.Power; Tokeniser.Int 2; Tokeniser.Power; Tokeniser.Int 3]
            Expected = Ok (("", Float 65536),  Dictionary<string, Parser.NumType>())
       }
       {
            // Test for modulo
            Args = [Tokeniser.Int 5; Tokeniser.Modulus; Tokeniser.Int 3]
            Expected = Ok (("", Int 2),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Int 5216; Tokeniser.Modulus; Tokeniser.Int 413]
            Expected = Ok (("", Int 260),  Dictionary<string, Parser.NumType>())
       }
       {
            // Subsequent modulo
            Args = [Tokeniser.Int 763; Tokeniser.Modulus; Tokeniser.Int 129; Tokeniser.Modulus; Tokeniser.Int 20]
            Expected = Ok (("", Int 18),  Dictionary<string, Parser.NumType>())
       }
       {
            // Negative exponent
            Args = [Tokeniser.Int 4; Tokeniser.Power; Tokeniser.Minus; Tokeniser.Int 2]
            Expected = Ok (("", Float 0.0625),  Dictionary<string, Parser.NumType>())
       }
       {
            // Negative modulo
            Args = [Tokeniser.Int 71; Tokeniser.Modulus; Tokeniser.Minus; Tokeniser.Int 15]
            Expected = Ok (("", Int 11),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Minus; Tokeniser.Int 71; Tokeniser.Modulus; Tokeniser.Int 15]
            Expected = Ok (("", Int -11),  Dictionary<string, Parser.NumType>())
       }
       {
            Args = [Tokeniser.Minus; Tokeniser.Int 71; Tokeniser.Modulus; Tokeniser.Minus; Tokeniser.Int 15]
            Expected = Ok (("", Int -11),  Dictionary<string, Parser.NumType>())
       }
       {
            //basic variable assignment with integer
            Args = [Tokeniser.Identifier "x"; Tokeniser.Equals; Tokeniser.Int 3]
            Expected = Ok (("x", Int 3), (Helper.createDictionary "x" (Int 3)))
       }
       {
            //basic variable assignment with float
            Args = [Tokeniser.Identifier "var1"; Tokeniser.Equals; Tokeniser.Float 5]
            Expected = Ok (("var1", Float 5), (Helper.createDictionary "var1" (Float 5)))
       }
       {
            //asignment with float and int
            Args = [Tokeniser.Identifier "var2"; Tokeniser.Equals; Tokeniser.Float 5.5; Tokeniser.Add; Tokeniser.Int 2]
            Expected = Ok (("var2", Float 7.5), (Helper.createDictionary "var2" (Float 7.5)))

       }
       {
            // Negative brackets assignment
            Args = [Tokeniser.Identifier "y"; Tokeniser.Equals; Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Int 6; Tokeniser.Add; Tokeniser.Int 11;
                    Tokeniser.RightBracket]
            Expected = Ok (("y", Int -17), (Helper.createDictionary "y" (Int -17)))

       }
       {
            // Parse multiple expressions
            Args = [Tokeniser.Identifier "x"; Tokeniser.Equals; Tokeniser.Int 5; Tokeniser.EOL; Tokeniser.Identifier "y"; Tokeniser.Equals;
                    Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Identifier "x"]
            Expected = Ok (("y", Int 7), Dictionary<string, Parser.NumType>() )
       }
    ]
    static member parserErrorCases: ParserTestCase list = [
       {
            // No input
            Args = []
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Operators without an operand between
            Args = [Tokeniser.Int 5; Tokeniser.Add; Tokeniser.Add; Tokeniser.Int 2]
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Operator without an operand following
            Args = [Tokeniser.Int 5; Tokeniser.Divide]
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Operator with no operands at all
            Args = [Tokeniser.Multiply]
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Opening a bracket without closing it
            Args = [Tokeniser.LeftBracket; Tokeniser.LeftBracket; Tokeniser.RightBracket]
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Closing a bracket without having opened a matching one
            Args = [Tokeniser.LeftBracket; Tokeniser.RightBracket; Tokeniser.RightBracket]
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            // Division by 0
            Args = [Tokeniser.Int 1; Tokeniser.Divide; Tokeniser.Int 0]
            Expected = Error "Error while parsing: division by 0"
       }
       {
            // As above but with float 0
            Args = [Tokeniser.Int 1; Tokeniser.Divide; Tokeniser.Float 0]
            Expected = Error "Error while parsing: division by 0"
       }
       {
            // Modulo with non-ints
            Args = [Tokeniser.Int 2; Tokeniser.Modulus; Tokeniser.Float 3]
            Expected = Error "Error while parsing: modulo cannot be used with floats"
       }
    ]

    [<TestCaseSource("parserTestCases")>]
    // Check parser test cases
    member this._Test_Parser_Pass(testCase: ParserTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
        let symTable =  Dictionary<string, Parser.NumType>()


        // Act
        let actual = parseEval args symTable

        // Assert correct return type
        match expected with
        | Ok ((vID, Int x), dict) -> match actual with
                                     | Ok ((vID, Int x), dict) -> Assert.IsTrue(true)
                                     | _  -> Assert.IsTrue(false)
        | Ok ((vID, Float x), dict) -> match actual with
                                       | Ok ((vID, Float x), dict) -> Assert.IsTrue(true)
                                       | _ -> Assert.IsTrue(false)
        | _ -> Assert.IsTrue(false)
        // Assert correct value (within tolerance to the 10th decimal place, for floating point errors)
        let actualValue = match actual with
                          | Ok ((vID, x), dict) -> match x with
                                                   | Int y -> float y
                                                   | Float y -> y
                          | _ -> 0
        let expectedValue = match expected with
                            | Ok ((vID, x), dict) -> match x with
                                                     | Int y -> float y
                                                     | Float y -> y
                            | _ -> 0
        Assert.That(actualValue, Is.EqualTo(expectedValue).Within(0.0000000001));
        
    [<TestCaseSource("parserErrorCases")>]
    // Check parser error cases
    member this._Test_Parser_Error(testCase: ParserTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
        let symTable =  Dictionary<string, Parser.NumType>()

        // Act
        let actual = parseEval args symTable

        // Assert
        Assert.AreEqual(actual, expected)