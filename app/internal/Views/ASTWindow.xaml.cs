using GraphX.Controls;
using GraphX.Logic.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Msagl.Drawing;

using QuickGraph;
using System.Windows;


namespace app
{
    /// <summary>
    /// Interaction logic for ASTWindow.xaml
    /// </summary>
    public partial class ASTWindow : Window
    {
        public ASTWindow(Graph graph)
        {
            InitializeComponent();


            gViewer.Graph = graph;
        }

    }
}
