using Moq;

namespace app.Test
{
    public class PlottingServiceTest
    {
        private app.PlotingService _plottingService;
        private Mock<IPlotEquationEvaluator> _evaluatorMock;

        [SetUp]
        public void Setup()
        {
            _evaluatorMock = new Mock<IPlotEquationEvaluator>();
            _plottingService = new PlotingService(_evaluatorMock.Object);
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
    }
}
