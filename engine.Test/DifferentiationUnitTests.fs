namespace Engine.Test

module DifferentiationUnitTest =
    open NUnit.Framework
    open Engine
    open Types
    open Differentiation

    type DifferentiationTestCase = {
        Name: string;
        Args: Node;
        Expected: Node;
    }

    type DifferentiationErrorTestCase = {
        Name: string;
        Args: Node;
        Expected: string;
    }

    [<TestFixture>]
    type DifferentiationTests() =
        static member DifferentiationTestCases: DifferentiationTestCase list = [
            {
                Name = "Testing number differentiation"
                Args = Number(Int 5)
                Expected = Number(Int 0)
            }
            {
                Name = "Testing variable differentiation"
                Args = Variable("x")
                Expected = Number(Int 1)
            }
            {
                Name = "Testing polynomial differentiation"
                Args = BinaryOperation("+", Variable("x"), Variable("y"))
                Expected = BinaryOperation("+", Number(Int 1), Number(Int 0))
            }
            {
                Name = "Testing product rule, x*y = y"
                Args = BinaryOperation("*", Variable("x"), Variable("y"))
                Expected = BinaryOperation(
                                "+",
                                BinaryOperation(
                                    "*",
                                    Number(Int 1),
                                    Variable "y"
                                ),
                                BinaryOperation(
                                    "*",
                                    Variable "x",
                                    Number(Int 0)
                                )
                            )
            }
            {
                Name = "Testing quotent rule, x/y = ..."
                Args = BinaryOperation("/", Variable("x"), Variable("y"))
                Expected = BinaryOperation(
                                "/",
                                BinaryOperation(
                                    "-",
                                    BinaryOperation("*", Number(Int 1), Variable "y"),
                                    BinaryOperation("*", Variable "x", Number(Int 0))
                                ),
                                BinaryOperation(
                                    "^",
                                    Variable "y",
                                    Number(Int 2)
                                )
                            )
            }
            {
                Name = "Testing power rule, x^2 = 2x"
                Args = BinaryOperation("^", Variable("x"), Number(Int 2))
                Expected = BinaryOperation("*", Number(Int 2), BinaryOperation("^", Variable "x", Number(Int 1)))
            }
            {
                Name = "Testing chain rule, sin(cos(x)) = cos(cos(x)) * -sin(x)"
                Args = Function("sin", Function("cos", Variable("x")))
                Expected = BinaryOperation(
                                "*",
                                Function(
                                    "cos",
                                    Function(
                                        "cos",
                                        Variable("x")
                                    )
                                ),
                                BinaryOperation(
                                    "*",
                                    UnaryMinusOperation(
                                        "-",
                                        Function("sin", Variable("x"))
                                    ),
                                    Number(Int 1)
                                )
                            )
            }
            {
                Name = "Testing polynomial with chain rule"
                Args = Function("sin", BinaryOperation("^", Variable("x"), Number(Int 2)))
                Expected = BinaryOperation(
                                "*",
                                Function (
                                    "cos",
                                    BinaryOperation ("^", Variable "x", Number (Int 2))
                                ),
                                BinaryOperation(
                                    "*",
                                    Number (Int 2),
                                    BinaryOperation ("^", Variable "x", Number (Int 1))
                                )
                            )
            }
            {
                Name = "Testing chain rule with nested functions sin(cos(log(x)))"
                Args = Function("sin", Function("cos", Function("log", Variable "x")))
                Expected = BinaryOperation(
                                "*",
                                Function(
                                    "cos",
                                    Function(
                                        "cos",
                                        Function("log", Variable "x")
                                    )
                                ),
                                BinaryOperation(
                                    "*",
                                    UnaryMinusOperation(
                                        "-",
                                        Function(
                                            "sin",
                                            Function("log", Variable "x")
                                        )
                                    ),
                                    BinaryOperation(
                                        "*",
                                        BinaryOperation("/", Number(Int 1), Variable "x"),
                                        Number(Int 1)
                                    )
                                )
                            )
            }
        ]



        [<TestCaseSource("DifferentiationTestCases")>]
        member this.Test_Differentiation_Happy_Paths(tc: DifferentiationTestCase) =
            // --------
            // ASSEMBLE
            // --------
            let args = tc.Args
            let expected = tc.Expected


            // ---
            // ACT
            // ---
            let actual = differentiate args "x"

            // ------
            // ASSERT
            // ------
            match actual with
                   | Ok ast -> Assert.AreEqual(expected, ast)
                   | Error err -> Assert.Fail("Parsing failed with unexpected error: " + err)

        static member DifferentiationErrorTestCases: DifferentiationErrorTestCase list = [
            {
                Name = "Test error: unsupported operation type"
                Args = BinaryOperation("@", Number(Int 1), Number(Int 1))
                Expected = "Operation '@' is not supported for differentiation"
            }
            {
                Name = "Test error: unsupported node type"
                Args = VariableAssignment("x", Number(Int 1))
                Expected = "Unsupported node type for differentiation"
            }
            {
                Name = "Test error: non-constant power"
                Args = BinaryOperation("^", Variable("x"), BinaryOperation("+", Number(Int 1), Number(Int 2)))
                Expected = "Differentiation with non-constant power is not supported"
            }
            {
                Name = "Test error: unsupported function"
                Args = Function("ln", Variable("x"))
                Expected = "Function 'ln' is not supported for differentiation"
            }
        ]


        [<TestCaseSource("DifferentiationErrorTestCases")>]
        member this.Test_Differentiation_Unhappy_Paths(tc: DifferentiationErrorTestCase) =
            // --------
            // ASSEMBLE
            // --------
            let args = tc.Args
            let expected = tc.Expected

            // ---
            // ACT
            // ---
            let actual = differentiate args "x"

            // ------
            // ASSERT
            // ------
            match actual with
                   | Ok _ -> Assert.Fail("Unexpected pass, error tests must return errors")
                   | Error err -> Assert.AreEqual(expected, err)