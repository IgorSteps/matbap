using Moq;
using FSharpASTNode = Engine.Types.Node;

namespace app.Test.Unit
{
    public class ExpressionEvaluatingService_test
    {
        private Mock<ISymbolTableManager> _mockSymbolTableManager;
        private Mock<IFSharpEvaluatorWrapper> _mockExpressionEvaluator;
        private Mock<IExpressionManager> _mockExpressionManager;
        private Mock<IASTConverter> _mockConverter;
        private ExpressionEvaluatingService _service;

        [SetUp]
        public void Setup()
        {
            _mockSymbolTableManager = new Mock<ISymbolTableManager>();
            _mockExpressionEvaluator = new Mock<IFSharpEvaluatorWrapper>();
            _mockExpressionManager = new Mock<IExpressionManager>();
            _mockConverter = new Mock<IASTConverter>();
            _service = new ExpressionEvaluatingService(
                _mockSymbolTableManager.Object,
                _mockExpressionEvaluator.Object,
                _mockExpressionManager.Object,
                _mockConverter.Object
                );
        }

        [Test]
        public void Test_ExpressionEvaluatingService_EvaluatesSuccessfuly()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            string expectedAnswer = "2";
            Expression expression = new Expression( testInput );
            SymbolTable testSymTable = new SymbolTable();

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);
            _mockSymbolTableManager.Setup(m => m.GetSymbolTable()).Returns(testSymTable);

            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(expectedAnswer, null);
            _mockExpressionEvaluator.Setup(e => e.Evaluate(testInput, testSymTable)).Returns(mockResult);

            // --------
            // ACT
            // --------
            var result = _service.Evaluate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "Must be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(expectedAnswer, Is.EqualTo(result.Result), "Answers don't match");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }

        [Test]
        public void Test_ExpressionEvaluatingService_EvaluatesError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            Error testError = new Error("Test error");
            Expression expression = new Expression(testInput);
            SymbolTable testSymTable = new SymbolTable();

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);
            _mockSymbolTableManager.Setup(m => m.GetSymbolTable()).Returns(testSymTable);

            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(null, testError);
            _mockExpressionEvaluator.Setup(e => e.Evaluate(testInput, testSymTable)).Returns(mockResult);

            // --------
            // ACT
            // --------
            var result = _service.Evaluate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must noy be null");
            Assert.That(testError, Is.EqualTo(result.Error), "Errors don't match");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }

        [Test]
        public void Test_ExpressionEvaluatingService_DifferentiateSuccess()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "x";
            Expression expression = new Expression(testInput);
            var ast = FSharpASTNode.NewVariable(testInput);
            expression.FSharpAST = ast;

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);

            var mockResult = new FSharpDifferentiationResult(ast, null);
            _mockExpressionManager.Setup(m => m.Differentiate(expression)).Returns(mockResult);
            var mockConverterResult = new VariableNode(testInput);
            _mockConverter.Setup(c => c.Convert(expression.FSharpAST)).Returns(mockConverterResult);
            _mockConverter.Setup(c => c.ConvertToString(mockConverterResult)).Returns(testInput);
            
            // --------
            // ACT
            // --------
            var result = _service.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "Must be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Result, Is.EqualTo(testInput), "Answers don't match");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }

        [Test]
        public void Test_ExpressionEvaluatingService_DifferentiateError()
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
            var result = _service.Differentiate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }
    }
}
