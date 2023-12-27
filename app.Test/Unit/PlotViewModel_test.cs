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
            _plotterMock.
                Setup(p => p.CreatePlot(_viewModel.OxyPlotModel, _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
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

        [Test]
        public void Test_PlotViewModel_PlotCmd_PlotterError()
        {
            // --------
            // ASSEMBLE
            // --------
            Error testError = new Error("Boom");
            _plotterMock.
                Setup(p => p.CreatePlot(It.IsAny<PlotModel>(), _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns(testError);

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo(testError.ToString()), "Errors don't match");
            Assert.That(_viewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "Plot model should have 0 series");
        }

        [Test]
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
            Assert.That(_viewModel.OxyPlotModel.Series.Count, Is.EqualTo(0), "OxyPlotModel should be clean and empty");
            Assert.That(_viewModel.Plots.Count, Is.EqualTo(0), "Plots collection should be empty");
        }

        [Test]
        public void Test_PlotViewModel_AddtangentCmd_AddsTangent_SelectedPlotNullError()
        {
            // --------
            // ASSEMBLE
            // -------- 

            // --------
            // ACT
            // --------
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(_viewModel.Error, Is.EqualTo("You must select the plot"), "There must be an error");
        }

        [Test]
        public void Test_PlotViewModel_AddtangentCmd_AddsTangent_Sucess()
        {
            // --------
            // ASSEMBLE
            // -------- 
            _viewModel.OxyPlotModel.Title = "Hi";
            _viewModel.InputEquation = "x";
            _viewModel.TangentX = 2;
            _plotterMock.
                Setup(p => p.CreatePlot(_viewModel.OxyPlotModel, _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns((Error)null).
                Callback<PlotModel, string, double, double, double>((plotModel, _, _, _, _) =>
                {
                    plotModel.Series.Add(new LineSeries());
                });

            _plotterMock.
                Setup(p => p.AddTangent(_viewModel.OxyPlotModel, _viewModel.TangentX, _viewModel.InputEquation, _viewModel.XMinimum, _viewModel.XMaximum, _viewModel.XStep)).
                Returns((Error)null).
                Callback<PlotModel, double, string, double, double, double>((plotModel, _, _, _, _, _) =>
                {
                    plotModel.Series.Add(new LineSeries());
                });

            // --------
            // ACT
            // --------
            _viewModel.PlotCmd.Execute(null);
            _viewModel.AddTangentCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.IsEmpty(_viewModel.Error, "There shouldn't be an error");
            Assert.That(_viewModel.OxyPlotModel.Series.Count, Is.EqualTo(2), "There should be 2 serieses in the plot model");
        }
    }
}

