namespace Engine.Tests

open NUnit.Framework

[<TestFixture>]
type TokeniserTests () =

    [<Test>]
    member this._Test_Tokeniser_Pass() =
        // --------
        // Assemble
        // --------
        let expected = [
            Engine.Tokeniser.createToken Engine.Tokeniser.Digit '1';
            Engine.Tokeniser.createToken Engine.Tokeniser.Operator '+';
            Engine.Tokeniser.createToken Engine.Tokeniser.Digit '1'
        ]
        let args = "1+1"

        // ---
        // Act
        // ---
        let actual = Engine.Tokeniser.tokenise args

        // ------
        // Assert
        // ------
        match actual with
        | Ok tokenList -> Assert.AreEqual(expected |> List.toArray, tokenList |> List.toArray)
        | Error errMsg -> failwithf "failed to tokenise: %s" errMsg