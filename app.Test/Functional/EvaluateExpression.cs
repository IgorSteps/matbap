namespace app.Test.Functional
{
    public class EvaluateExpression
    {
        /// <summary>
        /// Test to simualte user entering a math expression and getting an answer back.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_ReturnType()
        {
            // --------
            // ASSEMBLE
            // --------
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            viewModel.Expression = "1+1";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            // We check F# engine returns a string to make sure our GUI output is a clear string.
            Assert.That(viewModel.Response, Is.EqualTo("2"), "F# engine returned a value C# can't understand");
        }

        /// <summary>
        /// Test to simualte user entering a math expression with new lines and getting an answer back.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithNewLine()
        {
            // --------
            // ASSEMBLE
            // --------
            var interpreter = new InterpretationModel();
            var viewModel = new InterpretationViewModel(interpreter);
            // Same as 
            // x=1;
            // y=1;
            // x+y
            viewModel.Expression = "x=1;\r\ny=1;\r\nx+y";

            // ---
            // ACT
            // ---
            viewModel.InterpretCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Response, Is.EqualTo("2"), "Expression with new lines has returned wrong answer");
        }
    }
}
