using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace app
{
    public class HelpViewModel : ObservableObject
    {

        public string ExpressionsHelpText { get; set; }
        public string PlotHelpText { get; set; }


        public HelpViewModel()
        {
            ExpressionsHelpText = 
                "Supported tokens are +, -, *, /, ), (, ^, =, sin, cos, tan, log, variable names like 'x' and 'y'\n" +
                "You can assign variables like so x=5\n" +
                "You can have multiple lines, just put a semicolon ';' on the end of your lines";

            PlotHelpText = 
                "Able to plot linear and polynomial equations\n" +
                "You can plot multiple graphs, just enter new equation and click Plot\n" +
                "Click clear to clear plotting area\n" +
                "Don't include 'y = ' part, otherwise we will error";
        }

  
    }
}
