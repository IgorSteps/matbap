
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
        public void PlotViewModel_InterpretCommand_WillDoSomething()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "y=2x+12";
            var expectedResponse = new List<double[]>
            {
                new double[] { 0, 2 },
                new double[] { 1, 4 },
                new double[] { 2, 6 } 
            };
            _viewModel.InputEquation = testInput;
            _modelMock.Setup(i => i.Evaluate(testInput)).Returns(expectedResponse);
            
            LineSeries lineSeries = _viewModel.PlotModel.Series[0] as LineSeries;
            Assert.IsNotNull(lineSeries);
            
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
