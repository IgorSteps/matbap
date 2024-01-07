using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Msagl.Core.Geometry.Curves;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        private readonly IRootFinder _rootFinder;
        private string _answer;
        private RelayCommand _evalauteCmd, _differentiateCmd, _visualiseASTCmd, _findRootsCmd;
        private string _expressionValue;
        private double _rootXMin, _rootXMax;
        public ExpressionViewModel(IEvaluator evaluator, IRootFinder rootFinder)
        {
            _evaluator = evaluator;
            _rootFinder = rootFinder;
            _evalauteCmd = new RelayCommand(Evaluate);
            _differentiateCmd = new RelayCommand(Differentiate);
            _visualiseASTCmd = new RelayCommand(VisualiseAST);
            _findRootsCmd = new RelayCommand(FindRoots);

            // Defaults.
            _rootXMax = -10;
            _rootXMax = 10;
        }

        public RelayCommand EvaluateCmd => _evalauteCmd;
        public RelayCommand DifferentiateCmd => _differentiateCmd;
        public RelayCommand VisualiseCmd => _visualiseASTCmd;
        public RelayCommand FindRootsCmd => _findRootsCmd;

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

        public double RootXMin
        {
            get => _rootXMin;
            set => SetProperty(ref _rootXMin, value);
        }

        public double RootXMax
        {
            get => _rootXMax;
            set => SetProperty(ref _rootXMax, value);
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
                return;
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

        public void FindRoots()
        {
            var result = _rootFinder.FindRoots(_expressionValue, RootXMin, RootXMax);
            if (result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Roots;
        }

        /// <summary>
        /// Send a message when user uses for-loop plot function for plotting view model.
        /// </summary>
        public void PlotExpression(Expression exp)
        {
            var message = new PlotExpressionMessage(exp);
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
