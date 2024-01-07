using Moq;
using FSharpASTNode = Engine.Types.Node;
using Microsoft.Msagl.Drawing;
using static app.ASTManager;

namespace app.Test.Unit
{
    public class ExpressionEvaluatingService_test
    {
        private Mock<IFSharpASTGetterWrapper> _mockASTGetter;
        private Mock<IValidator> _mockValidator;
        private Mock<ISymbolTableManager> _mockSymbolTableManager;
        private Mock<IFSharpEvaluatorWrapper> _mockExpressionEvaluator;
        private Mock<IExpressionManager> _mockExpressionManager;
        private Mock<IASTConverter> _mockConverter;
        private ExpressionEvaluatingService _service;
        [SetUp]
        public void Setup()
        {
            _mockASTGetter = new Mock<IFSharpASTGetterWrapper>();
            _mockValidator = new Mock<IValidator>();
            _mockSymbolTableManager = new Mock<ISymbolTableManager>();
            _mockExpressionEvaluator = new Mock<IFSharpEvaluatorWrapper>();
            _mockExpressionManager = new Mock<IExpressionManager>();
            _mockConverter = new Mock<IASTConverter>();
            _service = new ExpressionEvaluatingService(
                _mockASTGetter.Object,
                _mockValidator.Object,
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
            Expression expression = new Expression(testInput);
            SymbolTable testSymTable = new SymbolTable();

            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);
            _mockSymbolTableManager.Setup(m => m.GetSymbolTable()).Returns(testSymTable);
            double[][] points = new double[][]
            {
                new double[] {1.0, 2.0},
                new double[] {3.0, 4.0},
            };
            var ast = FSharpASTNode.NewVariable(testInput);
            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(expectedAnswer, points, ast, null);
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
            Assert.That(ast, Is.EqualTo(result.Expression.FSharpAST), "AST don't equal");
            Assert.That(points, Is.EqualTo(result.Expression.Points), "Points don't equal");
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

            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(null, null, null, testError);
            _mockExpressionEvaluator.Setup(e => e.Evaluate(testInput, testSymTable)).Returns(mockResult);

            // --------
            // ACT
            // --------
            var result = _service.Evaluate(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsTrue(result.HasError, "Must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(testError, Is.EqualTo(result.Error), "Errors don't match");
            Assert.That(result.Expression, Is.Null, "Expression must be null");
            Assert.That(result.Result, Is.Null, "Answer must be null");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }

        [Test]
        public void Test_ExpressionEvaluatingService_VisualiseASTHappy()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            var graph = new Graph();
            graph.AddNode("x");
            Graph expectedGraph = graph;
            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns((Error)null);
            var fsharpNode = FSharpASTNode.NewVariable("x");
            var mockedResult = new FSharpASTGetterResult(fsharpNode, null);
            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedResult);
            var csharpNode = new VariableNode("x");
            var mockConverterResult = new ConvertionResult(csharpNode, null);
            _mockConverter.Setup(c => c.Convert(fsharpNode)).Returns(mockConverterResult);

            _mockConverter.Setup(c => c.ConvertAstToGraph(csharpNode)).Returns(graph);

            // --------
            // ACT
            // --------
            var result = _service.VisualiseAST(testInput);

            // --------
            // ASSERT
            // --------
            Assert.IsFalse(result.HasError, "Must be false");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(expectedGraph, Is.EqualTo(result.AST), "Graphs don't match");
            _mockExpressionManager.VerifyAll();
            _mockSymbolTableManager.VerifyAll();
            _mockExpressionEvaluator.VerifyAll();
        }

        [Test]
        public void Test_ExpressionEvaluatingService_VisualiseASTError()
        {
            // --------
            // ASSEMBLE
            // --------
            string testInput = "1+1";
            var graph = new Graph();
            graph.AddNode("x");
            Graph expectedGraph = graph;
            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns((Error)null);
            var fsharpNode = FSharpASTNode.NewVariable("x");
            var mockedResult = new FSharpASTGetterResult(fsharpNode, null);
            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedResult);

            var testError = new Error("boom");
            var expectedResult = new ConvertionResult(null, testError);
            _mockConverter.Setup(c => c.Convert(fsharpNode)).Returns(expectedResult);

            // --------
            // ACT
            // --------
            var result = _service.VisualiseAST(testInput);

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
