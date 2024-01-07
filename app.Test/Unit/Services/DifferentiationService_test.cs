using Moq;
using FSharpASTNode = Engine.Types.Node;
using static app.ASTManager;
using QuickGraph.Algorithms.Services;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace app.Test.Unit
{
    public class DifferentiationService_test
    {
        private Mock<IFSharpASTGetterWrapper> _mockASTGetter;
        private Mock<IValidator> _mockValidator;
        private Mock<IASTConverter> _mockConverter;
        private Mock<IExpressionManager> _mockExpressionManager;

        private DifferentiationService _differentiationService;

        [SetUp]
        public void Setup()
        {
            _mockASTGetter = new Mock<IFSharpASTGetterWrapper>();
            _mockValidator = new Mock<IValidator>();
            _mockExpressionManager = new Mock<IExpressionManager>();
            _mockConverter = new Mock<IASTConverter>();
            _differentiationService = new DifferentiationService(
                _mockASTGetter.Object,
                _mockValidator.Object,
                _mockExpressionManager.Object,
                _mockConverter.Object
                );
        }

        [Test]
        public void Test_DifferentiatonService_DifferentiateHappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression testExpression = new Expression(testInput);
            var testAST = FSharpASTNode.NewVariable(testInput);
            testExpression.FSharpAST = testAST;
            var mockedGetterResult = new FSharpASTGetterResult(testAST, null);
            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(testExpression);
            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedGetterResult);

            var mockResult = new FSharpDifferentiationResult(testAST, null);
            _mockExpressionManager.Setup(m => m.Differentiate(testExpression)).Returns(mockResult);
            var testCSNode = new VariableNode(testInput);
            var mockConverterResult = new ConvertionResult(testCSNode, null);
            _mockConverter.Setup(c => c.Convert(testExpression.FSharpAST)).Returns(mockConverterResult);
            _mockConverter.Setup(c => c.ConvertToString(testCSNode)).Returns(testInput);

            // --------
            // ACT
            // --------
            var result = _differentiationService.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "Must be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Derivative, Is.EqualTo(testInput), "Answers don't match");
            _mockExpressionManager.VerifyAll();
            _mockConverter.VerifyAll();
        }

        [Test]
        public void Test_DifferentiationService_DifferentiateError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression expression = new Expression(testInput);
            var testError = new Error("test");

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);

            var mockResult = new FSharpDifferentiationResult(null, testError);
            _mockExpressionManager.Setup(m => m.Differentiate(expression)).Returns(mockResult);

            // --------
            // ACT
            // --------
            var result = _differentiationService.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
            _mockExpressionManager.VerifyAll();
            _mockConverter.VerifyAll();
        }

        [Test]
        public void Test_DifferentiationService_ValidateError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression expression = new Expression(testInput);
            var testError = new Error("test");
            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns(testError);
           
            // --------
            // ACT
            // --------
            var result = _differentiationService.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
            _mockValidator.VerifyAll();
        }

        [Test]
        public void Test_DifferentiationService_GetASTError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression testExpression = new Expression(testInput);
          
            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns((Error)null);

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(testExpression);
           
            var testError = new Error("test");
            var mockedGetterResult = new FSharpASTGetterResult(null, testError);
            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedGetterResult);

            // --------
            // ACT
            // --------
            var result = _differentiationService.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
            _mockValidator.VerifyAll();
        }

        [Test]
        public void Test_DifferentiationService_ConvertASTError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression testExpression = new Expression(testInput);
            var testAST = FSharpASTNode.NewVariable(testInput);
            testExpression.FSharpAST = testAST;
            var mockedGetterResult = new FSharpASTGetterResult(testAST, null);
            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(testExpression);
            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedGetterResult);

            var mockResult = new FSharpDifferentiationResult(testAST, null);
            _mockExpressionManager.Setup(m => m.Differentiate(testExpression)).Returns(mockResult);
            var testError = new Error("test error");
            var mockConverterResult = new ConvertionResult(null, testError);
            _mockConverter.Setup(c => c.Convert(testExpression.FSharpAST)).Returns(mockConverterResult);

            // --------
            // ACT
            // --------
            var result = _differentiationService.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
            _mockValidator.VerifyAll();
        }
    }
}
