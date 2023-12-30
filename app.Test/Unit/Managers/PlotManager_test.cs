using Moq;

namespace app.Test.Unit
{
    public class PlotManager_test
    {
        private Mock<IFSharpFunctionEvaluatorWrapper> _evaluator;
        private PlotManager _manager;

        [SetUp]
        public void Setup()
        {
            _evaluator = new Mock<IFSharpFunctionEvaluatorWrapper>();
            _manager = new PlotManager( _evaluator.Object );
        }

        [Test]
        public void Test_PlotManager_CreatePlot_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x^2";
            double xmin = 0, xmax = 10, xstep = 1;

            // --------
            // ACT
            // --------
            var plot = _manager.CreatePlot(function, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsNotNull(plot, "Plot must not be null");
            Assert.That(plot.Function, Is.EqualTo(function), "Plot's function doesn't match");
            Assert.That(plot.XMin, Is.EqualTo(xmin), "Plot's xmin doesn't match");
            Assert.That(plot.XMax, Is.EqualTo(xmax), "Plot's xmax doesn't match");
            Assert.That(plot.XStep, Is.EqualTo(xstep), "Plot's xstep doesn't match");
        }

        [Test]
        public void Test_PlotManager_GetLineSeriesForPlot_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            Plot plot = new Plot("x^2", 0, 10, 1);
            double[][] testPoints = new[]
            {
                new double[] { 0, 0 },
                new double[] { 1, 1 }
            };
            var evaluationResult = new FunctionEvaluationResult(testPoints, null);
            _evaluator.
                Setup(e => e.Evaluate(plot.Function, plot.XMin, plot.XMax, plot.XStep)).
                Returns(evaluationResult);

            // --------
            // ACT
            // --------
            var result = _manager.GetLineSeriesForPlot(plot);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "HasError must be false");
            Assert.IsNotNull(result.LineSeries, "Line series must not be null");
            Assert.That(result.LineSeries.Points.Count, Is.EqualTo(2), "Line series should have 2 sets of [x,y] coords");
            _evaluator.VerifyAll();
        }

        [Test]
        public void Test_PlotManager_GetLineSeriesForPlot_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            Plot plot = new Plot("x^2", 0, 10, 1);
            Error error = new Error("Error evaluating function");
            var evaluationResult = new FunctionEvaluationResult(null, error);
            _evaluator.
                Setup(e => e.Evaluate(plot.Function, plot.XMin, plot.XMax, plot.XStep)).
                Returns(evaluationResult);

            // --------
            // ACT
            // --------
            var result = _manager.GetLineSeriesForPlot(plot);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "HasError must be true");
            Assert.That(result.Error, Is.EqualTo(error), "Errors don't match");
            Assert.IsNull(result.LineSeries, "Line series must be null");
            _evaluator.VerifyAll();
        }
    }
}
