using CommunityToolkit.Mvvm.Messaging;
using Moq;
using OxyPlot;
using OxyPlot.Series;

namespace app.Test.Unit
{
    public class PlotViewModel_test
    {
        private PlotViewModel _viewModel;
        private Mock<IPlotter> _plotterMock;
        private Mock<IAreaUnderCurveShower> _areaUnderCurveShowerMock;
        private Mock<IOxyPlotModelManager> _plotModelManager;
        [SetUp]
        public void Setup()
        {
            _plotterMock = new Mock<IPlotter>();
            _plotModelManager = new Mock<IOxyPlotModelManager>();
            _areaUnderCurveShowerMock = new Mock<IAreaUnderCurveShower>();
            _viewModel = new PlotViewModel(_plotterMock.Object, _plotModelManager.Object, _areaUnderCurveShowerMock.Object);
        }

        [Test]
        public void Test_PlotViewModel_PlotCmd_CreatePlot_Successfully()
        {
            // --------
            // ASSEMBLE
            // --------
            Plot testPlot = new Plot(_viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep);
            var mockResult = new CreatePlotResult(testPlot, null);
            _plotterMock.
                Setup(p => p.CreatePlot(_viewModel.OxyPlotModel, _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(mockResult);

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(1), "Number of plots in the collection should be 1");
            Assert.That(_viewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
            Assert.IsEmpty(_viewModel.Error, "Error must be empty");
        }

        [Test]
        public void Test_PlotViewModel_PlotCmd_CreatesPlot_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            Error testError = new Error("Boom");
            var mockResult = new CreatePlotResult(null, testError);

            _plotterMock.
                Setup(p => p.CreatePlot(It.IsAny<PlotModel>(), _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(mockResult);
            _plotModelManager.Setup(p => p.RefreshPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(testError.ToString()), "Errors don't match");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(0), "Plot model should have 0 series");
        }

        [Test]
        public void Test_PlotViewModel_ClearCmd_Clears()
        {
            // --------
            // ASSEMBLE
            // --------
            _plotModelManager.Setup(p => p.ClearPlotModel(_viewModel.OxyPlotModel));
            _plotModelManager.Setup(p => p.RefreshPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.ClearCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "OxyPlotModel should be clean and empty");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(0), "Plots collection should be empty");
        }

        [Test]
        public void Test_PlotViewModel_AddtangentCmd_AddTangent_SelectedPlotNullError()
        {
            // --------
            // ASSEMBLE
            // -------- 
            string errMsg = "You must select the plot to add a tangent to it.";

            // --------
            // ACT
            // --------
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(errMsg), "There must be an error");
        }

        [Test]
        public void Test_PlotViewModel_AddtangentCmd_AddsTangent_Sucess()
        {
            // --------
            // ASSEMBLE
            // -------- 
            Plot testPlot = new Plot(_viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep);
            _viewModel.SelectedPlot = testPlot;
            Tangent testTangent = new Tangent(1, 2, 3);
            var mockTangetResult = new AddTangentResult(testTangent, null);
            _plotterMock.
                Setup(p => p.AddTangent(_viewModel.OxyPlotModel, _viewModel.TangentX, _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(mockTangetResult);
            _plotModelManager.Setup(p => p.RefreshPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There shouldn't be an error");
            Assert.That(_viewModel.SelectedPlot.Tangent, Is.EqualTo(testTangent), "Tangents are not equal");
        }

        [Test]
        public void Test_PlotViewModel_HandlePlotExpressionMessage_Success()
        {
            // --------
            // ASSEMBLE
            // -------- 
            string testInput = "123";
            Expression testExpression = new Expression(testInput);
            var samplePoints = new double[][]
            {
                new double [] {1.0, 2.0},
                new double [] {3.0, 4.0},
            };
            testExpression.Points = samplePoints;
            int length = testExpression.Points.Length;
            double xmin = testExpression.Points[0][0];
            double xmax = testExpression.Points[length - 1][0];
            double xstep = xmin - testExpression.Points[0][1];
            Plot testPlot = new Plot(testInput, xmin, xmax, xstep);

            var mockedResult = new CreatePlotResult(testPlot, null);
            _plotterMock.Setup(p => p.CreatePlotFromExpression(_viewModel.OxyPlotModel, testExpression)).Returns(mockedResult);

            // --------
            // ACT
            // --------
            WeakReferenceMessenger.Default.Send(new PlotExpressionMessage(testExpression));

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There shouldn't be an error");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(1), "Number of plots in the collection should be 1");
            Assert.That(_viewModel.SelectedPlot, Is.Not.Null, "Selected plot must not be null");
        }

        [Test]
        public void Test_PlotViewModel_ShowAreaUnderTheCurveCmd_HappyPath()
        {
            // --------
            // ASSEMBLE
            // -------- 
            Plot testPlot = new Plot(_viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep);
            _viewModel.SelectedPlot = testPlot;

            double area = 100;
            var mockResult = new CalculateAreaUnderCurveResult(area, null);
            _areaUnderCurveShowerMock.Setup(p => p.ShowAreaUnderCurve(_viewModel.OxyPlotModel, testPlot, _viewModel.IntegrationStep)).
                Returns(mockResult);
            _plotModelManager.Setup(p => p.RefreshPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.ShowAreaUnderCurveCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo("Area under the curve = " + area.ToString()), "The error is used to display Area, so it can't be empty");
        }

        [Test]
        public void Test_PlotViewModel_ShowAreaUnderTheCurveCmd_UnhappyPath_ServiceError()
        {
            // --------
            // ASSEMBLE
            // -------- 
            Plot testPlot = new Plot(_viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep);
            _viewModel.SelectedPlot = testPlot;

            Error testError = new Error("Boom");
            var mockResult = new CalculateAreaUnderCurveResult(0, testError);
            _areaUnderCurveShowerMock.Setup(p => p.ShowAreaUnderCurve(_viewModel.OxyPlotModel, testPlot, _viewModel.IntegrationStep)).
                Returns(mockResult);
            _plotModelManager.Setup(p => p.RefreshPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.ShowAreaUnderCurveCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(testError.ToString()), "The errors don't match");
        }

        [Test]
        public void Test_PlotViewModel_ShowAreaUnderTheCurveCmd_UnhappyPath_NullSelectedPlot()
        {
            // --------
            // ASSEMBLE
            // -------- 

            // --------
            // ACT
            // --------
            _viewModel.ShowAreaUnderCurveCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo("You must select the plot to show area under it."), "The errors don't match");
        }
    }
}

