namespace Engine.Tests

open Engine
open NUnit.Framework

type EvaluatorTestCase = {
    Args: string;
    Expected: string;
}

[<TestFixture>]
type EvaluatorTests () =
    static member evaluatorTestCases: EvaluatorTestCase list = [
       // Basic tests (parser's tests should test its own function)
       {
            Args = "7+9"
            Expected = "16"
       }
       {
            Args = "6-1"
            Expected = "5"
       }
       {
            Args = "6*8"
            Expected = "48"
       }
       {
            Args = "121/11"
            Expected = "11"
       }
       {
            Args = "4+-3"
            Expected = "1"
       }
       // Error tests
       {
            Args = ""
            Expected =  "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "2*"
            Expected =  "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "/"
            Expected =  "Error while parsing: Unexpected token or end of expression"
       }
       {
            Args = "9/0"
            Expected =  "Error while parsing: division by 0"
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