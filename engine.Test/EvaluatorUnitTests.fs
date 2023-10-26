namespace Engine.Tests

open Engine
open NUnit.Framework

type EvaluatorTestCase = {
    Args: string;
    Expected: Result<float,string>;
}

[<TestFixture>]
type EvaluatorTests () =
    static member evaluatorTestCases: EvaluatorTestCase list = [
       // Error tests
       {
            Args = "7+9"
            Expected = Ok 16
       }
       {
            Args = "6-1"
            Expected = Ok 5
       }
       {
            Args = "6*8"
            Expected = Ok 48
       }
       {
            Args = "121/11"
            Expected = Ok 11
       }
       // Error tests
       {
            Args = ""
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "4+-3"
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "2*"
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "/"
            Expected = Error "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "9/0"
            Expected = Error "Error while parsing: division by 0"
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
        Assert.AreEqual(actual, expected)