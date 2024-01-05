﻿//using Engine;
//using Moq;
//using FSharpASTNode = Engine.Types.Node;
//using Microsoft.Msagl.Drawing;
//using static app.ASTManager;

//namespace app.Test.Unit
//{
//    public class ExpressionEvaluatingService_test
//    {
//        private Mock<IFSharpASTGetterWrapper> _mockASTGetter;
//        private Mock<IValidator> _mockValidator;
//        private Mock<ISymbolTableManager> _mockSymbolTableManager;
//        private Mock<IFSharpEvaluatorWrapper> _mockExpressionEvaluator;
//        private Mock<IExpressionManager> _mockExpressionManager;
//        private Mock<IASTConverter> _mockConverter;
//        private ExpressionEvaluatingService _service;

//        [SetUp]
//        public void Setup()
//        {
//            _mockASTGetter = new Mock<IFSharpASTGetterWrapper>();
//            _mockValidator = new Mock<IValidator>();
//            _mockSymbolTableManager = new Mock<ISymbolTableManager>();
//            _mockExpressionEvaluator = new Mock<IFSharpEvaluatorWrapper>();
//            _mockExpressionManager = new Mock<IExpressionManager>();
//            _mockConverter = new Mock<IASTConverter>();
//            _service = new ExpressionEvaluatingService(
//                _mockASTGetter.Object,
//                _mockValidator.Object,
//                _mockSymbolTableManager.Object,
//                _mockExpressionEvaluator.Object,
//                _mockExpressionManager.Object,
//                _mockConverter.Object
//                );
//        }

//        [Test]
//        public void Test_ExpressionEvaluatingService_EvaluatesSuccessfuly()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "1+1";
//            string expectedAnswer = "2";
//            Expression expression = new Expression( testInput );
//            SymbolTable testSymTable = new SymbolTable();

//            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);
//            _mockSymbolTableManager.Setup(m => m.GetSymbolTable()).Returns(testSymTable);

//            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(expectedAnswer, null);
//            _mockExpressionEvaluator.Setup(e => e.Evaluate(testInput, testSymTable)).Returns(mockResult);

//            // --------
//            // ACT
//            // --------
//            var result = _service.Evaluate(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsFalse(result.HasError, "Must be false");
//            Assert.That(result.Error, Is.Null, "Error must be null");
//            Assert.That(expectedAnswer, Is.EqualTo(result.Result), "Answers don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }

//        [Test]
//        public void Test_ExpressionEvaluatingService_EvaluatesError()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "1+1";
//            Error testError = new Error("Test error");
//            Expression expression = new Expression(testInput);
//            SymbolTable testSymTable = new SymbolTable();

//            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);
//            _mockSymbolTableManager.Setup(m => m.GetSymbolTable()).Returns(testSymTable);

//            FSharpEvaluationResult mockResult = new FSharpEvaluationResult(null, testError);
//            _mockExpressionEvaluator.Setup(e => e.Evaluate(testInput, testSymTable)).Returns(mockResult);

//            // --------
//            // ACT
//            // --------
//            var result = _service.Evaluate(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsTrue(result.HasError, "Must be true");
//            Assert.That(result.Error, Is.Not.Null, "Error must noy be null");
//            Assert.That(testError, Is.EqualTo(result.Error), "Errors don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }

//        [Test]
//        public void Test_ExpressionEvaluatingService_DifferentiateSuccess()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "x";
//            Expression expression = new Expression(testInput);
//            var ast = FSharpASTNode.NewVariable(testInput);
//            expression.FSharpAST = ast;

//            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);

//            var mockResult = new FSharpDifferentiationResult(ast, null);
//            _mockExpressionManager.Setup(m => m.Differentiate(expression)).Returns(mockResult);
//            var testCSNode = new VariableNode(testInput);
//            var mockConverterResult = new ConvertionResult(testCSNode, null);
//            _mockConverter.Setup(c => c.Convert(expression.FSharpAST)).Returns(mockConverterResult);
//            _mockConverter.Setup(c => c.ConvertToString(testCSNode)).Returns(testInput);
            
//            // --------
//            // ACT
//            // --------
//            var result = _service.Differentiate(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsFalse(result.HasError, "Must be false");
//            Assert.That(result.Error, Is.Null, "Error must be null");
//            Assert.That(result.Result, Is.EqualTo(testInput), "Answers don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }

//        [Test]
//        public void Test_ExpressionEvaluatingService_DifferentiateError()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "x";
//            Expression expression = new Expression(testInput);
//            var testError = new Error("test");

//            _mockExpressionManager.Setup(m => m.CreateExpression(testInput)).Returns(expression);

//            var mockResult = new FSharpDifferentiationResult(null, testError);
//            _mockExpressionManager.Setup(m => m.Differentiate(expression)).Returns(mockResult);

//            // --------
//            // ACT
//            // --------
//            var result = _service.Differentiate(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsTrue(result.HasError, "Must be true");
//            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
//            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }


//        [Test]
//        public void Test_ExpressionEvaluatingService_VisualiseASTHappy()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "1+1";
//            var graph = new Graph();
//            graph.AddNode("x");
//            Graph expectedGraph = graph;
//            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns((Error)null);
//            var fsharpNode = FSharpASTNode.NewVariable("x");
//            var mockedResult = new FSharpASTGetterResult(fsharpNode, null);
//            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedResult);
//            var csharpNode = new VariableNode("x");
//            var mockConverterResult = new ConvertionResult(csharpNode, null);
//            _mockConverter.Setup(c => c.Convert(fsharpNode)).Returns(mockConverterResult);
            
//            _mockConverter.Setup(c => c.ConvertAstToGraph(csharpNode)).Returns(graph);

//            // --------
//            // ACT
//            // --------
//            var result = _service.VisualiseAST(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsFalse(result.HasError, "Must be false");
//            Assert.That(result.Error, Is.Null, "Error must be null");
//            Assert.That(expectedGraph, Is.EqualTo(result.AST), "Graphs don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }

//        [Test]
//        public void Test_ExpressionEvaluatingService_VisualiseASTError()
//        {
//            // --------
//            // ASSEMBLE
//            // --------
//            string testInput = "1+1";
//            var graph = new Graph();
//            graph.AddNode("x");
//            Graph expectedGraph = graph;
//            _mockValidator.Setup(v => v.ValidateExpressionInputIsNotNull(testInput)).Returns((Error)null);
//            var fsharpNode = FSharpASTNode.NewVariable("x");
//            var mockedResult = new FSharpASTGetterResult(fsharpNode, null);
//            _mockASTGetter.Setup(g => g.GetAST(testInput)).Returns(mockedResult);

//            var testError = new Error("boom");
//            var expectedResult = new ConvertionResult(null, testError);
//            _mockConverter.Setup(c => c.Convert(fsharpNode)).Returns(expectedResult);

//            // --------
//            // ACT
//            // --------
//            var result = _service.VisualiseAST(testInput);

//            // --------
//            // ASSERT
//            // --------
//            Assert.IsTrue(result.HasError, "Must be true");
//            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
//            Assert.That(result.Error.Message, Is.EqualTo(testError.Message), "Errors don't match");
//            _mockExpressionManager.VerifyAll();
//            _mockSymbolTableManager.VerifyAll();
//            _mockExpressionEvaluator.VerifyAll();
//        }
//    }
//}
