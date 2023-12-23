using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<InterpretationViewModel>();

            // @TODO: Remove me for better implementation(a toggle)!!!!!!!
            ASTWindow astW = new ASTWindow();
            astW.Show();
        }
        private void OpenTokenHelpClick(object sender, RoutedEventArgs e)
        {
            HelpWindow tokenHelpWindow = new HelpWindow();
            tokenHelpWindow.Show();
        }

        private void OpenPlotClick(object sender, RoutedEventArgs e)
        {
            PlotWindow plotWindow = new PlotWindow();
            plotWindow.Show();
        }
    }



}
