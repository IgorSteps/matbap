
//using Moq;
//using OxyPlot.Series;
//using System.Collections.ObjectModel;

//namespace app.Test
//{
//    internal class PlotViewModelUnitTest
//    {
//        private app.PlotViewModel _viewModel;
//        private Mock<IPlotEquationEvaluator> _modelMock;

//        [SetUp]
//        public void Setup()
//        {
//            _modelMock = new Mock<IPlotEquationEvaluator>();
//            _viewModel = new PlotViewModel(_modelMock.Object);
//        }

//        [Test]
//        public void PlotViewModel_InterpretCommand_Success()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            var testInput = "2*x+2";
//            _viewModel.InputEquation = testInput;
//            double[][] expectedPoints = new double[][]
//            {
//                new double[] { 1, 2 },
//                new double[] { 3, 4 },
//            };
//            var testResult = new EvaluationResult(expectedPoints, "");
//            double xMin = -10, xMax = 10, xStep = 0.1;
//            _modelMock.
//                Setup(e => e.Evaluate(xMin, xMax, xStep, testInput)).
//                Returns(testResult);


//            // ---
//            // ACT
//            // ---
//            _viewModel.PlotCmd.Execute(null);

//            // ------
//            // ASSERT
//            // ------
//            var lineSeries = _viewModel.PlotModel.Series[0] as LineSeries; ;
//            Assert.That(lineSeries, Is.Not.Null, "line series shouldn't be nil");
//            Assert.That(lineSeries.Points, Has.Count.EqualTo(expectedPoints.Length), "number of points in the lines series doesn't match expected");
//            for (int i = 0; i < expectedPoints.Length; i++)
//            {
//                Assert.That(lineSeries.Points[i].X, Is.EqualTo(expectedPoints[i][0]), "x coords don't match");
//                Assert.That(lineSeries.Points[i].Y, Is.EqualTo(expectedPoints[i][1]), "y coords don't match");
//            }
//        }

//        [Test]
//        public void PlotViewModel_InterpretCommand_Error()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            var testInput = "y=2";
//            _viewModel.InputEquation = testInput;
//            var testError = "error";
//            var testResult = new EvaluationResult(new double[100][], testError);
//            double xMin = -10, xMax = 10, xStep = 0.1;
//            _modelMock.
//                Setup(e => e.Evaluate(xMin, xMax, xStep, testInput)).
//                Returns(testResult);
            
//            // ---
//            // ACT
//            // ---
//            _viewModel.PlotCmd.Execute(null);

//            // ------
//            // ASSERT
//            // ------
//            Assert.That(_viewModel.EvaluatorError, Is.EqualTo(testError));
//        }
//    }
//}
