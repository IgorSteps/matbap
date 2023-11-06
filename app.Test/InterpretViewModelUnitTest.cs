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
        public void InterpretModeView_InterpretCommand_UpdatesResponseWithModelResponse()
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

        [Test]
        public void InterpretModeView_SettingExpression_PropertyChangedRaised()
        {
            // --------
            // ASSEMBLE
            // --------
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) => 
            { 
                if (args.PropertyName == "Expression") 
                    eventRaised = true; 
            };

            // ---
            // ACT
            // ---
            _viewModel.Expression = "new expression";

            // ------
            // ASSERT
            // ------
            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void InterpretModeView_SettingResponse_PropertyChangedRaised()
        {
            // --------
            // ASSEMBLE
            // --------
            var testInput = "123";
            _viewModel.Expression = testInput;
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Response")
                    eventRaised = true;
            };

            // Set private Response by calling Interpret().
            _modelMock.Setup(i => i.Interpret(testInput)).Returns("bla bla");

            // ---
            // ACT
            // ---
            _viewModel.InterpretCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(eventRaised, Is.True);
        }

        [Test]
        public void InterpretModeView_FT()
        {
            // --------
            // ASSEMBLE
            // --------
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "1+1";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            // We check F# engine returns a string to make sure our GUI output is a clear string
            Assert.That(viewModel.Response, Is.EqualTo("2"), "F# engine returned a value C# can't understand");
        }

    }
}