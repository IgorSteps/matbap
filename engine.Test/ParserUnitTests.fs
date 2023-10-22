namespace Engine.Tests

open Engine
open NUnit.Framework
open System

type ParserTestCase = {
    Args: Tokeniser.Token list;
    Expected: Result<float,string>;
}

[<TestFixture>]
type ParserTests () =
    static member parserTestCases: ParserTestCase list = [
       {
            Args = [Tokeniser.Int(2); Tokeniser.Add; Tokeniser.Int(10);]
            Expected = Ok 12
       }
       {
            Args = [Tokeniser.Int(7); Tokeniser.Minus; Tokeniser.Int(3)]
            Expected = Ok 4
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Multiply; Tokeniser.Int(7)]
            Expected = Ok 35
       }
       {
            Args = [Tokeniser.Int(12); Tokeniser.Divide; Tokeniser.Int(8)]
            Expected = Ok 1.5
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Minus; Tokeniser.Float(2.3)]
            Expected = Ok 2.7
       }
       {
            Args = [Tokeniser.Float(4.62); Tokeniser.Multiply; Tokeniser.Int(9)]
            Expected = Ok 41.58
       }
       {
            Args = [Tokeniser.Int(49); Tokeniser.Divide; Tokeniser.Float(7.0)]
            Expected = Ok 7
       }
       {
            Args = [Tokeniser.Float(11.53); Tokeniser.Minus; Tokeniser.Int(23); Tokeniser.Add; Tokeniser.Int(612)]
            Expected = Ok 600.53
       }
       {
            Args = [Tokeniser.Float(13.56); Tokeniser.Minus; Tokeniser.Int(6); Tokeniser.Add; Tokeniser.Int(14)
                    Tokeniser.Multiply; Tokeniser.Float(20.1)]
            Expected = Ok 288.96
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Int(3); Tokeniser.Add
                    Tokeniser.Float(0.5); Tokeniser.RightBracket]
            Expected = Ok 17.5
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Minus; Tokeniser.Float(2.5); Tokeniser.Divide; Tokeniser.LeftBracket
                    Tokeniser.Float(6.0); Tokeniser.Add; Tokeniser.Float(6.5); Tokeniser.RightBracket; Tokeniser.Add
                    Tokeniser.Int(1)]
            Expected = Ok 5.8
       }
       {
            Args = [Tokeniser.Float(2.5); Tokeniser.Add; Tokeniser.LeftBracket; Tokeniser.LeftBracket
                    Tokeniser.Float(2.5); Tokeniser.RightBracket; Tokeniser.RightBracket; Tokeniser.Multiply
                    Tokeniser.Int(3)]
            Expected= Ok 10
       }
       {
            Args = [Tokeniser.Int(9999)]
            Expected = Ok 9999
       }
       {
            Args = [Tokeniser.LeftBracket; Tokeniser.Int(0); Tokeniser.RightBracket]
            Expected = Ok 0
       }
       
       {
            Args = []
            Expected = Error "Error parsing expression."
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Add; Tokeniser.Minus; Tokeniser.Int(2);]
            Expected = Error "Error parsing expression."
       }
       {
            Args = [Tokeniser.Int(5); Tokeniser.Divide;]
            Expected = Error "Error parsing expression."
       }
       {
            Args = [Tokeniser.Multiply;]
            Expected = Error "Error parsing expression."
       }
       {
            Args = [Tokeniser.LeftBracket; Tokeniser.LeftBracket; Tokeniser.RightBracket;]
            Expected = Error "Error parsing expression."
       }
       {
            Args = [Tokeniser.LeftBracket; Tokeniser.RightBracket; Tokeniser.RightBracket;]
            Expected = Error "Error parsing expression."
       }
    ]

    [<TestCaseSource("parserTestCases")>]
    // Check parser test cases
    member this._Test_Parser_Pass(testCase: ParserTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
  
        // Act
        let actual = Parser.parseEval args
        Console.WriteLine(actual)

        // Assert
        Assert.AreEqual(actual, expected)