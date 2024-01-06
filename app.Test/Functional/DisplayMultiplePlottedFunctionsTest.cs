using Engine;

namespace app.Test.Functional
{
    public class DisplayMultiplePlottedFunctionsTest
    {
        /// <summary>
        /// Test to simulate user adding 3 plots and GUI displaying their functions in the "Plotted Functions:" panel.
        /// </summary>
        [Test]
        public void TestDisplayMultiplePlottedFunctions()
        {
            // --------
            // ASSEMBLE
            // --------
            PlotViewModel plotViewModel = Utils.CreaePlotViewModel();

            string userSetInputEquation = "x^2";
            double userSetXMin = -10, userSetXMax = 10, userSetXStep = 0.5;

            plotViewModel.InputEquation = userSetInputEquation;
            plotViewModel.XMinimum = userSetXMin;
            plotViewModel.XMaximum = userSetXMax;
            plotViewModel.XStep = userSetXStep;

            string userSetInputEquationSecond = "2*x+1";
            string userSetInputEquationThird = "123";

            // -----
            // ACT
            // -----
            plotViewModel.PlotCmd.Execute(null);
            
           
            plotViewModel.InputEquation = userSetInputEquationSecond;
            plotViewModel.PlotCmd.Execute(null);
            
            plotViewModel.InputEquation = userSetInputEquationThird;
            plotViewModel.PlotCmd.Execute(null);

            // ------
            // ASSERT
            // ------
            Assert.That(plotViewModel.Plots.Count, Is.EqualTo(3), "There must be 3 plot");
            Assert.That(plotViewModel.Plots[0].Function, Is.EqualTo(userSetInputEquation), "Incorrect equation is stored in the Plots collection");
            Assert.That(plotViewModel.Plots[1].Function, Is.EqualTo(userSetInputEquationSecond), "Incorrect equation is stored in the Plots collection");
            Assert.That(plotViewModel.Plots[2].Function, Is.EqualTo(userSetInputEquationThird), "Incorrect equation is stored in the Plots collection");
        }
    }
}
