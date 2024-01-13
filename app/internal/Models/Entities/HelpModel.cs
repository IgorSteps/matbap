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
        public string TangentHelpText { get; }

        public HelpContentModel()
        {
            ExpressionsHelpText =
                "1) Supported tokens are +, -, *, /, ), (, ^, =, sin, cos, tan, log, variable names like 'x' and 'y'\n" +
                "2) You can assign variables like so x=5\n" +
                "3) You can have multiple lines, just put a semicolon ';' on the end of your lines";

            PlotHelpText =
                "1) Able to plot linear and polynomial equations\n" +
                "2) You can plot multiple graphs, just enter new equation and click Plot\n" +
                "3) Click clear to clear plotting area\n" +
                "4) Don't include 'y = ' part, otherwise we will error";

            TangentHelpText =
                "Add a tagnent to plot from the list on the right\n" +
                "You can select the plot by left clicking its equation in the list on the right";
        }
    }
}
