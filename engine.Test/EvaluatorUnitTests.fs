namespace Engine.Tests

open Engine
open NUnit.Framework
open System.Collections.Generic

type EvaluatorTestCase = {
    Args: string;
    Expected: Result<string * Dictionary<string, Parser.NumType>,string>;
}

[<TestFixture>]
type EvaluatorTests () =
    static member evaluatorTestCases: EvaluatorTestCase list = [
        // Basic tests (parser's tests should test its own function)
        {
            Args = "7+9"
            Expected = Ok("16", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "6-1"
            Expected = Ok("5", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "6*8"
            Expected = Ok("48", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "121/11"
            Expected = Ok("11", Dictionary<string, Parser.NumType>())
        }
        // Unary Minus
        {
            Args = "4+-3"
            Expected = Ok("1", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-2 + -3"
            Expected = Ok("-5", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-2 - (-3)"
            Expected = Ok("1", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-(2+3)"
            Expected = Ok("-5", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-(2 + (-3))"
            Expected = Ok("1", Dictionary<string, Parser.NumType>())
        }
        // Power
        {
            Args = "2^2"
            Expected = Ok("4", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "2^(2+2)"
            Expected = Ok("16", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "2^-2"
            Expected = Ok("0.25", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "2^-(2 + (-3))"
            Expected = Ok("2", Dictionary<string, Parser.NumType>())
        }
        // Modulo
        {
            Args = "10 % 3"
            Expected = Ok("1", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "10 % -3"
            Expected = Ok("1", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-10 % 3"
            Expected = Ok("-1", Dictionary<string, Parser.NumType>())
        }
        {
            Args = "-10 % -3"
            Expected = Ok("-1", Dictionary<string, Parser.NumType>())
        }
        // Error tests
        {
            Args = ""
            Expected =  Error("Error while parsing: Unexpected token or end of expression")
        }
        {
            Args = "2*"
            Expected =  Error("Error while parsing: Unexpected token or end of expression")
        }
        {
            Args = "/"
            Expected =  Error("Error while parsing: Unexpected token or end of expression")
        }
        {
            Args = "9/0"
            Expected =  Error("Error while parsing: division by 0")
        }
    ]

    [<TestCaseSource("evaluatorTestCases")>]
    // Check evaluator test cases
    member this._Test_Evaluator_Pass(testCase: EvaluatorTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
  
        // Act
        let actual = Evaluator.eval args

        // Assert
        match expected, actual with
        | Ok (expectedValue, _), Ok (actualValue, _) ->
            Assert.AreEqual(expectedValue, actualValue, "Values are not equal")

        | Error expectedError, Error actualError ->
            Assert.AreEqual(expectedError, actualError, "Errors are not equal")
        | _ ->
            Assert.Fail("Expected and actual have different result types")