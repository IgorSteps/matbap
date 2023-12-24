using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    public class HelpContentModel
    {
        public string ExpressionsHelpText { get; }
        public string PlotHelpText { get; }

        public HelpContentModel()
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
