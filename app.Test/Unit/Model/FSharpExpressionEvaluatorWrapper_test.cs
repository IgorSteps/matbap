using Engine;
using Microsoft.FSharp.Core;
using Moq;
using System;

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
            var tuple = Tuple.Create("2", sTable.Table);
            var successResult = FSharpResult<Tuple<string, Dictionary<string, Parser.NumType>>, string>.NewOk(tuple);

            _mockEngineEvaluator.Setup(e => e.Eval(expression, sTable.Table)).Returns(successResult);

            // -----
            // ACT
            // -----
            var result = _evaluator.Evaluate(expression, sTable);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Error, Is.Null, "Error must be null");
            Assert.That(result.EvaluationResult, Is.EqualTo("2"), "Answers don't match");
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
            var errorResult = FSharpResult<Tuple<string, Dictionary<string, Parser.NumType>>, string>.NewError(error);

            _mockEngineEvaluator.Setup(e => e.Eval(expression, sTable.Table)).Returns(errorResult);

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
            Assert.That(result.EvaluationResult, Is.Null, "Answer must be null");
        }
    }
}
