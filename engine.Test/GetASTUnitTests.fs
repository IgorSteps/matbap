
namespace Engine.Tests
open Engine.Types
open NUnit.Framework
    module GetASTUnitTests =
        type GetASTTestCaseHappy = {
            Args: string;
            Expected: Engine.Types.Node
        }

        type GetASTTestCaseUnhappy = {
            Args: string;
            Expected: string
        }

        [<TestFixture>]
        type GetASTTests () =
            static member testCases: GetASTTestCaseHappy list = [
                {
                    Args = "1+1"
                    Expected = BinaryOperation("+", Number(Int 1), Number(Int 1))
                }
            ]

            [<TestCaseSource("GetASTTests")>]
            member this.Test_GetAST_HappyPath(tc:GetASTTestCaseHappy)=
                // --------
                // Assemble
                // --------
                let args = tc.Args
                let expected = tc.Expected
  
                // ---
                // Act
                // ---
                let actual = Engine.ASTGetter.getAST args

                // ------
                // Assert
                // ------
                match actual with
                 | Ok ast -> Assert.AreEqual(expected, ast)
                 | Error err -> Assert.Fail("Getting AST failed with unexpected error: " + err)

            static member errorTestCases: GetASTTestCaseUnhappy list = [
                {
                    Args = "1a"
                    Expected = "Error"
                }
            ]

            [<TestCaseSource("GetASTTests")>]
            member this.Test_GetAST_UnhappyPath(tc:GetASTTestCaseUnhappy)=
                // --------
                // Assemble
                // --------
                let args = tc.Args
                let expected = tc.Expected
  
                // ---
                // Act
                // ---
                let actual = Engine.ASTGetter.getAST args

                // ------
                // Assert
                // ------
                match actual with
                   | Ok _ -> Assert.Fail("Unexpected pass, error tests must return errors")
                   | Error err -> Assert.AreEqual(expected, err)                