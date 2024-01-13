namespace app.Test.Unit
{
    public class Expression_test
    {
        [Test]
        public void Test_Expression_Create()
        {
            // --------
            // ASSEMBLE
            // --------
            string expression = "1+1";

            // ----
            // ACT
            // ----
            Expression actual = new Expression(expression);

            // ------
            // ASSERT
            // ------
            Assert.That(actual.Value, Is.EqualTo(expression), "Expressions don't match");
        }
    }
}
