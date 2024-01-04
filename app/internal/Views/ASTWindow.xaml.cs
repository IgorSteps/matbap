using Microsoft.Msagl.Drawing;
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
