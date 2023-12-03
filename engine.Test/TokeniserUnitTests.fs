namespace Engine.Tests

open NUnit.Framework
open Engine.Tokeniser

type TokeniserTestCase = {
    Args: string;
    Expected: Engine.Tokeniser.Token list
}

type TokeniserLexicalErrorCase = {
    Args: string;
    Expected: Engine.Tokeniser.LexicalError
}

[<TestFixture>]
type TokeniserTests () =
    static member testCases: TokeniserTestCase list = [
       {
            Args = "1a+12";
            Expected = [
               Engine.Tokeniser.Int 1;
               Engine.Tokeniser.Identifier "a";
               Engine.Tokeniser.Add;
               Engine.Tokeniser.Int 12
            ]
       };
       {
            Args = "1a+(4b)";
            Expected = [
                 Engine.Tokeniser.Int 1;
                 Engine.Tokeniser.Identifier "a";
                 Engine.Tokeniser.Add;
                 Engine.Tokeniser.LeftBracket;
                 Engine.Tokeniser.Int 4;
                 Engine.Tokeniser.Identifier "b";
                 Engine.Tokeniser.RightBracket;
            ]
       };
       {
            Args = "33.3 / (12 + 3.5a)";
            Expected = [
                Engine.Tokeniser.Float 33.3;
                Engine.Tokeniser.Divide;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 12;
                Engine.Tokeniser.Add;
                Engine.Tokeniser.Float 3.5;
                Engine.Tokeniser.Identifier "a"
                Engine.Tokeniser.RightBracket;
            ]
       };
       {
            Args = "sin(43)/cos(1.5) * tan(34)";
            Expected = [
                Engine.Tokeniser.Sin;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 43;
                Engine.Tokeniser.RightBracket;
                Engine.Tokeniser.Divide;
                Engine.Tokeniser.Cos;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Float 1.5;
                Engine.Tokeniser.RightBracket;
                Engine.Tokeniser.Multiply;
                Engine.Tokeniser.Tan;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 34;
                Engine.Tokeniser.RightBracket;
            ]
       };
       {
            Args = "5 +            10.5";
            Expected = [
                Engine.Tokeniser.Int 5;
                Engine.Tokeniser.Add;
                Engine.Tokeniser.Float 10.5;
            ]
       };
       {
            Args = "var1 + var2 = 4";
            Expected = [
                Engine.Tokeniser.Identifier "var1";
                Engine.Tokeniser.Add;
                Engine.Tokeniser.Identifier "var2";
                Engine.Tokeniser.Equals;
                Engine.Tokeniser.Int 4;
            ]
       };
       {
            Args = "4 % 2";
            Expected = [
                Engine.Tokeniser.Int 4;
                Engine.Tokeniser.Modulus;
                Engine.Tokeniser.Int 2
            ]
       };
       {
            Args = "4^3 / tan(44)";
            Expected = [
                Engine.Tokeniser.Int 4;
                Engine.Tokeniser.Power;
                Engine.Tokeniser.Int 3;
                Engine.Tokeniser.Divide;
                Engine.Tokeniser.Tan;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 44;
                Engine.Tokeniser.RightBracket;
            ]
       };
       {
            Args = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            Expected = [
                Engine.Tokeniser.Identifier "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            ]
       };
       {
            Args = "4 3.21 + a";
            Expected = [
                Engine.Tokeniser.Int 4;
                Engine.Tokeniser.Float 3.21;
                Engine.Tokeniser.Add;
                Engine.Tokeniser.Identifier "a";
            ]
       };
       {
            Args = "6(4(2(1(0))))";
            Expected = [
                Engine.Tokeniser.Int 6;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 4;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 2;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 1;
                Engine.Tokeniser.LeftBracket;
                Engine.Tokeniser.Int 0;
                Engine.Tokeniser.RightBracket;
                Engine.Tokeniser.RightBracket;
                Engine.Tokeniser.RightBracket;
                Engine.Tokeniser.RightBracket;
            ]
       };
       {
            //test delimeter and correct order
            Args = "x = 5; \n\r y = x + 3";
            Expected = [
                    Token.Identifier "x";
                    Token.Equals;
                    Token.Int 5;
                    Token.EOL;
                    Token.Identifier "y";
                    Token.Equals;
                    Token.Identifier "x";
                    Token.Add;
                    Token.Int 3
            ]
       }
    ]

    static member lexicalErrorTestCases: TokeniserLexicalErrorCase list = [
        {
            Args = "1. + 43";
            Expected = Engine.Tokeniser.InvalidFloat ("Invalid Float at token position 1: the mantissa "+
                                                       "cannot lead with non digit")
        };
        {
            Args = "123 * 4 / 1.a";
            Expected = Engine.Tokeniser.InvalidFloat ("Invalid Float at token position 5: the mantissa "+
                                                       "cannot lead with non digit")
        };
        {
            Args = "1.4.2 (3)";
            Expected = Engine.Tokeniser.InvalidFloat ("Invalid Float at token position 1: " +
                                                     "Can't have 2 decimal places in a float")
        };
        {
            Args = "1&";
            Expected = Engine.Tokeniser.InvalidToken "Invalid Token at token position 2: &"
        };
        {
            Args = "4*\3";
            Expected = Engine.Tokeniser.InvalidToken "Invalid Token at token position 3: \\"
        };
        {
            Args = ".6 + 34.5"
            Expected = Engine.Tokeniser.InvalidToken "Invalid Token at token position 1: ."
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
        | Error err -> match err with
                       | Engine.Tokeniser.InvalidToken msg -> failwith msg
                       | Engine.Tokeniser.InvalidFloat msg -> failwith msg

    [<TestCaseSource("lexicalErrorTestCases")>]
    // Check tokeniser returns error if a float is not formatted correctly.
    member this._Test_Tokeniser_LexicalError(testCase: TokeniserLexicalErrorCase) =
        // --------
        // Assemble
        // --------
        let args = testCase.Args
        let expectedError = testCase.Expected
         
        // ---
        // Act
        // ---
        let actual = Engine.Tokeniser.tokenise args

        // ------
        // Assert
        // ------
        match actual with
        | Ok _ -> failwithf "Unexpected success"
        | Error err -> Assert.AreEqual(expectedError, err)