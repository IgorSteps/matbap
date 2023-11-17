
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

        // Test stub for future.
        [Test]
        public void PlotViewModel_InterpretCommand_WillDoSomething()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "y=2x+12";
            var expectedResponse = "123";
            _viewModel.InputEquation = testInput;
            _modelMock.Setup(i => i.Evaluate(testInput)).Returns(expectedResponse);

            // ---
            // ACT
            // ---
            _viewModel.InterpretCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            //Assert.That(_viewModel.Response, Is.EqualTo(expectedResponse), "Actual response is not equal expected");
        }
    }
}
