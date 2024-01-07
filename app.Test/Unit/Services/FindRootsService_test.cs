using Moq;

namespace app.Test.Unit
{
    public class FindRootsService_test
    {
        private Mock<IFSharpFindRootsWrapper> _mockFSharpFindRootsWrapper;
        private Mock<IValidator> _mockValidator;
        private FindRootsService _service;

        [SetUp]
        public void Setup()
        {
            _mockFSharpFindRootsWrapper = new Mock<IFSharpFindRootsWrapper>();
            _mockValidator = new Mock<IValidator>();
            _service = new FindRootsService(_mockFSharpFindRootsWrapper.Object, _mockValidator.Object);
        }

        [Test]
        public void Test_FindRootsService_HappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x^2";
            string expectedRoots = "1";
            double xmin = 1, xmax = 10;
            _mockValidator.Setup(v => v.ValidateFindRootsInput(testInput, xmin, xmax)).Returns((Error)null);
            var mockedResult = new FSharpFindRootResult(expectedRoots, null);
            _mockFSharpFindRootsWrapper.Setup(w => w.FindRoots(testInput, xmin, xmax)).Returns(mockedResult);

            // --------
            // ACT
            // --------
            var actual = _service.FindRoots(testInput, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(actual.HasError, "HasError flag must be false");
            Assert.IsNull(actual.Error, "Error must be null");
            Assert.That(expectedRoots, Is.EqualTo(actual.Roots), "Expected roots don't match actual roots");
            _mockValidator.VerifyAll();
            _mockFSharpFindRootsWrapper.VerifyAll();
        }

        [Test]
        public void Test_FindRootsService_ValidorUnhappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x^2";
            double xmin = 1, xmax = 10;
            Error testError = new Error("test error");
            _mockValidator.Setup(v => v.ValidateFindRootsInput(testInput, xmin, xmax)).Returns(testError);

            // --------
            // ACT
            // --------
            var actual = _service.FindRoots(testInput, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(actual.HasError, "HasError flag must be true");
            Assert.That(actual.Error.Message, Is.EqualTo(testError.Message), "Error messages don't match");
            Assert.IsNull(actual.Roots, "Actual roots must be null");
            _mockValidator.VerifyAll();
            _mockFSharpFindRootsWrapper.VerifyAll();
        }

        [Test]
        public void Test_FindRootsService_FSharpWrapperUnhappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x^2";
            double xmin = 1, xmax = 10;
            Error testError = new Error("test error");
            _mockValidator.Setup(v => v.ValidateFindRootsInput(testInput, xmin, xmax)).Returns((Error)null);
            var mockedResult = new FSharpFindRootResult(null, testError);
            _mockFSharpFindRootsWrapper.Setup(w => w.FindRoots(testInput, xmin, xmax)).Returns(mockedResult);

            // --------
            // ACT
            // --------
            var actual = _service.FindRoots(testInput, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(actual.HasError, "HasError flag must be true");
            Assert.That(actual.Error.Message, Is.EqualTo(testError.Message), "Error messages don't match");
            Assert.IsNull(actual.Roots, "Actual roots must be null");
            _mockValidator.VerifyAll();
            _mockFSharpFindRootsWrapper.VerifyAll();
        }
    }
}
