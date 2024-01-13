namespace Engine.Test
    module IntegrationUnitTest =
        open Engine.Types
        open NUnit.Framework

        type IntegrationTestCase = {
            Name : string;
            Expression : string;
            Min : double;
            Max : double;
            Step : double;
            ExpectedArea : double;
            ExpectedVerticies : Vertices;
        }

        [<TestFixture>]
        type IntegrationTests() =
            static member IntegrationTestCases: IntegrationTestCase list = [
                {
                    Name = "Testing y=x, in the interval [0, 2] with a step of 1";
                    Expression = "x";
                    Min = 0.0;
                    Max = 2.0;
                    Step = 1.0;
                    ExpectedArea = 2.0;
                    ExpectedVerticies = [
                        (0.0, 0.0);
                        (0.0, 0.0);
                        (1.0, 1.0);
                        (1.0, 0.0);

                        (1.0, 0.0);
                        (1.0, 1.0);
                        (2.0, 2.0);
                        (2.0, 0.0);
                    ];
                }
                {
                    Name = "Testing y=x^2, in the interval [0, 2] with a step of 1";
                    Expression = "x^2";
                    Min = 0.0;
                    Max = 2.0;
                    Step = 1.0;
                    ExpectedArea = 3.0;
                    ExpectedVerticies = [
                        (0.0, 0.0);
                        (0.0, 0.0);
                        (1.0, 1.0);
                        (1.0, 0.0);

                        (1.0, 0.0);
                        (1.0, 1.0);
                        (2.0, 4.0);
                        (2.0, 0.0);
                    ];
                }
            ]   

            [<TestCaseSource("IntegrationTestCases")>]
            member this.Test_Integration_Happy_Paths(tc: IntegrationTestCase) =
                // --------
                // ASSEMBLE
                // --------
                let expression = tc.Expression
                let min = tc.Min
                let max = tc.Max
                let step = tc.Step
                let expectedArea = tc.ExpectedArea
                let expectedVerticies = tc.ExpectedVerticies


                // ---
                // ACT
                // ---
                let actual = Engine.Integration.integrate expression min max step

                // ------
                // ASSERT
                // ------
                match actual with
                        | Ok (area, verticies) ->
                            Assert.AreEqual(expectedArea, area)
                            CollectionAssert.AreEqual(expectedVerticies, verticies)
                        | Error err -> Assert.Fail("Integration failed with unexpected error: " + err)