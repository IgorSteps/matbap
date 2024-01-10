namespace Engine.Tests

open Engine
open NUnit.Framework
open System.Collections.Generic
open Types

type AstEvaluatorTestCase = {
    Args: string;
    Expected: Result<(string*NumType) * Points * SymbolTable, string>;
}
type AstPlotTestCase = {
    Min: float;
    Max: float;
    Step: float;
    Exp: string;
    Expected: Result<float array array, string>;
}
type rootTestCase = {
    Min: float;
    Max: float;
    Exp: string;
    Expected: Result<float array, string>;
}

module AstHelper = 
    let createDictionary (vName: string) (tVal: NumType) = dict [vName, tVal] |> Dictionary

[<TestFixture>]
[<DefaultFloatingPointTolerance(0.0000000001)>]
type AstEvaluatorTests () =
    static member astEvaluatorTestCases: AstEvaluatorTestCase list = [
       {
            // Basic addition
            Args = "2+10"
            Expected = Ok (("", Int 12), [], SymbolTable())
       }
       {
            // Basic subtraction
            Args = "7-3"
            Expected = Ok (("", Int 4), [], SymbolTable())
       }
       {
            // Basic multiplication
            Args = "5*7"
            Expected = Ok (("", Int 35), [], SymbolTable())
       }
       {
            // Basic division, with integer truncation
            Args = "12/8"
            Expected = Ok (("", Int 1), [], SymbolTable())
       }  
       {
            // Using integers with floats
            Args = "5+2.3"
            Expected = Ok (("", Float 7.3), [], SymbolTable())
       }
       {
            Args = "5-2.3"
            Expected = Ok (("", Float 2.7), [], SymbolTable())
       }
       {
            // As above but for a different grammar
            Args = "4.62*9"
            Expected = Ok (("", Float 41.58), [], SymbolTable())
       }
       {
            Args = "49/7.0"
            Expected = Ok (("", Float 7), [], SymbolTable())

       }
       {
            // Using multiple operations together
            Args = "11.53-23+612"
            Expected = Ok (("", Float 600.53), [], SymbolTable())
       }
       {
            // Testing proper order of operations
            Args = "13.56-6+14*20.1"
            Expected = Ok (("", Float 288.96), [], SymbolTable())

       }
       {
            // Using brackets with an expression inside
            Args = "5*(3+0.5)"
            Expected = Ok (("", Float 17.5), [], SymbolTable())

       }
       {
            // Order of operations using brackets, and operations after brackets
            Args = "5-2.5/(6+6.5)+1"
            Expected = Ok (("", Float 5.8), [], SymbolTable())
       }
       {
            Args = "2.5+((2.5))*3"
            Expected = Ok (("", Float 10), [], SymbolTable())
       }
       {
            // Return a value on its own without operations
            Args = "9999"
            Expected = Ok (("", Int 9999), [], SymbolTable())

       }
       {
            // As above, but inside brackets
            Args = "(0)"
            Expected = Ok (("", Int 0), [], SymbolTable())
       }
       {
            // Unary minus addition
            Args = "3+-2"
            Expected = Ok (("", Int 1), [], SymbolTable())

       }
       {
            // Unary minus subtraction
            Args = "4--2"
            Expected = Ok (("", Int 6), [], SymbolTable())
       }
       {
            // Unary minus multiplication
            Args = "-3*-9"
            Expected = Ok (("", Int 27), [], SymbolTable())
       }
       {
            Args = "6*-10.5"
            Expected = Ok (("", Float -63), [], SymbolTable())
       }
       {
            // Unary minus division
            Args = "8/-2"
            Expected = Ok (("", Int -4), [], SymbolTable())
       }
       {
            Args = "-320/-64"
            Expected = Ok (("", Int 5), [], SymbolTable())
       }
       {
            // Unary minus with brackets
            Args = "-(6+11)"
            Expected = Ok (("", Int -17), [], SymbolTable())
       }
       {
            Args = "-(8+(-24))"
            Expected = Ok (("", Int 16), [], SymbolTable())
       }
       {
            // Test for exponent
            Args = "2^8"
            Expected = Ok (("", Int 256), [], SymbolTable())
       }
       {
            Args = "6^2.3"
            Expected = Ok (("", Float 61.6237149387), [], SymbolTable())
       }
       {
            Args = "25^0.5"
            Expected = Ok (("", Float 5), [], SymbolTable())
       }
       {
            // Testing order of operations with exponent
            Args = "2*3.0^2"
            Expected = Ok (("", Float 18), [], SymbolTable())
       }
       {
            // Subsequent exponent
            Args = "4^2^3"
            Expected = Ok (("", Float 65536), [], SymbolTable())
       }
       {
            // Test for modulo
            Args = "5%3"
            Expected = Ok (("", Int 2), [], SymbolTable())
       }
       {
            Args = "5216%413"
            Expected = Ok (("", Int 260), [], SymbolTable())
       }
       {
            // Subsequent modulo
            Args = "763%129%20"
            Expected = Ok (("", Int 18), [], SymbolTable())
       }
       {
            // Negative exponent
            Args = "4^-2"
            Expected = Ok (("", Float 0.0625), [], SymbolTable())
       }
       {
            Args = "4^-1"
            Expected = Ok (("", Float 0.25), [], SymbolTable())
       }
       {
            // Negative modulo
            Args = "71%-15"
            Expected = Ok (("", Int 11), [], SymbolTable())
       }
       {
            Args = "-71%15"
            Expected = Ok (("", Int -11), [], SymbolTable())
       }
       {
            Args = "-71%-15"
            Expected = Ok (("", Int -11), [], SymbolTable())
       }
       {
            // Basic variable assignment with integer
            Args = "x=3"
            Expected = Ok (("x", Int 3), [], (AstHelper.createDictionary "x" (Int 3)))
       }
       {
            // Basic variable assignment with float
            Args = "var1=5.0"
            Expected = Ok (("var1", Float 5), [], (AstHelper.createDictionary "var1" (Float 5)))
       }
       {
            // Assignment with float and int
            Args = "var2=5.5+2"
            Expected = Ok (("var2", Float 7.5), [], (AstHelper.createDictionary "var2" (Float 7.5)))
       }
       {
            // Negative brackets assignment
            Args = "y=-(6+11)"
            Expected = Ok (("y", Int -17), [], (AstHelper.createDictionary "y" (Int -17)))

       }
       {
            // For loop points
            Args = "for x in range(1,5): 2*x + 1"
            Expected = Ok (("", Int 0), [(1.0, 3.0); (2.0, 5.0); (3.0, 7.0); (4.0,9.0); (5.0, 11.0)], SymbolTable())
       }
       {
            // For loop points with function call
            Args = "for x in range(0,1): sin(x)"
            Expected = Ok (("", Int 0), [(0.0, 0.0); (1.0, 0.8414709848);], SymbolTable())

       }
       {
            // For loop points with function call, negative xmin
            Args = "for x in range(-1,1): sin(x)"
            Expected = Ok (("", Int 0), [(-1.0, -0.8414709848); (0.0, 0.0); (1.0, 0.8414709848);], SymbolTable())
                
       }
    ]
    
    static member astPlotTestCases: AstPlotTestCase list = [
        // Plot test cases.
        // Some of these can be rewritten once implicit multiplication is allowed
        {
            Min = 0; Max = 5; Step = 0.1; Exp = "(x^2)+(4*x)+4";
            Expected = Ok [|[|0.0; 4.0|]; [|0.1; 4.41|]; [|0.2; 4.84|]; [|0.3; 5.29|];
            [|0.4; 5.76|]; [|0.5; 6.25|]; [|0.6; 6.76|]; [|0.7; 7.29|]; [|0.8; 7.84|];
            [|0.9; 8.41|]; [|1.0; 9.0|]; [|1.1; 9.61|]; [|1.2; 10.24|]; [|1.3; 10.89|];
            [|1.4; 11.56|]; [|1.5; 12.25|]; [|1.6; 12.96|]; [|1.7; 13.69|]; [|1.8; 14.44|];
            [|1.9; 15.21|]; [|2.0; 16.0|]; [|2.1; 16.81|]; [|2.2; 17.64|]; [|2.3; 18.49|];
            [|2.4; 19.36|]; [|2.5; 20.25|]; [|2.6; 21.16|]; [|2.7; 22.09|]; [|2.8; 23.04|];
            [|2.9; 24.01|]; [|3.0; 25.0|]; [|3.1; 26.01|]; [|3.2; 27.04|]; [|3.3; 28.09|];
            [|3.4; 29.16|]; [|3.5; 30.25|]; [|3.6; 31.36|]; [|3.7; 32.49|]; [|3.8; 33.64|];
            [|3.9; 34.81|]; [|4.0; 36.0|]; [|4.1; 37.21|]; [|4.2; 38.44|]; [|4.3; 39.69|];
            [|4.4; 40.96|]; [|4.5; 42.25|]; [|4.6; 43.56|]; [|4.7; 44.89|]; [|4.8; 46.24|];
            [|4.9; 47.61|]; [|5.0; 49.0|]|]
        }
        {
            Min = -4; Max = 9; Step = 1; Exp = "2*x+6";
            Expected = Ok [|[|-4.0; -2.0|]; [|-3.0; 0.0|]; [|-2.0; 2.0|]; [|-1.0; 4.0|];
            [|0.0; 6.0|]; [|1.0; 8.0|]; [|2.0; 10.0|]; [|3.0; 12.0|]; [|4.0; 14.0|];
            [|5.0; 16.0|]; [|6.0; 18.0|]; [|7.0; 20.0|]; [|8.0; 22.0|]; [|9.0; 24.0|]|]
        }
        {
            Min = -0.5; Max = 2; Step = 0.7; Exp = "x+2";
            Expected = Ok [|[|-0.5; 1.5|]; [|0.2; 2.2|]; [|0.9; 2.9|]; [|1.6; 3.6|]|]
        }
        {
            Min = -0.5; Max = 7.5; Step = 2; Exp = "x/2";
            Expected = Ok [|[|-0.5; -0.25|]; [|1.5; 0.75|]; [|3.5; 1.75|]; [|5.5; 2.75|]; [|7.5; 3.75|]|]
        }
        {
            Min = 0; Max = 3; Step = 0.25; Exp = "2-x^2";
            Expected = Ok [|[|0.0; 2.0|]; [|0.25; 1.9375|]; [|0.5; 1.75|]; [|0.75; 1.4375|];
            [|1.0; 1.0|]; [|1.25; 0.4375|]; [|1.5; -0.25|]; [|1.75; -1.0625|]; [|2.0; -2.0|];
            [|2.25; -3.0625|]; [|2.5; -4.25|]; [|2.75; -5.5625|]; [|3.0; -7.0|]|]
        }
        {
            Min = -2; Max = 2; Step = 0.25; Exp = "x^3";
            Expected = Ok [|[|-2.0; -8.0|]; [|-1.75; -5.359375|]; [|-1.5; -3.375|];
            [|-1.25; -1.953125|]; [|-1.0; -1.0|]; [|-0.75; -0.421875|]; [|-0.5; -0.125|];
            [|-0.25; -0.015625|]; [|0.0; 0.0|]; [|0.25; 0.015625|]; [|0.5; 0.125|];
            [|0.75; 0.421875|]; [|1.0; 1.0|]; [|1.25; 1.953125|]; [|1.5; 3.375|];
            [|1.75; 5.359375|]; [|2.0; 8.0|]|]
        }
        {
            Min = 2; Max = 2; Step = 0.25; Exp = "x";
            Expected = Ok [|[|2.0; 2.0|]|];
        }
        {
            Min = 3; Max = 4; Step = 1; Exp = "y=x+2";
            Expected = Error "Evaluation error: can't assign variables while in plot mode."
        }
        {
            Min = 3; Max = 4; Step = 1; Exp = "abcd=efgh";
            Expected = Error "Evaluation error: can't assign variables while in plot mode."
        }
    ]
    
    static member rootTestCases: rootTestCase list = [
        // Root finding test cases (up to 5 d.p. accuracy should be accepted)
        {
            Min = -5; Max = 5; Exp = "x^2";
            Expected = Ok [| 0 |];
        }
        {
            Min = -214; Max = 193; Exp = "(x^3)+(3*x^2)-2";
            Expected = Ok [| -2.73205; -1; 0.73205 |];
        }
        {
            Min = -30; Max = 54; Exp = "(6*x^2)-3.2";
            Expected = Ok [| -0.73030; 0.73030 |];
        }
        {
            Min = -10; Max = 10; Exp = "x^2+3";
            Expected = Ok [| |];
        }
        {
            Min = 0; Max = 200; Exp = "x^4-50";
            Expected = Ok [| 2.65915 |];
        }
        {
            Min = -5; Max = 5; Exp = "sin(x)";
            Expected = Ok [| -3.14159; 0; 3.14159 |];
        }
        
        // Edge cases
        // Min and max identical
        {
            Min = -1; Max = -1; Exp = "x";
            Expected = Ok [| |];
        }
        // Large range with no roots
        {
            Min = -50000; Max = 50000; Exp = "-1";
            Expected = Ok [| |];
        }
        // Function with asymptotes, and no real roots.
        // Correct behaviour is to return an empty array but if it does... worth investigating!
        {
            Min = -10; Max = 10; Exp = "1/x";
            Expected = Ok [| 0 |];
        }
        // Rapidly changing function: should find more but not calculated at a high enough accuracy to
        {
            Min = -1; Max = 1; Exp = "sin(1/x)";
            Expected = Ok [| -0.31831; 0.07957; 0.31831 |];
        }
        // Non-polynomial
        {
            Min = -3; Max = 3; Exp = "3*x+2";
            Expected = Ok [| -0.66667 |];
        }
    ]

    [<TestCaseSource("astEvaluatorTestCases")>]
    // Check evaluator test cases
    member this._Test_AST_Evaluator_Pass(testCase: AstEvaluatorTestCase) =
        // Assemble
        let args = testCase.Args
        let expected = testCase.Expected
        let symTable = Dictionary<string, NumType>()
  
        // Act
        let actual = ASTEvaluator.eval args symTable false

        // Assert correct non-value returns
        let expectedReturn = match expected with
                             | Ok ((var, _), points, symTable) -> Ok (var, points, symTable)
                             | Error e -> Error e
        let actualReturn   = match actual with
                             | Ok ((var, _), points, symTable, _) -> Ok (var, points, symTable)
                             | Error e -> Error e
        Assert.That(actualReturn, Is.EqualTo(expectedReturn))
        // Assert correct value (within tolerance to the 10th decimal place, for floating point errors)
        let actualValue =   match actual with
                            | Ok ((_, x),_, _, _) -> match x with
                                                     | Int y -> float y
                                                     | Float y -> y
                            | _ -> infinity
                            // This should be something we never expect the result to be, in order to assert that it
                            // failed in the case of an error. 
        let expectedValue = match expected with
                            | Ok ((_, x), _, _) -> match x with
                                                     | Int y -> float y
                                                     | Float y -> y
                            | _ -> -infinity
        Assert.That(actualValue, Is.EqualTo(expectedValue));
            
    [<TestCaseSource("astPlotTestCases")>]
    // Check evaluator test cases
    member this.Test_AST_Plot_Pass(testCase: AstPlotTestCase) =
        // Assemble
        let expected = testCase.Expected
  
        // Act
        let actual = ASTEvaluator.plotPoints testCase.Min testCase.Max testCase.Step testCase.Exp

        // Assert
        match expected, actual with
        | Ok(expectedPoints), Ok(actualPoints) ->
            Assert.AreEqual(expectedPoints, actualPoints, "Points are not equal")
        | Error expectedError, Error actualError ->
            Assert.AreEqual(expectedError, actualError, "Errors are not equal")
        | _ ->
            Assert.Fail("Expected and actual have different result types")
            
    [<TestCaseSource("rootTestCases")>]
    [<DefaultFloatingPointTolerance(0.00001)>]
    // Check evaluator test cases (with 5 d.p. accuracy)
    member this.Test_Root_Pass(testCase: rootTestCase) =
        // Assemble
        let expected = testCase.Expected
  
        // Act
        let actual = ASTEvaluator.findRoots testCase.Min testCase.Max testCase.Exp

        // Assert
        match expected, actual with
        | Ok(expectedRoots), Ok(actualRoots) ->
            Assert.AreEqual(expectedRoots, actualRoots, "Roots found are not equal")
        | Error expectedError, Error actualError ->
            Assert.AreEqual(expectedError, actualError, "Errors are not equal")
        | _ ->
            Assert.Fail("Expected and actual have different result types")