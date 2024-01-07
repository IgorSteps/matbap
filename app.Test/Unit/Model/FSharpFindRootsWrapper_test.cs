using Microsoft.FSharp.Core;
using Moq;

namespace app.Test.Unit
{
    public class FSharpFindRootsWrapper_test
    {
        private Mock<Engine.IRootFinder> _rootFinder;
        private FSharpFindRootsWrapper _wrapper;

        [SetUp]
        public void Setup()
        {
            _rootFinder = new Mock<Engine.IRootFinder>();
            _wrapper = new FSharpFindRootsWrapper(_rootFinder.Object);
        }

        [Test]
        public void Test_FSharpFindRootsWrapper_HappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testExpression = "x";
            double xmin = 1.0, xmax = 2.0;
            var testArray = new double[]
            {
                1.0, 2.0, 3.0
            };
            var mockedResult = FSharpResult<double[], string>.NewOk(testArray);
            _rootFinder.Setup(r => r.FindRoots(xmin, xmax, testExpression)).Returns(mockedResult);
            var expectedRootsString = "1, 2, 3";

            // --------
            // ACT
            // --------
            var actual = _wrapper.FindRoots(testExpression, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.That(actual.Roots, Is.EqualTo(expectedRootsString), "Roots don't match");
            Assert.IsNull(actual.Error, "Error must be null");
            Assert.IsFalse(actual.HasError, "HasError flag must be false");
        }

        [Test]
        public void Test_FSharpFindRootsWrapper_UnhappyPath()
        {
            // --------
            // ASSEMBLE
            // --------
            string testExpression = "x";
            double xmin = 1.0, xmax = 2.0;
            string testError = "boom";
            var mockedResult = FSharpResult<double[], string>.NewError(testError);
            _rootFinder.Setup(r => r.FindRoots(xmin, xmax, testExpression)).Returns(mockedResult);

            // --------
            // ACT
            // --------
            var actual = _wrapper.FindRoots(testExpression, xmin, xmax);

            // --------
            // ASSERT
            // --------
            Assert.IsNull(actual.Roots, "Roots must be null");
            Assert.IsTrue(actual.HasError, "HasError flag must be true");
            Assert.That(actual.Error.Message, Is.EqualTo(testError) ,"Error messages don't match");
        }
    }
}
