namespace engine.Tests

open engine
open NUnit.Framework

[<TestFixture>]
type TokeniserTests () =

    [<Test>]
    member this._Test_Tokeniser_Pass() =
        let expected = [
            engine.Tokeniser.CreateToken "Digit" '1';
            engine.Tokeniser.CreateToken "Operator" '+';
            engine.Tokeniser.CreateToken "Digit" '1'
        ]
        let actual = engine.Tokeniser.Execute "1+1"

        Assert.AreEqual(expected |> List.toArray, actual |> List.toArray)
    