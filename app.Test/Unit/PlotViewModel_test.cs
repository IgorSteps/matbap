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
        public void Test_PlotViewModel_PlotCmd_CreatesPlotSuccessfully()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";

            _plotterMock.
                Setup(p => p.CreatePlot(_viewModel.OxyPlotModel, testFunctionInput, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns((Error)null);

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

        public void Test_PlotViewModel_PlotCmd_PlotterError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";

            Error testError = new Error("Boom");
            _plotterMock.
                Setup(p => p.CreatePlot(_viewModel.OxyPlotModel, testFunctionInput, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(testError);

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(testError.ToString()), "Errors don't match");
            Assert.IsNull(_viewModel.OxyPlotModel, "Plot model should be null");
        }

        public void Test_PlotViewModel_ClearCmd_Clears()
        {
            // --------
            // ASSEMBLE
            // --------
            _plotterMock.Setup(p => p.ClearPlotModel(_viewModel.OxyPlotModel));

            // --------
            // ACT
            // --------
            _viewModel.ClearCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsNull(_viewModel.OxyPlotModel, "OxyPlotModel should've been cleared");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(0), "Plots collection should be empty");
        }

        public void Test_PlotViewModel_AddtangentCmd_AddsTangentSuccessfully()
        {
            // --------
            // ASSEMBLE
            // --------
            string testFunctionInput = "x+1";

            _plotterMock.
                Setup(p => p.AddTangent(_viewModel.OxyPlotModel, _viewModel.TangentX, testFunctionInput, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns((Error)null);

            // --------
            // ACT
            // --------
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There mustn't be an error");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(1), "Number of plots in the collection should be 1");
        }
    }
}

