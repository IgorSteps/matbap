using Moq;
using OxyPlot;
using OxyPlot.Series;

namespace app.Test.Unit
{
    public class PlotViewModel_test
    {
        private PlotViewModel _viewModel;
        private Mock<IPlotter> _plotterMock;

        [SetUp]
        public void Setup()
        {
            _plotterMock = new Mock<IPlotter>();
            _viewModel = new PlotViewModel(_plotterMock.Object);
        }

        [Test]
        public void Test_PlotViewModel_PlotCmd_UpdatePlotModel()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";
            double testInput = 1;

            // Setting properties on ViewModel as they are the ones passed to
            // CreatePlot. Without that, mock setup will fail.
            _viewModel.XMinimum = testInput;
            _viewModel.XMaximum = testInput;
            _viewModel.XStep = testInput;
            _viewModel.InputEquation = testFunctionInput;

            // Need to give PlotModel a line series so that it triggers UpdateSeriesColors
            // update of equationColours.
            var testLineSeries = new LineSeries();
            testLineSeries.Title = "boo";
            var plotModel = new PlotModel();
            plotModel.Series.Add(testLineSeries);

            var testResult = new PlotResult(plotModel, null);
            _plotterMock.Setup(p => p.CreatePlot(testFunctionInput, testInput, testInput, testInput)).Returns(testResult);

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There mustn't be an error");
            Assert.That(_viewModel.OxyPlotModel, Is.EqualTo(plotModel), "Plot models should be equal");
            Assert.IsNotEmpty(_viewModel.EquationColors, "EquationColours can't be empty");
        }

        public void Test_PlotViewModel_PlotCmd_PlotterError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";
            double testInput = 1;

            // Setting properties on ViewModel as they are the ones passed to
            // CreatePlot. Without that, mock setup will fail.
            _viewModel.XMinimum = testInput;
            _viewModel.XMaximum = testInput;
            _viewModel.XStep = testInput;
            _viewModel.InputEquation = testFunctionInput;

            // Need to give PlotModel a line series so that it triggers UpdateSeriesColors
            // update of equationColours.
            var testLineSeries = new LineSeries();
            testLineSeries.Title = "boo";
            var plotModel = new PlotModel();
            plotModel.Series.Add(testLineSeries);

            string testError = "Boom";
            var testResult = new PlotResult(null, testError);
            _plotterMock.Setup(p => p.CreatePlot(testFunctionInput, testInput, testInput, testInput)).Returns(testResult);

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(testError), "Errors don't match");
            Assert.IsNull(_viewModel.OxyPlotModel, "Plot model should be null");
        }

        public void Test_PlotViewModel_ClearCmd_Clears()
        {
            // --------
            // ASSEMBLE
            // --------
            _plotterMock.Setup(p => p.ClearPlots());

            // --------
            // ACT
            // --------
            _viewModel.ClearCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.EquationColors, "EquationColours should've been cleared");
            Assert.IsNull(_viewModel.OxyPlotModel, "OxyPlotModel should've been cleared");
        }

        public void Test_PlotViewModel_AddtangentCmd_AddsTangent()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";
            double testInput = 1;

            // Setting properties on ViewModel as they are the ones passed to
            // AddTangent. Without that, mock setup will fail.
            _viewModel.XMinimum = testInput;
            _viewModel.XMaximum = testInput;
            _viewModel.XStep = testInput;
            _viewModel.InputEquation = testFunctionInput;

            // Need to give PlotModel a line series so that it triggers UpdateSeriesColors
            // update of equationColours.
            var testLineSeries = new LineSeries();
            testLineSeries.Title = "Tangent Boo";
            var plotModel = new PlotModel();
            plotModel.Series.Add(testLineSeries);

            var testResult = new PlotResult(plotModel, null);
            _plotterMock.
                Setup(p => p.AddTangent(testInput, testFunctionInput, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(testResult);

            // --------
            // ACT
            // --------
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There mustn't be an error");
            Assert.That(_viewModel.OxyPlotModel, Is.EqualTo(plotModel), "Plot models should be equal");
            Assert.IsNotEmpty(_viewModel.EquationColors, "EquationColours can't be empty");
        }
    }
}

