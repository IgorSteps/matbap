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