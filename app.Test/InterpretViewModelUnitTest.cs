using Moq;

namespace app.Test
{
    public class Tests
    {
        private app.InterpretationViewModel _viewModel;
        private Mock<IInterpretModel> _modelMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IInterpretModel>();
            _viewModel = new InterpretationViewModel(_modelMock.Object);
        }

        [Test]
        public void InterpretCommand_UpdatesResponseWithModelResponse()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "123";
            var expectedResponse = "123";
            _viewModel.Expression = testInput;
            _modelMock.Setup(i => i.Interpret(testInput)).Returns(expectedResponse);

            // ---
            // ACT
            // ---
            _viewModel.InterpretCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(_viewModel.Response, Is.EqualTo(expectedResponse), "Actual response is not equal expected");
        }
    }
}