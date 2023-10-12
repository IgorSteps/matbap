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
               Engine.Tokeniser.createToken Engine.Tokeniser.Digit '1';
               Engine.Tokeniser.createToken Engine.Tokeniser.Letter 'a';
               Engine.Tokeniser.createToken Engine.Tokeniser.Operator '+';
               Engine.Tokeniser.createToken Engine.Tokeniser.Digit '1'
               Engine.Tokeniser.createToken Engine.Tokeniser.Digit '2'
            ]
       };
       {
            Args = "1a+(4b)";
            Expected = [
                 Engine.Tokeniser.createToken Engine.Tokeniser.Digit '1';
                 Engine.Tokeniser.createToken Engine.Tokeniser.Letter 'a';
                 Engine.Tokeniser.createToken Engine.Tokeniser.Operator '+';
                 Engine.Tokeniser.createToken Engine.Tokeniser.LeftBracket '(';
                 Engine.Tokeniser.createToken Engine.Tokeniser.Digit '4'
                 Engine.Tokeniser.createToken Engine.Tokeniser.Letter 'b'
                 Engine.Tokeniser.createToken Engine.Tokeniser.RightBracket ')';
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
        | Error errMsg -> failwithf "Unexpected failure: failed to tokenise: %s" errMsg

    [<Test>]
    // Check tokeniser returns error if Unknown token is passed.
    member this._Test_Tokeniser_Error() =
        // --------
        // Assemble
        // --------
        let args = "1&"
        let expectedErrorMsg = "Unknown token: &"
         
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