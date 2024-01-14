using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using System.Collections.ObjectModel;

namespace app
{
    public class PlotViewModel : ObservableObject
    {
        private readonly IPlotter _plotter;
        private readonly IOxyPlotModelManager _plotModelManager;
        private readonly IAreaUnderCurveShower _areaUnderCurveShower;
        private readonly RelayCommand _plotCmd, _clearCmd, _addTangentCmd, _showAreaUnderCurvCmd;

        private PlotModel _oxyPlotModel;
        private ObservableCollection<Plot> _plots;
        private Plot _selectedPlot;

        private string _inputEquation;
        private string _evaluatorError;

        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;
        private double _tangentX;
        private double _integrationStep;
        public PlotViewModel(IPlotter plotter, IOxyPlotModelManager plotModelManager, IAreaUnderCurveShower areaUnderCurveShower)
        {
            _plotter = plotter;
            _plotModelManager = plotModelManager;
            _areaUnderCurveShower = areaUnderCurveShower;

            _plotCmd = new RelayCommand(Plot);
            _clearCmd = new RelayCommand(Clear);
            _addTangentCmd = new RelayCommand(AddTangent);
            _showAreaUnderCurvCmd = new RelayCommand(ShowAreaUnderCurve);

            _plots = new ObservableCollection<Plot>();
            _oxyPlotModel = new PlotModel();

            // Set defaults.
            _evaluatorError = "";
            _inputEquation = "";
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;
            _integrationStep = 1;

            // Register to listen for PlotExpressionMessage.
            WeakReferenceMessenger.Default.Register<PlotExpressionMessage>(this, (recipient, msg) =>
            {
                HandlePlotExpressionMessage(msg.Expression);
            });
        }

        public ObservableCollection<Plot> Plots
        {
            get => _plots;
            set => SetProperty(ref _plots, value);
        }

        public Plot SelectedPlot
        {
            get => _selectedPlot;
            set => SetProperty(ref _selectedPlot, value);
        }

        public double TangentX
        {
            get => _tangentX;
            set => SetProperty(ref _tangentX, value);
        }

        public PlotModel OxyPlotModel
        {
            get => _oxyPlotModel;
            set => SetProperty(ref _oxyPlotModel, value);
        }

        public string InputEquation
        {
            get => _inputEquation; 
            set => SetProperty(ref _inputEquation, value);
        }

        public string Error
        {
            get => _evaluatorError;
            set => SetProperty(ref _evaluatorError, value);
        }

        public double XMinimum
        {
            get => _xMinimum;
            set => SetProperty(ref _xMinimum, value);
        }

        public double XMaximum
        {
            get => _xMaximum;
            set => SetProperty(ref _xMaximum, value);
        }

        public double XStep
        {
            get => _xStep;
            set => SetProperty(ref _xStep, value);
        }

        public double IntegrationStep
        {
            get => _integrationStep;
            set => SetProperty(ref _integrationStep, value);
        }

        /// <summary>
        ///  PlotCmd binds to a button in the plot view, executes the Plot() when clicked.
        /// </summary>
        public RelayCommand PlotCmd => _plotCmd;

        private void Plot()
        {
            var result = _plotter.CreatePlot(OxyPlotModel, InputEquation, XMinimum, XMaximum, XStep);
            if (result.HasError)
            {
                Error = result.Error.ToString();
                return;
            }

            Plots.Add(result.Plot);
            SelectedPlot = result.Plot;   
            RefreshPlottingArea(); 
        }

        /// <summary>
        ///  AddTangentCmd binds to a button in the plot view, executes the AddTanget() when clicked.
        /// </summary>
        public RelayCommand AddTangentCmd => _addTangentCmd;

        private void AddTangent()
        {
            if (SelectedPlot == null)
            {
                Error = "You must select the plot to add a tangent to it.";
                return;
            }

            var result = _plotter.AddTangent(OxyPlotModel, TangentX, InputEquation, XMinimum, XMaximum, XStep);
            if (result.HasError)
            {
                Error = result.Error.ToString();
                return;
            }

            SelectedPlot.Tangent = result.Tangent;

            RefreshPlottingArea();
        }

        private void HandlePlotExpressionMessage(Expression expression)
        {
            var result = _plotter.CreatePlotFromExpression(OxyPlotModel, expression);
            Plots.Add(result.Plot);
            SelectedPlot = result.Plot;
            RefreshPlottingArea();
        }

        /// <summary>
        /// ClearCmd binds to a button in the plot view, executes the Clear() when clicked.
        /// </summary>
        public RelayCommand ClearCmd => _clearCmd;

        /// <summary>
        /// Clear all plotting data.
        /// Clear _plots, _oxyPlotModel
        /// Sets SelectedPLot to null.
        /// </summary>
        private void Clear()
        {
            SelectedPlot = null;
            _plotModelManager.ClearPlotModel(_oxyPlotModel);
            _plots.Clear();
            RefreshPlottingArea() ;
        }

        public RelayCommand ShowAreaUnderCurveCmd => _showAreaUnderCurvCmd;
        private void ShowAreaUnderCurve()
        {
            if (SelectedPlot == null)
            {
                Error = "You must select the plot to show area under it.";
                return;
            }

            _areaUnderCurveShower.ClearTrapeziumList();
            var result = _areaUnderCurveShower.ShowAreaUnderCurve(OxyPlotModel, SelectedPlot, IntegrationStep);
            if (result.HasError)
            {
                Error = result.Error.ToString();
                return;
            }

            RefreshPlottingArea();
            Error = "Area under the curve = " + result.Area.ToString();
        }
        /// <summary>
        /// Set error to empty string.
        /// </summary>
        private void ClearError()
        {
            Error = "";
        }
 
        /// <summary>
        /// Refresh OxyPlot's Plot Model and clear error.
        /// </summary>
        private void RefreshPlottingArea()
        {
            _plotModelManager.RefreshPlotModel(_oxyPlotModel);
            ClearError();
        }
    }
} 
