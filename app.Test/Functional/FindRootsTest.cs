namespace app.Test.Functional
{
    public class FindRootsTest
    {
        /// <summary>
        /// Simulate user entering an expression and clicking Find roots button.
        /// </summary>
        [Test]
        public void Test_FindRoots_Success()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "sin(x)";
            var viewModel = Utils.CreateExpressionViewModel();
            viewModel.Expression = expression;

            // --------
            // ACT
            // --------
            viewModel.FindRootsCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(viewModel.Answer, Is.EqualTo("3.1415926535613847, 6.283185307215893, 9.424777960777265"), "Answer doesn't match expected");
        }

        /// <summary>
        /// Simulate user entering an expression and clicking Find roots button.
        /// </summary>
        [Test]
        public void Test_FindRoots_NoRealRootsError()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "x^2";
            var viewModel = Utils.CreateExpressionViewModel();
            viewModel.Expression = expression;

            // --------
            // ACT
            // --------
            viewModel.FindRootsCmd.Execute(null);

            // --------
            // ASSERT
            // --------
            Assert.That(viewModel.Answer, Is.EqualTo("Error: No real roots found"));
        }
    }
}
