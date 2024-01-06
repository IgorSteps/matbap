using Moq;
using OxyPlot;
using OxyPlot.Series;
using static Engine.Types.Node;

namespace app.Test.Unit
{
    public class PlottingServiceTest
    {
        private app.PlottingService _plottingService;

        private Mock<IValidator> _validatorMock;
        private Mock<IOxyPlotModelManager> _plotModelManagerMock;
        private Mock<IPlotManager> _plotManagerMock;
        private Mock<ITangentManager> _tangentManagerMock;
        private Mock<IExpressionManager> _expressionManagerMock;
        [SetUp]
        public void Setup()
        {
            _validatorMock = new Mock<IValidator>();
            _plotModelManagerMock = new Mock<IOxyPlotModelManager>();
            _plotManagerMock = new Mock<IPlotManager>();
            _tangentManagerMock = new Mock<ITangentManager>();
            _expressionManagerMock = new Mock<IExpressionManager>();
            _plottingService = new PlottingService(
                    _validatorMock.Object,
                    _plotModelManagerMock.Object,
                    _plotManagerMock.Object,
                    _tangentManagerMock.Object,
                    _expressionManagerMock.Object
                );
        }

        [Test]
        public void Test_PlottingService_CreatePlot_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string function = "1+1";
            double xmin = 1, xmax = 10, xstep = 0.1;

            Plot testPlot = new Plot(function, xmin, xmax, xstep);
            LineSeries testLineSeries = new LineSeries();
            var mockGetLineSeriesResult = new GetLineSeriesResult(testLineSeries, null);

            _validatorMock.Setup(v => v.ValidatePlotInput(xmin, xmax, xstep)).Returns((Error)null);
            _plotManagerMock.Setup(e => e.CreatePlot(function, xmin, xmax, xstep)).Returns(testPlot);
            _plotManagerMock.Setup(e => e.GetLineSeriesForPlot(testPlot)).Returns(mockGetLineSeriesResult);
            _plotModelManagerMock.
                Setup(m => m.AddSeriesToPlotModel(plotModel, testLineSeries)).
                Callback<PlotModel, Series>((plotModel, _) =>
                {
                    plotModel.Series.Add(testLineSeries);
                });
            _plotModelManagerMock.Setup(m => m.SetupAxisOnPlotModel(plotModel, xmin, xmax));

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlot(plotModel, function, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(result.HasError, Is.False, "If there is an error should be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Plot, Is.Not.Null, "Plot must not be null");
            Assert.That(plotModel.Series.Count, Is.EqualTo(1), "Plot Model must have 1 line series");
            _validatorMock.VerifyAll();
            _plotManagerMock.VerifyAll();
            _plotModelManagerMock.VerifyAll();
        }

        [Test]
        public void Test_PlottingService_CreatePlot_ValidatorError()
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
            var result = _plottingService.CreatePlot(plotModel, testInput, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Error, Is.EqualTo(validatorError), "Errors don't match");
            Assert.That(result.Plot, Is.Null, "Plot must be null");
            _validatorMock.VerifyAll();
        }

        [Test]
        public void Test_PlottingService_CreatePlot_PlotManagerError()
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
            double xmin = 10, xmax = 1, xstep = 0.1;
            Plot testPlot = new Plot(function, xmin, xmax, xstep);
            Error testError = new Error("test");
            var mockGetLineSeriesResult = new GetLineSeriesResult(null, testError);
            _validatorMock.Setup(v => v.ValidatePlotInput(xmin, xmax, xstep)).Returns((Error)null);
            _plotManagerMock.Setup(e => e.CreatePlot(function, xmin, xmax, xstep)).Returns(testPlot);
            _plotManagerMock.Setup(e => e.GetLineSeriesForPlot(testPlot)).Returns(mockGetLineSeriesResult);

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlot(plotModel, function, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Error, Is.EqualTo(testError), "Errors don't match");
            Assert.That(result.Plot, Is.Null, "Plot must be null");
            _validatorMock.VerifyAll();
            _plotManagerMock.VerifyAll();
        }

        [Test]
        public void Test_PlottingService_AddTangent_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel testPlotModel = new PlotModel();
            string testFunction = "x^2";
            double x = 5, xmin = 1, xmax = 10, xstep = 0.1;

            // Validator mock.
            _validatorMock.Setup(v => v.ValidateAddTangentInput(x, xmin, xmax, xstep)).Returns((Error)null);

