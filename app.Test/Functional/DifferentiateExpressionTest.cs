namespace app.Test.Functional
{
    public class DifferentiateExpressionTest
    {
        /// <summary>
        /// Test to simualte user entering a math expression and differentiating it successfuly.
        /// </summary>
        [Test]
        public void Test_DifferentiateExpression_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            
            var expression = "x^2";

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;

            // ---
            // ACT
            // ---
            viewModel.DifferentiateCmd.Execute(null);

            // ------
            // ASSERT
            // ------ 
            // We check F# engine returns a string to make sure our GUI output is a clear string.
            Assert.That(viewModel.Answer, Is.EqualTo("2*x^1"), "Answer doesn't match expected");
        }
    }
}
  