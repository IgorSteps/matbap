namespace app.Test.Unit
{
    public class Tangent_test
    {
        [Test]
        public void Test_Tangent_GetTangentEquation()
        {
            // --------
            // ASSEMBLE
            // --------
            double x = 11, y = 22, slope = 33;
            Tangent tangent = new Tangent(x, y, slope);
            double yIntercept = y - slope * x;

            // --------
            // ACT
            // --------
            string equation = tangent.GetTangentEquation();

            // --------
            // ASSERT
            // --------
            Assert.That(equation, Is.EqualTo("33 * x + " + yIntercept), "Equations don't match");
        }
    }
}
