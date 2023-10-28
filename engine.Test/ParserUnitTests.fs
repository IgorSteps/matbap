namespace Engine.Tests

open Engine
open Engine.Parser
open NUnit.Framework

type ParserTestCase = {
    Args: Tokeniser.Token list;
    Expected: Result<NumType,string>;
}

[<TestFixture>]
type ParserTests () =
    static member parserTestCases: ParserTestCase list = [
       {
            // Basic addition
            Args = [Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 10]
            Expected = Ok (Int 12)
       }
       {
            // Basic subtraction
            Args = [Tokeniser.Int 7; Tokeniser.Minus; Tokeniser.Int 3]
            Expected = Ok (Int 4)
       }
       {
            // Basic multiplication
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.Int 7]
            Expected = Ok (Int 7)
       }
       {
            // Basic division, with integer truncation
            Args = [Tokeniser.Int 12; Tokeniser.Divide; Tokeniser.Int 8]
            Expected = Ok (Int 1)
       }
       {
            // Using integers with floats
            Args = [Tokeniser.Int 5; Tokeniser.Add; Tokeniser.Float 2.3]
            Expected = Ok (Float 7.3)
       }
       {
            // As above
            Args = [Tokeniser.Int 5; Tokeniser.Minus; Tokeniser.Float 2.3]
            Expected = Ok (Float 2.7)
       }
       {
            // As above but for a different grammar
            Args = [Tokeniser.Float 4.62; Tokeniser.Multiply; Tokeniser.Int 9]
            Expected = Ok (Float 41.58)
       }
       {
            // As above
            Args = [Tokeniser.Int 49; Tokeniser.Divide; Tokeniser.Float 7]
            Expected = Ok (Float 7)
       }
       {
            // Using multiple operations together
            Args = [Tokeniser.Float 11.53; Tokeniser.Minus; Tokeniser.Int 23; Tokeniser.Add; Tokeniser.Int 612]
            Expected = Ok (Float 600.53)
       }
       {
            // Testing proper order of operations
            Args = [Tokeniser.Float 13.56; Tokeniser.Minus; Tokeniser.Int 6; Tokeniser.Add; Tokeniser.Int 14;
                    Tokeniser.Multiply; Tokeniser.Float 20.1]
            Expected = Ok (Float 288.96)
       }
       {
            // Using brackets with an expression inside
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Int 3; Tokeniser.Add;
                    Tokeniser.Float 0.5; Tokeniser.RightBracket]
            Expected = Ok (Float 17.5)
       }
       {
            // Order of operations using brackets, and operations after brackets
            Args = [Tokeniser.Int 5; Tokeniser.Minus; Tokeniser.Float 2.5; Tokeniser.Divide; Tokeniser.LeftBracket;
                    Tokeniser.Float 6; Tokeniser.Add; Tokeniser.Float 6.5; Tokeniser.RightBracket; Tokeniser.Add;
                    Tokeniser.Int(1)]
            Expected = Ok (Float 5.8)
       }
       {
            // As above
            Args = [Tokeniser.Float 2.5; Tokeniser.Add; Tokeniser.LeftBracket; Tokeniser.LeftBracket;
                    Tokeniser.Float 2.5; Tokeniser.RightBracket; Tokeniser.RightBracket; Tokeniser.Multiply;
                    Tokeniser.Int 3]
            Expected= Ok (Float 10)
       }
       {
            // Return a value on its own without operations
            Args = [Tokeniser.Int 9999]
            Expected = Ok (Int 9999)
       }
       {
            // As above, but inside brackets
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 0; Tokeniser.RightBracket]
            Expected = Ok (Int 0)
       }
       {
            // Negative addition
            Args = [Tokeniser.Int 3; Tokeniser.Add; Tokeniser.Int -2]
            Expected = Ok (Int 1)
       }
       {
            // Negative subtraction
            Args = [Tokeniser.Int 4; Tokeniser.Minus; Tokeniser.Int -2]
            Expected = Ok (Int 6)
       }
       {
            // Negative multiplication
            Args = [Tokeniser.Int -3; Tokeniser.Multiply; Tokeniser.Int -9]
            Expected = Ok (Int 27)
       }
       {
            // As above
            Args = [Tokeniser.Int 6; Tokeniser.Multiply; Tokeniser.Float -10.5]
            Expected = Ok (Float -63)
       }
       {
            // Negative division
            Args = [Tokeniser.Int 8; Tokeniser.Divide; Tokeniser.Int -2]
            Expected = Ok (Int -4)
       }
       {
            // As above
            Args = [Tokeniser.Int -320; Tokeniser.Divide; Tokeniser.Int -64]
            Expected = Ok (Int 5)
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
            Args = [Tokeniser.Int 5; Tokeniser.Add; Tokeniser.Minus; Tokeniser.Int 2]
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
    ]

    [<TestCaseSource("parserTestCases")>]
    // Check parser test cases
    member this._Test_Parser_Pass(testCase: ParserTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected

        // Act
        let actual = parseEval args

        // Assert correct return type
        Assert.AreEqual(actual.GetType, expected.GetType)
        // Assert correct value (within tolerance to the 10th decimal place, for floating point errors)
        let actualValue = match actual with
                          | Ok x -> match x with
                                    | Int y -> float y
                                    | Float y -> y
                          | _ -> 0
        let expectedValue = match actual with
                            | Ok x -> match x with
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
  
        // Act
        let actual = parseEval args

        // Assert
        Assert.AreEqual(actual, expected)