namespace Engine.Tests

open NUnit.Framework

type TokeniserTestCase = {
    Args: string;
    Expected: Engine.Tokeniser.Token list
}

[<TestFixture>]
type TokeniserTests () =
    static member testCases = [
       {
            Args = "1a+12";
            Expected = [
               Engine.Tokeniser.Int 1;
               Engine.Tokeniser.Variable "a";
               Engine.Tokeniser.Add;
               Engine.Tokeniser.Int 12
            ]
       };
       {
            Args = "1a+(4b)";
            Expected = [
                 Engine.Tokeniser.Int 1;
                 Engine.Tokeniser.Variable "a";
                 Engine.Tokeniser.Add;
                 Engine.Tokeniser.LeftBracket;
                 Engine.Tokeniser.Int 4;
                 Engine.Tokeniser.Variable "b";
                 Engine.Tokeniser.RightBracket;
            ]
       };
       {
            Args = "33.3 / (12 + 3.5a)"
            Expected = [
                Engine.Tokeniser.Float 33.3;
                Engine.Tokeniser.Divide;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 12;
                Engine.Tokeniser.Add;
                Engine.Tokeniser.Float 3.5;
                Engine.Tokeniser.Variable "a"
                Engine.Tokeniser.RightBracket;
            ]
       };
    ]

    [<TestCaseSource("testCases")>]
    // Check tokeniser tokenises correectly.
    member this._Test_Tokeniser_Pass(testCase: TokeniserTestCase) =
        // --------
        // Assemble
        // --------
        let args = testCase.Args
        let expected = testCase.Expected
  
        // ---
        // Act
        // ---
        let actual = Engine.Tokeniser.tokenise args

        // ------
        // Assert
        // ------
        match actual with
        | Ok tokenList -> Assert.AreEqual(expected |> List.toArray, tokenList |> List.toArray)
        | Error errMsg -> failwith "%O" Engine.Tokeniser.InvalidToken

    [<Test>]
    // Check tokeniser returns error if Unknown token is passed.
    member this._Test_Tokeniser_InvalidToken_Error() =
        // --------
        // Assemble
        // --------
        let args = "1&"
        let expectedErrorMsg = Engine.Tokeniser.InvalidToken
         
        // ---
        // Act
        // ---
        let actual = Engine.Tokeniser.tokenise args

        // ------
        // Assert
        // ------
        match actual with
        | Ok _ -> failwithf "Unexpected success"
        | Error errMsg -> Assert.AreEqual(expectedErrorMsg, errMsg)

    [<Test>]
    // Check tokeniser returns error if a flaot is not formatted correctly.
    member this._Test_Tokeniser_InvalidFloat_Error() =
        // --------
        // Assemble
        // --------
        let args = "1. + 43"
        let expectedErrorMsg = Engine.Tokeniser.InvalidFloat

        let args2 = "1.a / 34"
        let expectedErrorMsg2 = Engine.Tokeniser.InvalidFloat

        let args3 = "1.4.2 (3)"
        let expectedErrorMsg3 = Engine.Tokeniser.InvalidFloat
         
        // ---
        // Act
        // ---
        let actual = Engine.Tokeniser.tokenise args
        let actual2 = Engine.Tokeniser.tokenise args2
        let actual3 = Engine.Tokeniser.tokenise args3

        // ------
        // Assert
        // ------
        match actual with
        | Ok _ -> failwithf "Unexpected success"
        | Error errMsg -> Assert.AreEqual(expectedErrorMsg, errMsg)

        match actual2 with
        | Ok _ -> failwithf "Unexpected success"
        | Error errMsg -> Assert.AreEqual(expectedErrorMsg2, errMsg)

        match actual3 with
        | Ok _ -> failwithf "Unexpected success"
        | Error errMsg -> Assert.AreEqual(expectedErrorMsg3, errMsg)