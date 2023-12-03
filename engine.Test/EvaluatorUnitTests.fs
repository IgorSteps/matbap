namespace Engine.Tests

open Engine
open NUnit.Framework
open System.Collections.Generic

type EvaluatorTestCase = {
    Args: string;
    Expected: Result<string * Dictionary<string, Parser.NumType>,string>;
}
type PlotTestCase = {
    Min: float;
    Max: float;
    Step: float;
    Exp: string;
    Expected: Result<float array array, string>;
}

[<TestFixture>]
[<DefaultFloatingPointTolerance(0.0000000001)>]
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
    
    static member plotTestCases: PlotTestCase list = [
        // Plot test cases.
        // Some of these can be rewritten once implicit multiplication is allowed
        {
            Min = 0; Max = 5; Step = 0.1; Exp = "(x^2)+(4*x)+4";
            Expected = Ok [|[|0.0; 4.0|]; [|0.1; 4.41|]; [|0.2; 4.84|]; [|0.3; 5.29|];
            [|0.4; 5.76|]; [|0.5; 6.25|]; [|0.6; 6.76|]; [|0.7; 7.29|]; [|0.8; 7.84|];
            [|0.9; 8.41|]; [|1.0; 9.0|]; [|1.1; 9.61|]; [|1.2; 10.24|]; [|1.3; 10.89|];
            [|1.4; 11.56|]; [|1.5; 12.25|]; [|1.6; 12.96|]; [|1.7; 13.69|]; [|1.8; 14.44|];
            [|1.9; 15.21|]; [|2.0; 16.0|]; [|2.1; 16.81|]; [|2.2; 17.64|]; [|2.3; 18.49|];
            [|2.4; 19.36|]; [|2.5; 20.25|]; [|2.6; 21.16|]; [|2.7; 22.09|]; [|2.8; 23.04|];
            [|2.9; 24.01|]; [|3.0; 25.0|]; [|3.1; 26.01|]; [|3.2; 27.04|]; [|3.3; 28.09|];
            [|3.4; 29.16|]; [|3.5; 30.25|]; [|3.6; 31.36|]; [|3.7; 32.49|]; [|3.8; 33.64|];
            [|3.9; 34.81|]; [|4.0; 36.0|]; [|4.1; 37.21|]; [|4.2; 38.44|]; [|4.3; 39.69|];
            [|4.4; 40.96|]; [|4.5; 42.25|]; [|4.6; 43.56|]; [|4.7; 44.89|]; [|4.8; 46.24|];
            [|4.9; 47.61|]; [|5.0; 49.0|]|]
        }
        {
            Min = -4; Max = 9; Step = 1; Exp = "2*x+6";
            Expected = Ok [|[|-4.0; -2.0|]; [|-3.0; 0.0|]; [|-2.0; 2.0|]; [|-1.0; 4.0|];
            [|0.0; 6.0|]; [|1.0; 8.0|]; [|2.0; 10.0|]; [|3.0; 12.0|]; [|4.0; 14.0|];
            [|5.0; 16.0|]; [|6.0; 18.0|]; [|7.0; 20.0|]; [|8.0; 22.0|]; [|9.0; 24.0|]|]
        }
        {
            Min = -0.5; Max = 2; Step = 0.7; Exp = "x+2";
            Expected = Ok [|[|-0.5; 1.5|]; [|0.2; 2.2|]; [|0.9; 2.9|]; [|1.6; 3.6|]|]
        }
        {
            Min = -0.5; Max = 7.5; Step = 2; Exp = "x/2";
            Expected = Ok [|[|-0.5; -0.25|]; [|1.5; 0.75|]; [|3.5; 1.75|]; [|5.5; 2.75|]; [|7.5; 3.75|]|]
        }
        {
            Min = 0; Max = 3; Step = 0.25; Exp = "2-x^2";
            Expected = Ok [|[|0.0; 2.0|]; [|0.25; 1.9375|]; [|0.5; 1.75|]; [|0.75; 1.4375|];
            [|1.0; 1.0|]; [|1.25; 0.4375|]; [|1.5; -0.25|]; [|1.75; -1.0625|]; [|2.0; -2.0|];
            [|2.25; -3.0625|]; [|2.5; -4.25|]; [|2.75; -5.5625|]; [|3.0; -7.0|]|]
        }
        {
            Min = -2; Max = 2; Step = 0.25; Exp = "x^3";
            Expected = Ok [|[|-2.0; -8.0|]; [|-1.75; -5.359375|]; [|-1.5; -3.375|];
            [|-1.25; -1.953125|]; [|-1.0; -1.0|]; [|-0.75; -0.421875|]; [|-0.5; -0.125|];
            [|-0.25; -0.015625|]; [|0.0; 0.0|]; [|0.25; 0.015625|]; [|0.5; 0.125|];
            [|0.75; 0.421875|]; [|1.0; 1.0|]; [|1.25; 1.953125|]; [|1.5; 3.375|];
            [|1.75; 5.359375|]; [|2.0; 8.0|]|]
        }
    ]

    [<TestCaseSource("evaluatorTestCases")>]
    // Check evaluator test cases
    member this._Test_Evaluator_Pass(testCase: EvaluatorTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
        let symTable = Dictionary<string, Parser.NumType>()
  
        // Act
        let actual = Evaluator.eval args symTable

        // Assert
        match expected, actual with
        | Ok(expectedValue, _), Ok(actualValue, _) ->
            Assert.AreEqual(expectedValue, actualValue, "Values are not equal")

        | Error expectedError, Error actualError ->
            Assert.AreEqual(expectedError, actualError, "Errors are not equal")
        | _ ->
            Assert.Fail("Expected and actual have different result types")
            
    [<TestCaseSource("plotTestCases")>]
    // Check evaluator test cases
    member this._Test_Plot_Pass(testCase: PlotTestCase) =
        // Assemble
        let expected = testCase.Expected
  
        // Act
        let actual = Evaluator.plotPoints testCase.Min testCase.Max testCase.Step testCase.Exp

        // Assert
        match expected, actual with
        | Ok(expectedPoints), Ok(actualPoints) ->
            Assert.AreEqual(expectedPoints, actualPoints, "Points are not equal")

        | Error expectedError, Error actualError ->
            Assert.AreEqual(expectedError, actualError, "Errors are not equal")
        | _ ->
            Assert.Fail("Expected and actual have different result types")