using System;
using System.Windows;
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
            DataContext = App.Current.Services.GetService<ExpressionViewModel>();
        }
    }
}
