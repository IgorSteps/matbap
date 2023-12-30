using Engine;
using Microsoft.FSharp.Core;
using Moq;

namespace app.Test.Unit
{
    public class FSharpFunctionEvaluatorWrapper_test
    {
        private Mock<Engine.IEvaluator> _mockEngineEvaluator;
        private FSharpFunctionEvaluatiorWrapper _evaluator;

        [SetUp]
        public void Setup()
        {
            _mockEngineEvaluator = new Mock<Engine.IEvaluator>();
            _evaluator = new FSharpFunctionEvaluatiorWrapper(_mockEngineEvaluator.Object);
        }

        [Test]
        public void Test_FSharpFunctionEvaluatiorWrapper_Evaluate_EvaluatesSuccessfully()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double xmin = 1, xmax = 10, xstep = 0.1;
            double[][] points = new double[][]
            {
                new double[] {0,1},
                new double[] {2,3},
            };

            var successResult = FSharpResult<double[][], string>.NewOk(points);
            _mockEngineEvaluator.Setup(e => e.PlotPoints(xmin, xmax, xstep, function)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.Evaluate(function, xmin, xmax, xstep);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Points, Is.Not.Null, "Should've returned points");
        }

        [Test]
        public void Test_FSharpFunctionEvaluatiorWrapper_Evaluate_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double xmin = 1, xmax = 10, xstep = 0.1;
            string error = "Boom";
            var errorResult = FSharpResult<double[][], string>.NewError(error);
            _mockEngineEvaluator.Setup(e => e.PlotPoints(xmin, xmax, xstep, function)).Returns(errorResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.Evaluate(function, xmin, xmax, xstep);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error.Message, Is.EqualTo(error), "Errors don't match");
            Assert.That(result.Points, Is.Null, "Points should be null");
        }

        [Test]
        public void Test_FSharpFunctionEvaluatiorWrapper_EvaluateAtPoint_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double x = 1;
            double[][] points = new double[][]
            {
                new double[] {0,1},
            };

            var successResult = FSharpResult<double[][], string>.NewOk(points);
            _mockEngineEvaluator.Setup(e => e.PlotPoints(x, x, x, function)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.EvaluateAtPoint(x, function);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Points, Is.Not.Null, "Points shouldn't be null");
            Assert.That(result.Points, Has.Length.EqualTo(1), "Points should have only 1 [x,y] pair");
        }

        [Test]
        public void Test_FSharpFunctionEvaluatiorWrapper_EvaluateAtPoint_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "+1"; // invalid input.
            double x = 1;
            string error = "Boom";
            var errorResult = FSharpResult<double[][], string>.NewError(error);
            _mockEngineEvaluator.Setup(e => e.PlotPoints(x, x, x, function)).Returns(errorResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.EvaluateAtPoint(x, function);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error.Message, Is.EqualTo(error), "Errors don't match");
            Assert.That(result.Points, Is.Null, "Points should be null");
        }
    }
}
