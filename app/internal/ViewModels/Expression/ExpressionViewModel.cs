using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OxyPlot;
using System.Windows.Forms;
using static System.Reflection.Metadata.BlobBuilder;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        private readonly IPlotter _plotter;
        private string _answer;
        private RelayCommand _evalauteCmd, _differentiateCmd, _visualiseASTCmd;
        private string _expressionValue;

        public ExpressionViewModel(IEvaluator evaluator, IPlotter plotter)
        {
            _evaluator = evaluator;
            _evalauteCmd = new RelayCommand(Evaluate);
            _differentiateCmd = new RelayCommand(Differentiate);
            _visualiseASTCmd = new RelayCommand(VisualiseAST);
            _plotter = plotter;
        }

        public RelayCommand EvaluateCmd => _evalauteCmd;
        public RelayCommand DifferentiateCmd => _differentiateCmd;
        public RelayCommand VisualiseCmd => _visualiseASTCmd;

        public string Expression
        {
            get => _expressionValue;
            set => SetProperty(ref _expressionValue, value);
        }

        public string Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        public void Evaluate()
        {
            var result = _evaluator.Evaluate(_expressionValue);
            if(result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }
            if (result.Expression.Points != null) 
            {
                PlotExpression(result.Expression);
            }

            Answer = result.Result;
        }

        public void Differentiate()
        {
            var result = _evaluator.Differentiate(_expressionValue);
            if (result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Result;
        }

        public void VisualiseAST()
        {
            var result = _evaluator.VisualiseAST(_expressionValue);
            if (result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            var astWindow = new ASTWindow(result.AST);
            astWindow.Show();
        }

        public void PlotExpression(Expression exp)
        {
            PlotModel plotModel = new PlotModel(); ;
            var result = _plotter.CreatePlotFromExpression(plotModel, exp);

            var plotViewModel = App.Current.Services.GetService<PlotViewModel>();
            plotViewModel.OxyPlotModel = plotModel;
            plotViewModel.Plots.Add(result.Plot);
            plotViewModel.SelectedPlot = result.Plot;

            plotViewModel.OxyPlotModel.InvalidatePlot(true);
        }
    }
}
