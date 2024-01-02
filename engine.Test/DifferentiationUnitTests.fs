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


                   //Certainly! Testing your chain rule implementation thoroughly is crucial to ensure its correctness and robustness. Below are some test cases that cover a variety of scenarios. These tests involve differentiating composite functions where the chain rule is applicable. 
                   
                   //For each test case, you would need an input expression and the expected output after differentiation. Since the code deals with symbolic differentiation, the tests will check if the output expression matches the expected expression.
                   
                   //### Test Case 1: Basic Chain Rule Application
                   //- **Input**: `Function("sin", Function("cos", Variable("x")))`
                   //- **Expected Output**: Product of the derivative of `sin` at `cos(x)` and the derivative of `cos(x)` w.r.t. `x`.
                   //- **Expected Symbolic Output**: `BinaryOperation("*", Function("cos", Function("cos", Variable("x"))), UnaryMinusOperation("-", Function("sin", Variable("x"))))`
                   
                   //### Test Case 2: Chain Rule with Polynomial
                   //- **Input**: `Function("exp", BinaryOperation("^", Variable("x"), Number(Int 2)))`
                   //- **Expected Output**: `exp(x^2)` times the derivative of `x^2`.
                   //- **Expected Symbolic Output**: `BinaryOperation("*", Function("exp", BinaryOperation("^", Variable("x"), Number(Int 2))), BinaryOperation("*", Number(Int 2), Variable("x")))`
                   
                   //### Test Case 3: Nested Functions
                   //- **Input**: `Function("log", Function("sin", Function("exp", Variable("x"))))`
                   //- **Expected Output**: Derivative of `log` at `sin(exp(x))` times derivative of `sin` at `exp(x)` times derivative of `exp(x)`.
                   //- **Expected Symbolic Output**: `BinaryOperation("*", BinaryOperation("/", Number(Int 1), Function("sin", Function("exp", Variable("x")))), BinaryOperation("*", Function("cos", Function("exp", Variable("x"))), Function("exp", Variable("x"))))`
                   
                   //### Test Case 4: Chain Rule with Constants
                   //- **Input**: `Function("sin", BinaryOperation("*", Number(Int 3), Variable("x")))`
                   //- **Expected Output**: Derivative of `sin` at `3x` times derivative of `3x`.
                   //- **Expected Symbolic Output**: `BinaryOperation("*", Function("cos", BinaryOperation("*", Number(Int 3), Variable("x"))), Number(Int 3))`
                   
                   //### Test Case 5: Function with Non-Chain Rule Part
                   //- **Input**: `BinaryOperation("+", Function("exp", Variable("x")), Variable("x"))`
                   //- **Expected Output**: Sum of the derivative of `exp(x)` and the derivative of `x`.
                   //- **Expected Symbolic Output**: `BinaryOperation("+", Function("exp", Variable("x")), Number(Int 1))`
                   
                   //These test cases cover a range of scenarios, including basic chain rule application, nested functions, interaction with polynomials, handling constants, and mixed expressions. To conduct these tests, you would run each input through your `differentiate` function and compare the output with the expected symbolic output. This will help ensure that your implementation of the chain rule is working as intended.
