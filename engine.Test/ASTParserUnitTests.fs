namespace Engine.Tests

open Engine
open Engine.Types
open Engine.ASTParser
open NUnit.Framework
open System.Collections.Generic

type ASTParserTestCase = {
    Name: string
    Args: Tokeniser.Token list;
    Expected: Node
}

type ASTParserErrorTestCase = {
    Name: string
    Args: Tokeniser.Token list;
    Expected: string
}

[<TestFixture>]
type ASTParserTests() =
    static member AstParserTestCases: ASTParserTestCase list = [
        // Float vs Int difference.
        {
            Name = "Test parser differentiates floats and ints."
            Args = [Tokeniser.Int 2]
            Expected = Number (Int 2)
        }
        {
            Name = "Test parser differentiates floats and ints."
            Args = [Tokeniser.Float 2.5]
            Expected = Number (Float 2.5)
        }
        // Simple arithemtic operations.
        {
            Name = "Test simple addition: 1+1."
            Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 1]
            Expected = BinaryOperation ("+", Number (Int 1), Number (Int 1))
        }
        {
            Name = "Test simple substraction: 2-1."
            Args = [Tokeniser.Int 2; Tokeniser.Minus; Tokeniser.Int 1]
            Expected = BinaryOperation ("-", Number (Int 2), Number (Int 1))
        }
        {
            Name = "Test simple mutliplication: 5*5."
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.Int 5]
            Expected = BinaryOperation ("*", Number (Int 5), Number (Int 5))
        }
        {
            Name = "Test simple division: 10/5."
            Args = [Tokeniser.Int 10; Tokeniser.Divide; Tokeniser.Int 5]
            Expected = BinaryOperation ("/", Number (Int 10), Number (Int 5))
        }
        // Combined arithemtic operations.
        {
            Name = "Test combined operations: 1+2/3."
            Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.Divide; Tokeniser.Int 3]
            Expected = BinaryOperation ("+", Number (Int 1), BinaryOperation ("/", Number (Int 2), Number (Int 3)))
        }
        {
            Name = "Test combined operations: 4*2 +3."
            Args = [Tokeniser.Int 4; Tokeniser.Multiply; Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 3]
            Expected = BinaryOperation ("+", BinaryOperation ("*", Number (Int 4), Number (Int 2)), Number (Int 3))
        }
        // Parenthesised expressions.
        {
            Name = "Test parenthesised expression: (1+2)*3."
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.RightBracket; Tokeniser.Multiply; Tokeniser.Int 3]
            Expected = BinaryOperation ("*", ParenthesisExpression (BinaryOperation ("+", Number (Int 1), Number (Int 2))), Number (Int 3))
        }
        {
            Name = "Test parenthesised expression: 4*(2+3)."
            Args = [Tokeniser.Int 4; Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Int 2; Tokeniser.Add; Tokeniser.Int 3; Tokeniser.RightBracket;]
            Expected = BinaryOperation ("*", Number (Int 4), ParenthesisExpression (BinaryOperation ("+", Number (Int 2), Number (Int 3))))
        }
        // Nested parentheses.
        {
            Name = "Test nested parentheses: ((1+2)*3)/4"
            Args = [Tokeniser.LeftBracket; Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add;
                Tokeniser.Int 2; Tokeniser.RightBracket; Tokeniser.Multiply; Tokeniser.Int 3; Tokeniser.RightBracket; Tokeniser.Divide; Tokeniser.Int 4]
            Expected = BinaryOperation(
                            "/",
                            ParenthesisExpression(
                                BinaryOperation(
                                    "*",
                                    ParenthesisExpression(
                                        BinaryOperation(
                                            "+",
                                            Number(Int 1),
                                            Number (Int 2)
                                        )
                                    ),
                                    Number(Int 3)
                                )
                            ),
                            Number(Int 4)
                    )
        }
        // Power.
        {
            Name = "Test subsequent power is right-associative: 1^2^3 should eval as 1^(2^3)."
            Args = [Tokeniser.Int 1; Tokeniser.Power; Tokeniser.Int 2; Tokeniser.Power; Tokeniser.Int 3]
            Expected = BinaryOperation("^", Number(NumType.Int(1)), BinaryOperation("^", Number(NumType.Int(2)), Number(NumType.Int(3))))
        }
        {
            Name = "Test power to nested expression: 2^(3+4)."
            Args = [Tokeniser.Int 2; Tokeniser.Power; Tokeniser.LeftBracket; Tokeniser.Int 3; Tokeniser.Add; Tokeniser.Int 4; Tokeniser.RightBracket]
            Expected = BinaryOperation(
                            "^",
                            Number(Int 2),
                            ParenthesisExpression(
                                BinaryOperation(
                                    "+",
                                    Number(Int 3),
                                    Number(Int 4)
                                )
                            )
                        )
        }
        {
                   Name = "Test power in complex expression: 1+2*3^4/5."
                   Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.Multiply; Tokeniser.Int 3; Tokeniser.Power; Tokeniser.Int 4; Tokeniser.Divide; Tokeniser.Int 5]
                   Expected = BinaryOperation(
                                   "+",
                                   Number(Int 1),
                                   BinaryOperation(
                                       "/",
                                       BinaryOperation(
                                           "*",
                                           Number(Int 2),
                                           BinaryOperation(
                                               "^",
                                               Number(Int 3),
                                               Number(Int 4)
                                           )
                                       ),
                                       Number(Int 5)
                                   )
                               )
               }
        // Modulo.
        {
            Name = "Test modulo: 10%3."
            Args = [Tokeniser.Int 10; Tokeniser.Modulus; Tokeniser.Int 3]
            Expected = BinaryOperation("%", Number(NumType.Int(10)), Number(NumType.Int(3)))
        }
        // Mixed arithemitcs.
        {
            Name = "(4 - 5) ^ 6"
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 4; Tokeniser.Minus; Tokeniser.Int 5; Tokeniser.RightBracket; Tokeniser.Power; Tokeniser.Int 6]
            Expected = BinaryOperation(
                            "^",
                            ParenthesisExpression(
                                BinaryOperation(
                                    "-",
                                    Number(NumType.Int(4)),
                                    Number(NumType.Int(5))
                                )
                            ),
                            Number(NumType.Int(6))
                        )
        }
        {
                   Name = "2 * 3 / (4 - 5) ^ 2"
                   Args = [Tokeniser.Int 2; Tokeniser.Multiply; Tokeniser.Int 3; Tokeniser.Divide; Tokeniser.LeftBracket; Tokeniser.Int 4 ; Tokeniser.Minus; Tokeniser.Int 5; Tokeniser.RightBracket; Tokeniser.Power; Tokeniser.Int 2]
                   Expected = BinaryOperation(
                                   "/",
                                   BinaryOperation(
                                       "*",
                                       Number (NumType.Int 2),
                                       Number (NumType.Int 3)
                                   ),
                                   BinaryOperation(
                                       "^",
                                       ParenthesisExpression(
                                           BinaryOperation(
                                               "-",
                                               Number (NumType.Int 4),
                                               Number (NumType.Int 5)
                                           )
                                       ),
                                       Number (NumType.Int 2)
                                   )
                               )
        }
        {
            Name = "Test multiple operations with precedence: 1+2*3/(4-5)^6%7"
            Args = [Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 2; Tokeniser.Multiply; Tokeniser.Int 3; Tokeniser.Divide; Tokeniser.LeftBracket; Tokeniser.Int 4; Tokeniser.Minus; Tokeniser.Int 5; Tokeniser.RightBracket; Tokeniser.Power; Tokeniser.Int 6; Tokeniser.Modulus; Tokeniser.Int 7]
            Expected = BinaryOperation(
                            "+",
                            Number(NumType.Int(1)),
                            BinaryOperation(
                                "%",
                                BinaryOperation(
                                    "/",
                                    BinaryOperation(
                                        "*",
                                        Number(NumType.Int(2)),
                                        Number(NumType.Int(3))
                                    ),
                                    BinaryOperation(
                                        "^",
                                        ParenthesisExpression(
                                            BinaryOperation(
                                                "-",
                                                Number(NumType.Int(4)),
                                                Number(NumType.Int(5))
                                            )
                                        ),
                                        Number(NumType.Int(6))
                                    )
                                ),
                                Number(NumType.Int(7))
                            )
                        )
        }
        // Unary minus.
        {
            Name = "Testing unary minus: -1"
            Args = [Tokeniser.Minus; Tokeniser.Int(1);]
            Expected = UnaryMinusOperation("-", Number(NumType.Int(1)))
        }
        {
            Name = "Testing unary minus expression with unary minus: -(-1)"
            Args = [Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Minus; Tokeniser.Int(1); Tokeniser.RightBracket]
            Expected = UnaryMinusOperation(
                            "-",
                            ParenthesisExpression(  
                                UnaryMinusOperation(
                                    "-",
                                    Number(NumType.Int(1))
                                )
                            )
                       )
        }
        {
            Name = "Testing unary minus expression with unary minus: -(-(1 + 1))"
            Args = [Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Minus; Tokeniser.LeftBracket; Tokeniser.Int(1); Tokeniser.Add; Tokeniser.Int(1);Tokeniser.RightBracket; Tokeniser.RightBracket]
            Expected = UnaryMinusOperation(
                            "-",
                            ParenthesisExpression(  
                                UnaryMinusOperation(
                                    "-",
                                    ParenthesisExpression(
                                        BinaryOperation(
                                            "+",
                                            Number(NumType.Int(1)),
                                            Number(NumType.Int(1))
                                        )
                                    )
                                )
                            )
                       )
        }
        {
            Name = "Testinh unary minus in a power: 1^-1"
            Args = [Tokeniser.Int(1); Tokeniser.Power; Tokeniser.Minus; Tokeniser.Int(1)]
            Expected = BinaryOperation(
                            "^",
                            Number(NumType.Int(1)),
                            UnaryMinusOperation(
                                "-",
                                Number(NumType.Int(1))
                            )
                        )
        }
        // Variable Assignment
        {
            Name = "Test variable assignment: x = 1/2"
            Args = [Tokeniser.Identifier "x"; Tokeniser.Equals; Tokeniser.Int 1; Tokeniser.Divide; Tokeniser.Int 2]
            Expected = VariableAssignment(
                           "x",
                           BinaryOperation(
                               "/",
                               Number(Int(1)),
                               Number(Int(2))
                           )
                       )
        }
        // Variables
        {
            Name = "Test variables are parsed: 5*(x+1)"
            Args = [Tokeniser.Int 5; Tokeniser.Multiply; Tokeniser.LeftBracket; Tokeniser.Identifier "x"; Tokeniser.Add; Tokeniser.Int 1; Tokeniser.RightBracket]
            Expected = BinaryOperation(
                            "*",
                            Number(Int 5),
                            ParenthesisExpression(
                                BinaryOperation("+", Variable "x", Number(Int 1))
                            )
                        )
        }

    ]

    static member AstParserErrorTestCases: ASTParserErrorTestCase list = [
        {
            Name = "Testing error: Missing a number or a bracket: 1+."
            Args = [Tokeniser.Int 1; Tokeniser.Add]
            Expected = "Expected number, '(' or '-'."
        }
        {
            Name = "Testing error: Missing a number or a bracket: +1."
            Args = [Tokeniser.Add; Tokeniser.Int 1]
            Expected = "Expected number, '(' or '-'."
        }
        {
            Name = "Testing error: Missing a closing bracket: (1+1."
            Args = [Tokeniser.LeftBracket; Tokeniser.Int 1; Tokeniser.Add; Tokeniser.Int 1]
            Expected = "Missing closing bracket"
        }
        {
            Name = "Testing error: variable assignment without variable name: = 5"
            Args = [Tokeniser.Equals; Tokeniser.Int 5]
            Expected = "A variable assignment was attempted without giving a variable name"
        }
        {
            Name = "Testing error: variable assignment without assigning a value: x = "
            Args = [Tokeniser.Identifier "x"; Tokeniser.Equals]
            Expected = "A variable assignment was attempted without assigning a value"
        }
    ]

    [<TestCaseSource("AstParserTestCases")>]
    /// Unit test AST Parser for happy paths.
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
        match actual with
               | Ok ast -> Assert.AreEqual(expected, ast)
               | Error err -> Assert.Fail("Parsing failed with unexpected error: " + err)


    [<TestCaseSource("AstParserErrorTestCases")>]
    /// Unit test AST Parser for unhappy paths.
    member this.Test_ASTParser_UnhappyPaths(tc: ASTParserErrorTestCase) =
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
        match actual with
               | Ok _ -> Assert.Fail("Unexpected pass, error tests must return errors")
               | Error err -> Assert.AreEqual(expected, err)
