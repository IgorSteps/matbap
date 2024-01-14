using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Engine;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace app
{
    public class ExpressionViewModel : ObservableObject
    {
        private readonly IEvaluator _evaluator;
        private readonly IRootFinder _rootFinder;
        private readonly IDifferentiator _differentiator;
        private readonly ISymbolTableManager _symTableManager;
        private string _answer;
        private RelayCommand _evalauteCmd, _differentiateCmd, _visualiseASTCmd, _findRootsCmd, _clearSymTableCmd;
        private string _expressionValue;
        private double _rootXMin, _rootXMax;
        private ObservableCollection<KeyValuePair<string, Engine.Types.NumType>> _guiSymbolTable;

        public ExpressionViewModel(IEvaluator evaluator, IRootFinder rootFinder, IDifferentiator differentiator, ISymbolTableManager symTableManager)
        {
            _evaluator = evaluator;
            _rootFinder = rootFinder;
            _differentiator = differentiator;
            _symTableManager = symTableManager;
            _evalauteCmd = new RelayCommand(Evaluate);
            _differentiateCmd = new RelayCommand(Differentiate);
            _visualiseASTCmd = new RelayCommand(VisualiseAST);
            _findRootsCmd = new RelayCommand(FindRoots);
            _clearSymTableCmd = new RelayCommand(ClearSymbolTable);
            _guiSymbolTable = new ObservableCollection<KeyValuePair<string, Engine.Types.NumType>>();

            // Defaults.
            _rootXMax = -10;
            _rootXMax = 10;
        }

        public RelayCommand EvaluateCmd => _evalauteCmd;
        public RelayCommand DifferentiateCmd => _differentiateCmd;
        public RelayCommand VisualiseCmd => _visualiseASTCmd;
        public RelayCommand FindRootsCmd => _findRootsCmd;
        public RelayCommand ClearSymTableCmd => _clearSymTableCmd;
        public ObservableCollection<KeyValuePair<string, Engine.Types.NumType>> GUISymbolTable
        {
            get => _guiSymbolTable;
            private set => SetProperty(ref _guiSymbolTable, value);
        }
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
            UpdateGUISymbolTable(_symTableManager.GetSymbolTable().RawSymbolTable);
            Answer = result.Result;
        }

        public void Differentiate()
        {
            var result = _differentiator.Differentiate(_expressionValue);
            if (result.HasError)
            {
                Answer = result.Error.ToString();
                return;
            }

            Answer = result.Derivative;
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

        public void ClearSymbolTable()
        {
            _symTableManager.ClearSymbolTable();
            GUISymbolTable.Clear();
        }

        private void UpdateGUISymbolTable(Dictionary<string, Engine.Types.NumType> newTable)
        {
            GUISymbolTable.Clear();
            foreach (var item in newTable)
            {
                GUISymbolTable.Add(item);
            }
        }
    }
}
