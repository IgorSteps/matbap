using Moq;
using OxyPlot;

namespace app.Test.Unit
{
    public class TangentManager_test
    {
        private Mock<IFSharpFunctionEvaluatorWrapper> _evaluator;
        private Mock<IDifferentiator> _differentiator;
        private TangentManager _tangentManager;

        [SetUp]
        public void Setup()
        {
            _evaluator = new Mock<IFSharpFunctionEvaluatorWrapper>();
            _differentiator = new Mock<IDifferentiator>();
            _tangentManager = new TangentManager(_evaluator.Object, _differentiator.Object);
        }

        [Test]
        public void Test_TangentManager_CreateTangent_Sucess()
        {
            // --------
            // ASSEMBLE 
            // --------
            double x = 2, y = 4, slope = 4;
            string function = "x^2";
            double[][] testPoints = new[]
            {
                new double[] { x, y },
            };
            var evaluationResult = new FunctionEvaluationResult(testPoints, null);

            _evaluator.Setup(e => e.EvaluateAtPoint(x, function)).Returns(evaluationResult);
            Expression exp = new Expression(function);
            var diffResult = new DifferentiationServiceResult("2x", null);
            _differentiator.Setup(e => e.Differentiate(function)).Returns(diffResult);

            var diffEvaluateAtPointResult = new FunctionEvaluationResult(testPoints, null);
            _evaluator.Setup(e => e.EvaluateAtPoint(x, "2x")).Returns(evaluationResult);

            // --------
            // ACT
            // --------
            var result = _tangentManager.CreateTangent(x, function);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "HasError must be false");
            Assert.IsNotNull(result.Tangent, "Tangent must not be null");
            Assert.That(result.Tangent.Slope, Is.EqualTo(slope), "Slopes don't match");
            _evaluator.VerifyAll();
        }

        [Test]
        public void Test_TangentManager_CreateTangent_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            double x = 2;
            string function = "x^2";
            Error testError = new Error("test error");

            var evaluationResult = new FunctionEvaluationResult(null, testError);
            _evaluator.Setup(e => e.EvaluateAtPoint(x, function)).Returns(evaluationResult);

            // --------
            // ACT
            // --------
            var result = _tangentManager.CreateTangent(x, function);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "HasError must be true");
            Assert.That(result.Error, Is.EqualTo(testError), "Errors don't match");
            Assert.IsNull(result.Tangent, "Tangent must be null");
            _evaluator.VerifyAll();
        }

        [Test]
        public void Test_TangentManager_GetTangentLineSeries_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            double x = 1, y = 2, slope = 3;
            Tangent tangent = new Tangent(x, y, slope);
            double xmin = 1, xmax = 3, xstep = 0.1;

            double[][] testPoints = new[]
            {
                new double[] { 1, 2 },
                new double[] { 3, 4 }
            };
            var evaluationResult = new FunctionEvaluationResult(testPoints, null);
            _evaluator.Setup(f => f.Evaluate(tangent.GetTangentEquation(), xmin, xmax, xstep)).Returns(evaluationResult);

            // --------
            // ACT
            // --------
            var result = _tangentManager.GetTangentLineSeries(tangent, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "HasError must be false");
            Assert.IsNotNull(result.LineSeries, "Line series must not be null");
            Assert.That(result.LineSeries.Points.Count, Is.EqualTo(2), "There must be 2 line serieses");
            Assert.That(result.LineSeries.Title, Is.EqualTo($"Tangent at x = {x}"), "Title is wrong");
            Assert.That(result.LineSeries.LineStyle, Is.EqualTo(LineStyle.Dash), "Tangent lines must be dash");
            _evaluator.VerifyAll();
        }

    }
}
