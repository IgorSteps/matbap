using OxyPlot.Series;

namespace app.Test.Unit
{
    public class TrapeziumManager_test
    {
        [Test]
        public void Test_TrapeziumManager_CreatesAndAddsNewTrapzeium() 
        {
            // --------
            // ASSEMBLE
            // --------
            double x1 = 1, x2 = 2, x3 = 3, x4 = 4;
            double y1 = 1, y2 = 2, y3 = 3, y4 = 4;
            TrapeziumManager trapeziumManager = new TrapeziumManager();
            
            // --------
            // ACT
            // --------
            var trapezium = trapeziumManager.CreateTrapezium(x1, y1, x2, y2, x3, y3, x4, y4);

            // --------
            // ASSERT
            // --------
            Assert.That(trapeziumManager.Trapeziums, Contains.Item(trapezium));
        }

        [Test]
        public void Test_TrapeziumManager_ShouldHaveTwoTrapeziums()
        {
            // --------
            // ASSEMBLE
            // --------
            double x1 = 1, x2 = 2, x3 = 3, x4 = 4;
            double y1 = 1, y2 = 2, y3 = 3, y4 = 4;
            TrapeziumManager trapeziumManager = new TrapeziumManager();
            
            // --------
            // ACT
            // --------
            trapeziumManager.CreateTrapezium(x1, y1, x2, y2, x3, y3, x4, y4);
            trapeziumManager.CreateTrapezium(x1, y1, x2, y2, x3, y3, x4, y4);

            // --------
            // ASSERT
            // --------
            Assert.That(trapeziumManager.Trapeziums.Count, Is.EqualTo(2));
        }

        [Test]
        public void Test_TrapeziumManager_GetTrapeziumSeries_ShouldReturnCorrectNumberOfSeries()
        {
            // --------
            // ASSEMBLE
            // --------
            double x1 = 1, x2 = 2, x3 = 3, x4 = 4;
            double y1 = 1, y2 = 2, y3 = 3, y4 = 4;
            TrapeziumManager trapeziumManager = new TrapeziumManager();
            trapeziumManager.CreateTrapezium(x1, y1, x2, y2, x3, y3, x4, y4);
            trapeziumManager.CreateTrapezium(x1, y1, x2, y2, x3, y3, x4, y4);

            // --------
            // ACT
            // --------
            var series = trapeziumManager.GetAllTrapeziumSeries();

            // --------
            // ASSERT
            // --------
            Assert.That(series.Count, Is.EqualTo(2));
            var firstSeries = series[0] as LineSeries;
            Assert.NotNull(firstSeries);
            // 4 vertices + 1 to close the loop.
            Assert.That(firstSeries.Points.Count, Is.EqualTo(5));
        }
    }
}