            // CreateTangent mocks.
            Tangent mockTangent = new Tangent(x, x, x);
            var mockCreateTangentResult = new CreateTangentResult(mockTangent, null);
            _tangentManagerMock.Setup(m => m.CreateTangent(x, testFunction)).Returns(mockCreateTangentResult);

            // GetTangentLineSeries mocks.
            LineSeries testLineSeies = new LineSeries();
            var mockGetTangentLineSeriesresult = new GetTangentLineSeriesResult(testLineSeies, null);
            _tangentManagerMock.
                Setup(m => m.GetTangentLineSeries(mockCreateTangentResult.Tangent, xmin, xmax, xstep)).
                Returns(mockGetTangentLineSeriesresult);

            // AddSeriesToPlotModel and SetupAxisOnPlotModel mocks.
            _plotModelManagerMock.
                Setup(m => m.AddSeriesToPlotModel(testPlotModel, testLineSeies)).
                Callback<PlotModel, Series>((plotModel, _) =>
                {
                    plotModel.Series.Add(testLineSeies);
                }); ;
            _plotModelManagerMock.Setup(m => m.SetupAxisOnPlotModel(testPlotModel, xmin, xmax));


            // --------
            // ACT
            // --------
            var result = _plottingService.AddTangent(testPlotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.IsNull(result.Error, "Shouldn't return an error");
            Assert.That(result.Tangent, Is.Not.Null, "Tangent must not be null");
            Assert.That(testPlotModel.Series.Count, Is.EqualTo(1), "There should be a tangent line series in the OxyPlot's plot model");
            _validatorMock.VerifyAll();
            _tangentManagerMock.VerifyAll();
            _plotModelManagerMock.VerifyAll();
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
            var result = _plottingService.AddTangent(plotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Error, Is.EqualTo(validtorError), "Errors must be the same");
            Assert.That(result.Tangent, Is.Null, "Tangent must be null");
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
            Error testError = new Error("test error");

            var mockCreateTangentErrorResuklt = new CreateTangentResult(null, testError);
            _tangentManagerMock.Setup(e => e.CreateTangent(x, testFunction)).Returns(mockCreateTangentErrorResuklt);

            // --------
            // ACT
            // --------
            var result = _plottingService.AddTangent(plotModel, x, testFunction, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Error, Is.EqualTo(testError), "Errors don't match");
            Assert.That(result.Tangent, Is.Null, "Tangent must be null");
        }

        [Test]
        public void Test_PlottingService_CreatePlotFromExpression()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string testFunction = "x";
            Expression testExpression = new Expression(testFunction);
            var Points = new double[][]
            {
                new double [] {1.0, 2.0},
                new double [] {3.0, 4.0},
            };
            testExpression.Points = Points;
            int length = testExpression.Points.Length;
            double xmin = testExpression.Points[0][0];
            double xmax = testExpression.Points[length - 1][0];
            double xstep = xmin - testExpression.Points[0][1];
            Plot testPlot = new Plot(testFunction, xmin, xmax, xstep);
            _plotManagerMock.Setup(e => e.CreatePlot(testFunction, xmin, xmax, xstep)).Returns(testPlot);

            var testLineSeries = new LineSeries();
            foreach (var point in testExpression.Points)
            {
                testLineSeries.Points.Add(new DataPoint(point[0], point[1]));
            }
            _expressionManagerMock.Setup(m => m.GetLineSeriesFromExpression(testExpression)).Returns(testLineSeries);
            _plotModelManagerMock.
                Setup(m => m.AddSeriesToPlotModel(plotModel, testLineSeries)).
                Callback<PlotModel, Series>((plotModel, _) =>
                {
                    plotModel.Series.Add(testLineSeries);
                });
            _plotModelManagerMock.Setup(m => m.SetupAxisOnPlotModel(plotModel, xmin, xmax));

            // --------
            // ACT
            // --------
            var result = _plottingService.CreatePlotFromExpression(plotModel, testExpression);

            // --------
            // ASSERT
            // --------
            Assert.That(result.HasError, Is.False, "If there is an error should be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Plot, Is.Not.Null, "Plot must not be null");
            Assert.That(plotModel.Series.Count, Is.EqualTo(1), "Plot Model must have 1 line series");
            _plotManagerMock.VerifyAll();
            _plotModelManagerMock.VerifyAll();
            _expressionManagerMock.VerifyAll();
        }
    }
}
