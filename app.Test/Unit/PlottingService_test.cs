using Moq;
using OxyPlot.Series;

namespace app.Test.Unit
{
    public class PlottingServiceTest
    {
        private app.PlottingService _plottingService;
        private Mock<IPlotEquationEvaluator> _evaluatorMock;
        private Mock<IValidator> _validatorMock;

        [SetUp]
        public void Setup()
        {
            _evaluatorMock = new Mock<IPlotEquationEvaluator>();
            _validatorMock = new Mock<IValidator>();
            _plottingService = new PlottingService(_evaluatorMock.Object, _validatorMock.Object);
        }

        [Test]
        public void Test_PlottingService_CreatePort_AddsSeriesToOxyplotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            double[][] testPoints = new double[][] 
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            double testValue = 1;
            var testEvaluatorResult = new EvaluationResult(testPoints, null);
            string validtorError = null;
            _validatorMock.Setup(v => v.ValidatePlotInput(testValue, testValue, testValue)).Returns(validtorError);
            _evaluatorMock.
                Setup(e => e.Evaluate(testValue, testValue, testValue, testInput)).
                Returns(testEvaluatorResult);

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlot(testInput, testValue, testValue, testValue);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError);
            Assert.That(result.OxyPlotModel.Series.Count, Is.EqualTo(1));
        }

        [Test]
        public void Test_PlottingService_CreatePort_ValidatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            double[][] testPoints = new double[][]
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            double xmin = 10, xmax = 1, xstep = 0.1;
            string validtorError = "test error";
            _validatorMock.Setup(v => v.ValidatePlotInput(xmin, xmax, xstep)).Returns(validtorError);
            

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlot(testInput, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "There should be an error");
            Assert.IsNull(result.OxyPlotModel, "Plot Model must be null");
            Assert.That(result.Error, Is.EqualTo(validtorError), "Errors must be the same");
        }

        [Test]
        public void Test_PlottingService_CreatePort_EvauatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInvalidInput = "Boom";
            string testError = "Error";
            double testValue = 1;
            var testEvaluatorResult = new EvaluationResult(null, testError);
            _evaluatorMock.
                Setup(e => e.Evaluate(testValue, testValue, testValue, testInvalidInput)).
                Returns(testEvaluatorResult);

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlot(testInvalidInput, testValue, testValue, testValue);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError);
            Assert.IsNull(result.OxyPlotModel);  
            Assert.That(result.Error, Is.EqualTo(testError));
        }

        [Test]
        public void Test_PlottingService_Clear()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "11";
            double testValue = 1;
            double[][] testPoints = new double[][]
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            var testEvaluatorResult = new EvaluationResult(testPoints, null);
            _evaluatorMock.
                Setup(e => e.Evaluate(testValue, testValue, testValue, testInput)).
                Returns(testEvaluatorResult);

            _plottingService.CreatePlot(testInput, testValue, testValue, testValue);

            // --------
            // ACT
            // --------
            _plottingService.ClearPlots();

            // --------
            // ASSERT
            // --------
            Assert.That(_plottingService.OxyPlotModel.Series.Count, Is.EqualTo(0));
        }

        [Test]
        public void Test_PlottingService_AddTangent_ValidatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunction = "x";
            double x = 10;
            double xmin = 0, xmax = 10, xstep = 0.1;

            string validtorError = "test error";
            _validatorMock.Setup(v => v.ValidateAddTangentInput(x, xmin, xmax, xstep)).Returns(validtorError);

            // --------
            // ACT
            // --------
            var result = _plottingService.AddTangent(x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must return an error");
            Assert.IsNull(result.OxyPlotModel, "Plot Model should be null");
            Assert.That(result.Error, Is.EqualTo(validtorError), "Errors must be the same");
        }

        [Test]
        public void Test_PlottingService_AddTangent_EvaluatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunction = "x";
            double x = 10;
            double xmin = 0, xmax = 10, xstep = 0.1;
            string testError = "test error";
        
            var testEvaluationResult = new EvaluationResult(null, testError);
            
            _evaluatorMock.Setup(e => e.Evaluate(x, x, x, testFunction)).Returns(testEvaluationResult);

            // --------
            // ACT
            // --------
            var result = _plottingService.AddTangent(x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must return an error");
            Assert.IsNull(result.OxyPlotModel, "Plot Model should be null");
        }

        [Test]
        public void Test_PlottingService_AddTangent_Correct_LineSeriesPoints()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunction = "x^2";
            double x = 2;
            double expectedY = x * x; // y = x^2
            double expectedSlope = x * x;
            double xmin = 0, xmax = 10, xstep = 0.1;
            double expectedYIntercept = expectedY - expectedSlope * x;
            double[][] testReturnedPoint = new double[][]
            {
                // Evaluator returns [x,y].
                new double[] { x, expectedY },
            };
            var expectedEvaluationResult = new EvaluationResult(testReturnedPoint, null);
            string expectedTangentEquation = $"{expectedSlope} * x + {expectedYIntercept}";

            _evaluatorMock.Setup(e => e.Evaluate(x, x, x, testFunction)).Returns(expectedEvaluationResult);
            _evaluatorMock.Setup(e => e.TakeDerivative(x, testFunction)).Returns(expectedSlope);
            _evaluatorMock.Setup(e => e.Evaluate(xmin, xmax, xstep, expectedTangentEquation)).Returns(expectedEvaluationResult);

            // --------
            // ACT
            // --------
            var result = _plottingService.AddTangent(x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "Shouldn't return an error");
            Assert.That(result.OxyPlotModel.Series.Count, Is.EqualTo(1), "There should be a tangent line series in the plot model");

            var tangentLineSeries = (LineSeries)result.OxyPlotModel.Series.Last();
            Assert.That(tangentLineSeries.Title, Is.EqualTo($"Tangent at x = {x}"), "Series titlea don't match");

            // Tolerance for floating point comparison.
            double tolerance = 0.001;

            // Check if the tangent line is correct.
            foreach (var point in tangentLineSeries.Points)
            {
                // c is y-intercept of tangent line, calculated as y - slope * x.
                double tangentYIntercept = expectedY - expectedSlope * x;
                // y = mx + c
                double expectedYValue = expectedSlope * point.X + tangentYIntercept;
                Assert.That(point.Y, Is.EqualTo(expectedYValue).Within(tolerance), $"Expected y = {expectedYValue}, but got y = {point.Y}");
            }
        }
    }
}
