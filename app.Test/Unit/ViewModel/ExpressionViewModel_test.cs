using Moq;
using CommunityToolkit.Mvvm.Messaging;

namespace app.Test.Unit
{
    public class Tests
    {
        private app.ExpressionViewModel _viewModel;
        private Mock<IEvaluator> _evaluatorServiceMock;
        private Mock<IPlotter> _plottingServiceMock;

        [SetUp]
        public void Setup()
        {
            _evaluatorServiceMock = new Mock<IEvaluator>();
            _plottingServiceMock = new Mock<IPlotter>();
            _viewModel = new ExpressionViewModel(_evaluatorServiceMock.Object, _plottingServiceMock.Object);
        }

        [Test]
        public void Test_ExpressionViewModel_EvaluateCmd_ReceiveAnswerFromEvaluator()
        {
            // --------  
            // ASSEMBLE
            // --------
            var testInput = "123";
            Expression expression = new Expression(testInput);
            var mockResponse = new ExpressionEvaluatingServiceResult(testInput, expression, null);
            _viewModel.Expression = testInput;
            _evaluatorServiceMock.Setup(i => i.Evaluate(testInput)).Returns(mockResponse);

            // ---
            // ACT
            // ---
            _viewModel.EvaluateCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(_viewModel.Answer, Is.EqualTo(mockResponse.Result), "Actual response is not equal expected");
        }

        [Test]
        public void Test_ExpressionViewModel_EvaluateCmd_ReceiveErrorFromEvaluator()
        {
            // --------
            // ASSEMBLE
            // --------
            var testError = new Error("error");
            var mockResponse = new ExpressionEvaluatingServiceResult(null, null, testError);
            _viewModel.Expression = "123";
            _evaluatorServiceMock.Setup(i => i.Evaluate(_viewModel.Expression)).Returns(mockResponse);

            // ---
            // ACT
            // ---
            _viewModel.EvaluateCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(_viewModel.Answer, Is.EqualTo(testError.ToString()), "Actual response is not equal expected");
        }

        [Test]
        public void Test_ExpressionViewModel_SendsPlotExpressionMessage()
        {
            // --------  
            // ASSEMBLE
            // --------
            var testInput = "123";
            var samplePoints = new double[][]
            {
                new double [] {1.0, 2.0},
                new double [] {3.0, 4.0},
            };
            Expression expression = new Expression(testInput);
            expression.Points = samplePoints;

            var mockResponse = new ExpressionEvaluatingServiceResult(testInput, expression, null);
            _viewModel.Expression = testInput;
            _evaluatorServiceMock.Setup(i => i.Evaluate(testInput)).Returns(mockResponse);

            // Setup messaging system.
            var receivedMessage = false;
            Expression receivedExpression = null;
            WeakReferenceMessenger.Default.Register<PlotExpressionMessage>(this, (r, m) =>
            {
                receivedMessage = true;
                receivedExpression = m.Expression;
            });

            // ---
            // ACT
            // ---
            _viewModel.EvaluateCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(_viewModel.Answer, Is.Null, "Actual response must be null because we are plotting");
            Assert.IsTrue(receivedMessage, "Message wasn't received");
            Assert.That(receivedExpression, Is.EqualTo(expression), "Sent and received expressions are not equal");
        }
    }
}