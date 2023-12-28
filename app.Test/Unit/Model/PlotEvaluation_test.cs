namespace app.Test.Unit
{
    public class PlotEvaluation_test
    {
        [Test]
        public void Test_FunctionEvaluation_Evaluate_EvaluatesSuccessfully() 
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double xmin = 1, xmax = 10, xstep = 0.1;
            var functionEvaluator = new FunctionEvaluation();
            // -----
            // ACT
            // -----
            var result = functionEvaluator.Evaluate(function, xmin, xmax, xstep);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Points, Is.Not.Null, "Should've returned points");
        }

        [Test]
        public void Test_FunctionEvaluation_Evaluate_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "+1"; // invalid input.
            double xmin = 1, xmax = 10, xstep = 0.1;
            var functionEvaluator = new FunctionEvaluation();
            // -----
            // ACT
            // -----
            var result = functionEvaluator.Evaluate(function, xmin, xmax, xstep);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error.ToString(), Is.EqualTo("Error: Error while parsing: Unexpected token or end of expression"), "Errors don't match");
            Assert.That(result.Points, Is.Null, "Points should be null");
        }

        [Test]
        public void Test_FunctionEvaluation_EvaluateAtPoint_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double x = 1;
            var functionEvaluator = new FunctionEvaluation();

            // -----
            // ACT
            // -----
            var result = functionEvaluator.EvaluateAtPoint(x, function);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.False, "Shouldn't have an error");
            Assert.That(result.Points, Is.Not.Null, "Points shouldn't be null");
            Assert.That(result.Points, Has.Length.EqualTo(1), "Points should have only 1 [x,y] pair");
        }

        [Test]
        public void Test_FunctionEvaluation_EvaluateAtPoint_Error()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "+1"; // invalid input.
            double x = 1;
            var functionEvaluator = new FunctionEvaluation();

            // -----
            // ACT
            // -----
            var result = functionEvaluator.EvaluateAtPoint(x, function);

            // ------
            // ASSERT
            // ------
            Assert.That(result.HasError, Is.True, "Should have an error");
            Assert.That(result.Error.ToString(), Is.EqualTo("Error: Error while parsing: Unexpected token or end of expression"), "Errors don't match");
            Assert.That(result.Points, Is.Null, "Points should be null");
        }

        [Test]
        public void Test_FunctionEvaluation_TakeDerivative()
        {
            // --------
            // ASSEMBLE
            // --------
            string function = "x+1";
            double x = 1;
            var functionEvaluator = new FunctionEvaluation();

            // -----
            // ACT
            // -----
            double d = functionEvaluator.TakeDerivative(x, function);

            // ------
            // ASSERT
            // ------
            Assert.That(d, Is.EqualTo(1).Within(0.000001), "Should be equal");
        }
    }
}
