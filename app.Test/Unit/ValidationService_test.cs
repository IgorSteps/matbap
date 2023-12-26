using System.Xaml.Schema;

namespace app.Test.Unit
{
    public class ValidationService_test
    {
        const string tangentXZeroMsg = "Tangent's X can't be 0";
        const string xMinGreaterXMax = "XMin can't be greater than XMax";
        const string tangentXRangeMsg = "Tangent's X must be in the range [XMin, XMax]";
        const string xStepZeroMsg = "XStep can't be 0";

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
        [TestCase(0, 1, 10, 0.1, tangentXZeroMsg)]
        [TestCase(2, 10, 0, 0.1, xMinGreaterXMax)]
        [TestCase(200, 1, 10, 0.1, tangentXRangeMsg)]
        [TestCase(2, 1, 10, 0, xStepZeroMsg)]
        public void Test_ValidationService_ValidateAddTangentInput(double x, double xmin, double xmax, double xstep, string error)
        {
            // --------
            // ASSEMBLE
            // --------
            ValidationService validationService = new ValidationService();
            string expectedErr = error;

            // ----
            // ACT
            // ----
            string err = validationService.ValidateAddTangentInput(x, xmin, xmax, xstep);

            // --------
            // ASSERT
            // --------
            Assert.That(err, Is.EqualTo(expectedErr), "Errors must be the same");
        }
    }
}
