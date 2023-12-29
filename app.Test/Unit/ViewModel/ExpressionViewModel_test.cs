using Moq;

namespace app.Test.Unit
{
    public class Tests
    {
        private app.ExpressionViewModel _viewModel;
        private Mock<IEvaluator> _evaluatorMock;

        [SetUp]
        public void Setup()
        {
            _evaluatorMock = new Mock<IEvaluator>();
            _viewModel = new ExpressionViewModel(_evaluatorMock.Object);
        }

        [Test]
        public void Test_ExpressionViewModel_EvaluateCmd_ReceiveAnswerFromEvaluator()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "123";
            var mockResponse = new ExpressionEvaluatingServiceResult(testInput, null);
            _viewModel.Expression = testInput;
            _evaluatorMock.Setup(i => i.Evaluate(testInput)).Returns(mockResponse);

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
            var mockResponse = new ExpressionEvaluatingServiceResult(null, testError);
            _viewModel.Expression = "123";
            _evaluatorMock.Setup(i => i.Evaluate(_viewModel.Expression)).Returns(mockResponse);

            // ---
            // ACT
            // ---
            _viewModel.EvaluateCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(_viewModel.Answer, Is.EqualTo(testError.ToString()), "Actual response is not equal expected");
        }
    }
}