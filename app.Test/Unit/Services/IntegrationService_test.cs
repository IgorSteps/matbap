using NUnit.Framework;
using Moq;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;

namespace app.Test.Unit
{
    public class IntegrationService_test
    {
        private Mock<ITrapeziumManager> _mockTrapeziumManager;
        private Mock<IFSharpIntegratorWrapper> _mockIntegratorWrapper;
        private Mock<IOxyPlotModelManager> _mockOxyPlotManager;
        private Mock<IValidator> _mockValidationService;
        private IntegrationService _integrationService;

        [SetUp]
        public void Setup()
        {
            _mockTrapeziumManager = new Mock<ITrapeziumManager>();
            _mockIntegratorWrapper = new Mock<IFSharpIntegratorWrapper>();
            _mockOxyPlotManager = new Mock<IOxyPlotModelManager>();
            _mockValidationService = new Mock<IValidator>();
            _integrationService = new IntegrationService(
                _mockIntegratorWrapper.Object,
                _mockTrapeziumManager.Object,
                _mockOxyPlotManager.Object,
                _mockValidationService.Object
                );
        }

        [Test]
        public void Test_IntegrationService_ShowAreaUnderCurve_HappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string expression = "1";
            double area = 100, min = 1, max = 10, step = 0.1;
            double[][] vertices = new double[][]
            {
                new double[] {1.0, 2.0},
                new double[] {2.0, 3.0},
                new double[] {3.0, 4.0},
                new double[] {4.0, 5.0}
            };
            _mockValidationService.Setup(v => v.ValidateShowAreaUnderCurveInput(expression, min, max, step)).Returns((Error)null);
            var mockWrapperResult = new FSharpIntegratorResult(area, vertices, null);
            _mockIntegratorWrapper.Setup(w => w.CalculateAreaUnderCurve(expression, min, max, step))
                                  .Returns(mockWrapperResult);

            _mockTrapeziumManager.Setup(m => m.CreateTrapezium(1.0, 2.0, 2.0, 3.0, 3.0, 4.0, 4.0, 5.0));
            var mockLineSeries = new LineSeries();
            var mockLineSeriesList = new List<LineSeries> { mockLineSeries };
            _mockTrapeziumManager.Setup(m => m.GetAllTrapeziumSeries()).Returns(mockLineSeriesList);
            _mockOxyPlotManager.Setup(m => m.AddSeriesToPlotModel(plotModel, mockLineSeries));
            Plot testPlot = new Plot(expression, min, max, step);   
            // --------
            // ACT
            // --------
            var result = _integrationService.ShowAreaUnderCurve(plotModel, testPlot, step);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Area, Is.EqualTo(area), "Areas are not equal, but should be");
            Assert.IsFalse(result.HasError, "Has error flag must be false");
            Assert.IsNull(result.Error, "Error must be null");
            _mockIntegratorWrapper.VerifyAll();
            _mockOxyPlotManager.VerifyAll();
            _mockTrapeziumManager.VerifyAll();
            _mockValidationService.VerifyAll();
        }

        [Test]
        public void Test_IntegrationService_ShowAreaUnderCurve_UnhappyPath_ValidatorError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string expression = "1";
            double min = 1, max = 10, step = 0.1;
            Error testError = new Error("Boom");
            _mockValidationService.Setup(v => v.ValidateShowAreaUnderCurveInput(expression, min, max, step)).Returns(testError);
            Plot testPlot = new Plot(expression, min, max, step);

            // --------
            // ACT
            // --------
            var result = _integrationService.ShowAreaUnderCurve(plotModel, testPlot, step);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Area, Is.EqualTo(0), "Area must equal to 0");
            Assert.IsTrue(result.HasError, "Has error flag must be true");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors must match");
            _mockValidationService.VerifyAll();
        }

        [Test]
        public void Test_IntegrationService_ShowAreaUnderCurve_UnhappyPath_WrapperError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string expression = "1";
            double area = 100, min = 1, max = 10, step = 0.1;
            Error testError = new Error("No vertices found for given input, adjust min, max or step size.");
            _mockValidationService.Setup(v => v.ValidateShowAreaUnderCurveInput(expression, min, max, step)).Returns((Error)null);
            double[][] vertices = new double[][]
            {
                
            };
            var mockWrapperResult = new FSharpIntegratorResult(area, vertices, null);
            _mockIntegratorWrapper.Setup(w => w.CalculateAreaUnderCurve(expression, min, max, step))
                                 .Returns(mockWrapperResult);

            Plot testPlot = new Plot(expression, min, max, step);
            // --------
            // ACT
            // --------
            var result = _integrationService.ShowAreaUnderCurve(plotModel, testPlot, step);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Area, Is.EqualTo(0), "Area must equal to 0");
            Assert.IsTrue(result.HasError, "Has error flag must be true");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors must match");
            _mockValidationService.VerifyAll();
            _mockIntegratorWrapper.VerifyAll();
        }

        [Test]
        public void Test_IntegrationService_ShowAreaUnderCurve_UnhappyPath_NotEnoughVerticesError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string expression = "1";
            double min = 1, max = 10, step = 0.1;
            Error testError = new Error("Not enough vertices to form a trapezium, please adjust your step size.");
            double[][] vertices = new double[][]
            {
                new double[] {1.0, 2.0},
                new double[] {2.0, 3.0},
                new double[] {3.0, 4.0},
            };
            _mockValidationService.Setup(v => v.ValidateShowAreaUnderCurveInput(expression, min, max, step)).Returns((Error)null);
            var mockWrapperResult = new FSharpIntegratorResult(0, vertices, null);
            _mockIntegratorWrapper.Setup(w => w.CalculateAreaUnderCurve(expression, min, max, step))
                                 .Returns(mockWrapperResult);

            Plot testPlot = new Plot(expression, min, max, step);
            // --------
            // ACT
            // --------
            var result = _integrationService.ShowAreaUnderCurve(plotModel, testPlot, step);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Area, Is.EqualTo(0), "Area must equal to 0");
            Assert.IsTrue(result.HasError, "Has error flag must be true");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors must match");
            _mockValidationService.VerifyAll();
            _mockIntegratorWrapper.VerifyAll();
        }

        [Test]
        public void Test_IntegrationService_ShowAreaUnderCurve_UnhappyPath_NoVerticesError()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotModel plotModel = new PlotModel();
            string expression = "1";
            double min = 1, max = 10, step = 0.1;
            Error testError = new Error("Boom");
            _mockValidationService.Setup(v => v.ValidateShowAreaUnderCurveInput(expression, min, max, step)).Returns((Error)null);
            var mockWrapperResult = new FSharpIntegratorResult(0, null, testError);
            _mockIntegratorWrapper.Setup(w => w.CalculateAreaUnderCurve(expression, min, max, step))
                                 .Returns(mockWrapperResult);

            Plot testPlot = new Plot(expression, min, max, step);
            // --------
            // ACT
            // --------
            var result = _integrationService.ShowAreaUnderCurve(plotModel, testPlot, step);

            // --------
            // ASSERT
            // --------
            Assert.That(result.Area, Is.EqualTo(0), "Area must equal to 0");
            Assert.IsTrue(result.HasError, "Has error flag must be true");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors must match");
            _mockValidationService.VerifyAll();
            _mockIntegratorWrapper.VerifyAll();
        }
    }
}
