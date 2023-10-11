namespace engine.Tests

open engine
open NUnit.Framework

[<TestFixture>]
type TokeniserTests () =

    [<Test>]
    member this._Test_Tokeniser_Pass() =
        let expected = [
            engine.Tokeniser.createToken "Digit" '1';
            engine.Tokeniser.createToken "Operator" '+';
            engine.Tokeniser.createToken "Digit" '1'
        ]
        let actual = engine.Tokeniser.tokenise "1+1"

        Assert.AreEqual(expected |> List.toArray, actual |> List.toArray)
    