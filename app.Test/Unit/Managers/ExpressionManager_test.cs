namespace app.Test.Unit
{
    public class ExpressionManager_test
    {
        [Test]
        public void Test_ExpressionManager_CreateExpression()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";
            Expression expected = new Expression(expression);
            ExpressionManager manager = new ExpressionManager();

            // ----
            // ACT
            // ----
            Expression actual = manager.CreateExpression(expression);


            // ------
            // ASSERT
            // ------
            Assert.That(actual.Value, Is.EqualTo(expression), "Expressions don't match");
        }
    }
}
