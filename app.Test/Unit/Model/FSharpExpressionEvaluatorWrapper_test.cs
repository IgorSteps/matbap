namespace app.Test.Unit
{
    public class FSharpExpressionEvaluatorWrapper_test
    {
        [Test]
        public void Test_FSharpExpressionEvaluatiorWrapper_Evaluate_EvaluatesSuccessfully()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            var expressionEvaluator = new FSharpEvaluatorWrapper();
            SymbolTable sTable = new SymbolTable();
            // -----
            // ACT
            // -----
            var result = expressionEvaluator.Evaluate(expression, sTable);

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
            string expression = "1++1"; // Invalid.
            var expressionEvaluator = new FSharpEvaluatorWrapper();
            SymbolTable sTable = new SymbolTable();

            // -----
            // ACT
            // -----
            var result = expressionEvaluator.Evaluate(expression, sTable);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Error boolean must be true");
            Assert.That(result.Error, Is.Not.Null, "Error must not be null");
            Assert.That(result.Error.Message, Is.EqualTo("Error while parsing: Unexpected token or end of expression"), "Error's don't match");
            Assert.That(result.EvaluationResult, Is.Null, "Answer must be null");
        }
    }
}
