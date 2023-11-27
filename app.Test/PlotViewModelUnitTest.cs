
using Moq;
using OxyPlot.Series;
using System.Collections.ObjectModel;

namespace app.Test
{
    internal class PlotViewModelUnitTest
    {
        private app.PlotViewModel _viewModel;
        private Mock<IPlotEquationEvaluator> _modelMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IPlotEquationEvaluator>();
            _viewModel = new PlotViewModel(_modelMock.Object);
        }

        [Test]
        public void PlotViewModel_InterpretCommand_CallsEvaluateOnce()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "y=2x+12";
            _viewModel.InputEquation = testInput;

            // ---
            // ACT
            // ---
            _viewModel.InterpretCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            _modelMock.Verify(m => m.Evaluate(testInput), Times.Once);
        }
    }
}
