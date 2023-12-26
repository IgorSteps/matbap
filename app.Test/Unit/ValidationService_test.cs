
namespace app.Test.Unit
{
    public class ValidationServiceTest
    {
        [Test]
        public void Test_ValidationService_ValidatePlotInput_XMin()
        {
            // --------
            // ASSEMBLE
            // --------
            double xmin = 10, xmax = 1, xstep = 0.1;
            ValidationService validationService= new ValidationService();
            string expectedErr = "XMin can't be greater than XMax";

            // ----
            // ACT
            // ----
            string err = validationService.ValidatePlotInput(xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(err, Is.EqualTo(expectedErr), "Errors must be the same");
        }

        [Test]
        public void Test_ValidationService_ValidatePlotInput_XStep()
        {
            // --------
            // ASSEMBLE
            // --------
            double xmin = 1, xmax = 10, xstep = 0;
            ValidationService validationService = new ValidationService();
            string expectedErr = "XStep can't be 0";

            // ----
            // ACT
            // ----
            string err = validationService.ValidatePlotInput(xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(err, Is.EqualTo(expectedErr), "Errors must be the same");
        }

        [Test]
        public void Test_ValidationService_ValidateAddTangentInput_X()
        {
            // --------
            // ASSEMBLE
            // --------
            double x= 0;
            ValidationService validationService = new ValidationService();
            string expectedErr = "Tangent X can't be 0";

            // ----
            // ACT
            // ----
            string err = validationService.ValidateAddTangentInput(x);

            // --------
            // ASSERT
            // --------
            Assert.That(err, Is.EqualTo(expectedErr), "Errors must be the same");
        }
    }
}
