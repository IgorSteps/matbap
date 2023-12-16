namespace Engine.Tests

open Engine
open Engine.Types
open Engine.ASTParser
open NUnit.Framework
open System.Collections.Generic

type ASTParserTestCase = {
    Name: string
    Args: Tokeniser.Token list;
    Expected: Result<Node, string>
}


[<TestFixture>]
type ASTParserTests() =
    static member AstParserTestCases: ASTParserTestCase list = [
        // Float vs Int difference.
        {
            Name = "Test parser differentiates floats and ints."
            Args = [Tokeniser.Int 2]
            Expected = Ok (Number (Int 2))
        }
        {
            Name = "Test parser differentiates floats and ints."
            Args = [Tokeniser.Float 2.5]
            Expected = Ok (Number (Float 2.5))
        }
        // Simple arithemtic operations.
        {
            Name = "Test simple addition: 1+1."
            Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 1]
            Expected = Ok (BinaryOperation ("+", Number (Int 1), Number (Int 1)))
        }
        {
            Name = "Test simple substraction: 2-1."
            Args = [Tokeniser.Int 2; Tokeniser.Minus; Tokeniser.Int 1]
            Expected = Ok (BinaryOperation ("-", Number (Int 2), Number (Int 1)))
        }
        {
            Name = "Test simple mutliplication: 5*5."
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.Int 5]
            Expected = Ok (BinaryOperation ("*", Number (Int 5), Number (Int 5)))
        }
        {
            Name = "Test simple division: 10/5."
            Args = [Tokeniser.Int 10; Tokeniser.Divide; Tokeniser.Int 5]
            Expected = Ok (BinaryOperation ("/", Number (Int 10), Number (Int 5)))
        }
        // Combined arithemtic operations.
        {
            Name = "Test combined operations: 1+2/3."
            Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.Divide; Tokeniser.Int 3]
            Expected = Ok (BinaryOperation ("+", Number (Int 1), BinaryOperation ("/", Number (Int 2), Number (Int 3))))
        }
        {
            Name = "Test combined operations: 4*2 +3."
            Args = [Tokeniser.Int 4; Tokeniser.Multiply; Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 3]
            Expected = Ok (BinaryOperation ("+", BinaryOperation ("*", Number (Int 4), Number (Int 2)), Number (Int 3)))
        }
        // Parenthesised expressions.
        {
            Name = "Test parenthesised expression: (1+2)*3."
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.RightBracket; Tokeniser.Multiply; Tokeniser.Int 3]
            Expected = Ok (BinaryOperation ("*", ParenthesisExpression (BinaryOperation ("+", Number (Int 1), Number (Int 2))), Number (Int 3)))
        }
        {
            Name = "Test parenthesised expression: 4*(2+3)."
            Args = [Tokeniser.Int 4; Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 3; Tokeniser.RightBracket;]
            Expected = Ok (BinaryOperation ("*", Number (Int 4), ParenthesisExpression (BinaryOperation ("+", Number (Int 2), Number (Int 3)))))
        }
        // Nested parentheses.
        {
            Name = "Test nested parentheses: ((1+2)*3)/4"
            Args = [Tokeniser.LeftBracket; Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add;
                Tokeniser.Int 2; Tokeniser.RightBracket; Tokeniser.Multiply; Tokeniser.Int 3; Tokeniser.RightBracket; Tokeniser.Divide; Tokeniser.Int 4]
            Expected = Ok (BinaryOperation ("/", ParenthesisExpression (BinaryOperation ("*", ParenthesisExpression (BinaryOperation ("+", Number (Int 1), Number (Int 2))), Number (Int 3))), Number (Int 4)))
        }
        // Errors.
        {
            Name = "Testing error: Missing a number or a bracket: 1+."
            Args = [Tokeniser.Int 1; Tokeniser.Add]
            Expected = Error ("Expected number or '('")
        }
        {
            Name = "Testing error: Missing a number or a bracket: +1."
            Args = [Tokeniser.Add; Tokeniser.Int 1]
            Expected = Error ("Expected number or '('")
        }
        {
            Name = "Testing error: Missing a closing bracket: (1+1."
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 1]
            Expected = Error ("Missing closing bracket")
        }

    ]

    [<TestCaseSource("AstParserTestCases")>]
    // Unit test AST Parser.
    member this.Test_ASTParser_HappyPaths(tc: ASTParserTestCase) =
        // --------
        // ASSEMBLE
        // --------
        let args = tc.Args
        let expected = tc.Expected


        // ---
        // ACT
        // ---
        let actual = parse args

        // ------
        // ASSERT
        // ------
        Assert.That(actual, Is.EqualTo(expected))