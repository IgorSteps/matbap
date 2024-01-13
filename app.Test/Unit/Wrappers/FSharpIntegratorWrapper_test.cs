using Engine;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Moq;
using FSharpVertices = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;

namespace app.Test.Unit
{
    public class FSharpIntegratorWrapper_test
    {
        private Mock<Engine.IIntegrator> _mockIntegrator;
        private FSharpIntegratorWrapper _integratorWrapper;
        
        [SetUp]
        public void Setup()
        {
            _mockIntegrator = new Mock<Engine.IIntegrator>();
            _integratorWrapper = new FSharpIntegratorWrapper(_mockIntegrator.Object);
        }

        [Test]
        public void Test_FSharpIntegratorWrapper_Integrate_HappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            double min = 1.0, max = 10, step = 0.1;

            // Returned by F# engine.
            var vertciesTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(1.0, 2.0),
                new Tuple<double, double>(3.0, 4.0),
            };
            FSharpVertices fsharpVertexList = ListModule.OfSeq(vertciesTuples);

            // What it will be converted to.
            var expectedVertices = new double[][]
            {
                new double [] {1.0, 2.0},
                new double [] {3.0, 4.0},
            };

            var tuple = Tuple.Create(2.0, fsharpVertexList);
            var successResult = FSharpResult<Tuple<double, FSharpVertices>, string>.NewOk(tuple);

            _mockIntegrator.Setup(i => i.Integrate(expression, min, max, step)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _integratorWrapper.CalculateAreaUnderCurve(expression, min, max, step);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Area, Is.EqualTo(2), "Areas don't match");
            Assert.That(result.Vertices, Is.EqualTo(expectedVertices), "Vertices don't match");
        }

        [Test]
        public void Test_FSharpIntegratorWrapper_Integrate_UnhappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            double min = 1.0, max = 10, step = 0.1;

            string error = "Boom";

            var successResult = FSharpResult<Tuple<double, FSharpVertices>, string>.NewError(error);

            _mockIntegrator.Setup(i => i.Integrate(expression, min, max, step)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _integratorWrapper.CalculateAreaUnderCurve(expression, min, max, step);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Area, Is.EqualTo(0), "Area must be 0");
            Assert.That(result.Error.Message, Is.EqualTo(error), "Errors don't match");
        }
    }
}
