using Engine;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Moq;
using FSharpASTNode = Engine.Types.Node;
using FSharpPoints = Microsoft.FSharp.Collections.FSharpList<System.Tuple<double, double>>;

namespace app.Test.Unit
{
    public class FSharpExpressionEvaluatorWrapper_test
    {
        private Mock<Engine.IEvaluator> _mockEngineEvaluator;
        private FSharpEvaluatorWrapper _evaluator;

        [SetUp]
        public void Setup()
        {
            _mockEngineEvaluator = new Mock<Engine.IEvaluator>();
            _evaluator = new FSharpEvaluatorWrapper(_mockEngineEvaluator.Object);
        }

        [Test]
        public void Test_FSharpExpressionEvaluatiorWrapper_Evaluate_EvaluatesSuccessfully()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            SymbolTable sTable = new SymbolTable();
            var sampleAST = FSharpASTNode.NewVariable(expression);
            var pointTuples = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(1.0, 2.0),
                new Tuple<double, double>(3.0, 4.0),
            };
            FSharpPoints fsharpPointsList = ListModule.OfSeq(pointTuples);
            var expectedPoints = new double[][]
            {
                new double [] {1.0, 2.0},
                new double [] {3.0, 4.0},
            };

            var tuple = Tuple.Create("2", fsharpPointsList, sTable.RawSymbolTable, sampleAST);
            var successResult = FSharpResult<Tuple<string, FSharpPoints, Dictionary<string, Types.NumType>, FSharpASTNode>, string>.NewOk(tuple);

            _mockEngineEvaluator.Setup(e => e.Eval(expression, sTable.RawSymbolTable)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.Evaluate(expression, sTable);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.Answer, Is.EqualTo("2"), "Answers don't match");
            Assert.That(result.FSharpAST, Is.EqualTo(sampleAST), "ASTs don't match");
            Assert.That(result.Points, Is.EqualTo(expectedPoints), "Points are not equal");
        }

        [Test]
        public void Test_FSharpExpressionEvaluatiorWrapper_Evaluate_EvaluatesError()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            SymbolTable sTable = new SymbolTable();
            string error = "Boom";
            var errorResult = FSharpResult<Tuple<string, FSharpPoints, Dictionary<string, Types.NumType>, FSharpASTNode>, string>.NewError(error);

            _mockEngineEvaluator.Setup(e => e.Eval(expression, sTable.RawSymbolTable)).Returns(errorResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.Evaluate(expression, sTable);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Error boolean must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo(error), "Error's don't match");
            Assert.That(result.Answer, Is.Null, "Answer must be null");
            Assert.That(result.FSharpAST, Is.Null, "AST must be null");
            Assert.That(result.Points, Is.Null, "Points must be null");
        }
    }
}
