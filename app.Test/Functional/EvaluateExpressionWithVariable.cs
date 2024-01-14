using Engine;

namespace app.Test.Functional
{
    public class EvaluateExpressionWithVariable
    {
        /// <summary>
        /// Test to simualte user entering math expression with variables, and GUI Symbol table showing them.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithVariable()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "x=5";

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("10"), "Responses don't match");
            Assert.That(viewModel.GUISymbolTable.Count, Is.EqualTo(1), "GUI Symbol table must have an entry");
            var kv = new KeyValuePair<string, Engine.Types.NumType>("x", Engine.Types.NumType.NewInt(5));
            Assert.That(viewModel.GUISymbolTable.Contains(kv), "GUI Symbol table is missing a key value pair");
        }

        /// <summary>
        /// Test to simualte user entering math expression with 2 variables, and GUI Smybol table reflecting it.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_WithMultipleVariables()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "x=5";

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;
            string nextVariable = "y=5";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("10"), "Responses don't match");
            Assert.That(viewModel.GUISymbolTable.Count, Is.EqualTo(2), "GUI Symbol table must have 2 entries");
            var kv = new KeyValuePair<string, Engine.Types.NumType>("x", Engine.Types.NumType.NewInt(5));
            var kv2 = new KeyValuePair<string, Engine.Types.NumType>("y", Engine.Types.NumType.NewInt(5));
            Assert.That(viewModel.GUISymbolTable.Contains(kv), "GUI Symbol table is missing a key value pair");
            Assert.That(viewModel.GUISymbolTable.Contains(kv2), "GUI Symbol table is missing a key value pair");
        }

        /// <summary>
        /// Test to simualte user entering math expression and reassinging variables, and GUI Smybol table reflecting it.
        /// </summary>
        [Test]
        public void Test_EvaluateExpression_ReassignVariables()
        {
            // --------
            // ASSEMBLE
            // --------
            var expression = "x=5";

            var viewModel = Utils.CreateExpressionViewModel();

            viewModel.Expression = expression;
            string nextVariable = "x=10";
            string nextExpression = "5+x";

            // ---
            // ACT
            // ---
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextVariable;
            viewModel.EvaluateCmd.Execute(null);

            viewModel.Expression = nextExpression;
            viewModel.EvaluateCmd.Execute(null);


            // ------
            // ASSERT
            // ------
            Assert.That(viewModel.Answer, Is.EqualTo("15"), "Responses don't match");
            Assert.That(viewModel.GUISymbolTable.Count, Is.EqualTo(1), "GUI Symbol table must have 1 entry");
            var kv = new KeyValuePair<string, Engine.Types.NumType>("x", Engine.Types.NumType.NewInt(10));
            Assert.That(viewModel.GUISymbolTable.Contains(kv), "GUI Symbol table is missing a key value pair");
        }
    }
}
