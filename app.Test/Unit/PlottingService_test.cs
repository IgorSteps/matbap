using Moq;
using OxyPlot;
using OxyPlot.Series;

namespace app.Test.Unit
{
    public class PlottingServiceTest
    {
        private app.PlottingService _plottingService;
        private Mock<IFunctionEvaluator> _evaluatorMock;
        private Mock<IValidator> _validatorMock;

        [SetUp]
        public void Setup()
        {
            _evaluatorMock = new Mock<IFunctionEvaluator>();
            _validatorMock = new Mock<IValidator>();
            _plottingService = new PlottingService(_evaluatorMock.Object, _validatorMock.Object);
        }

        [Test]
        public void Test_PlottingService_CreatePort_AddsSeriesToOxyplotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();  
            string function = "1+1";
            double[][] testPoints = new double[][] 
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            double xmin = 1, xmax = 10, xstep = 0.1;
            var testEvaluatorResult = new EvaluationResult(testPoints, null);
            Error validtorError = null;
            _validatorMock.Setup(v => v.ValidatePlotInput(xmin, xmax, xstep)).Returns(validtorError);
            _evaluatorMock.Setup(e => e.Evaluate(xmin, xmax, xstep, function)).Returns(testEvaluatorResult);

            // --------
            // ACT
            // --------
            Error err = _plottingService.CreatePlot(plotModel, function, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsNull(err, "Must not return an error");
            Assert.That(plotModel.Series.Count, Is.EqualTo(1), "Plot Model must be populated with a Series");
        }

        [Test]
        public void Test_PlottingService_CreatePort_ValidatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string testInput = "1+1";
            double[][] testPoints = new double[][]
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            double xmin = 10, xmax = 1, xstep = 0.1;
            Error validatorError = new Error("test error");
            _validatorMock.Setup(v => v.ValidatePlotInput(xmin, xmax, xstep)).Returns(validatorError);


            // --------
            // ACT
            // --------
            Error err = _plottingService.CreatePlot(plotModel, testInput, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(err, Is.EqualTo(validatorError), "Errors don't match");
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "Plot Model must be empty");
        }

        [Test]
        public void Test_PlottingService_CreatePort_EvauatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string testInvalidInput = "Boom";
            string testError = "test error";
            double testValue = 1;
            var testEvaluatorResult = new EvaluationResult(null, testError);
            _evaluatorMock.
                Setup(e => e.Evaluate(testValue, testValue, testValue, testInvalidInput)).
                Returns(testEvaluatorResult);

            // --------
            // ACT
            // --------
            Error err = _plottingService.CreatePlot(plotModel, testInvalidInput, testValue, testValue, testValue);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "Plot Model must be empty");
            Assert.That(err.ToString(), Is.EqualTo("Error: " + testError), "Errors don't match");
        }

        [Test]
        public void Test_PlottingService_Clear()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
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

            _plottingService.CreatePlot(plotModel, testInput, testValue, testValue, testValue);

            // --------
            // ACT
            // --------
            _plottingService.ClearPlotModel(plotModel);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "Plot Model must be empty");
        }

        [Test]
        public void Test_PlottingService_AddTangent_ValidatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string testFunction = "x";
            double x = 10;
            double xmin = 0, xmax = 10, xstep = 0.1;

            Error validtorError = new Error("test error");
            _validatorMock.Setup(v => v.ValidateAddTangentInput(x, xmin, xmax, xstep)).Returns(validtorError);

            // --------
            // ACT
            // --------
            Error err = _plottingService.AddTangent(plotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "Plot Model must be empty");
            Assert.That(err, Is.EqualTo(validtorError), "Errors must be the same");
        }

        [Test]
        public void Test_PlottingService_AddTangent_EvaluatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string testFunction = "x";
            double x = 10;
            double xmin = 0, xmax = 10, xstep = 0.1;
            string testError = "test error";

            var testEvaluationResult = new EvaluationResult(null, testError);

            _evaluatorMock.Setup(e => e.Evaluate(x, x, x, testFunction)).Returns(testEvaluationResult);

            // --------
            // ACT
            // --------
            Error err = _plottingService.AddTangent(plotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(err.ToString(), Is.EqualTo("Error: " + testError), "Errors don't match");
            Assert.That(plotModel.Series.Count, Is.EqualTo(0), "Plot Model must be empty");
        }

        [Test]
        public void Test_PlottingService_AddTangent_Correct_LineSeriesPoints()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
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
            Error err = _plottingService.AddTangent(plotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsNull(err, "Shouldn't return an error");
            Assert.That(plotModel.Series.Count, Is.EqualTo(1), "There should be a tangent line series in the plot model");

            var tangentLineSeries = (LineSeries)plotModel.Series.Last();
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
