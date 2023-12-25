using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;


namespace app
{
    public class PlotViewModel : ObservableObject
    {
        private readonly IPlotter _plotter;
        private PlotModel _oxyPlotModel;
        private readonly RelayCommand _plotCmd, _clearCmd;

        private string _inputEquation;
        private string _evaluatorError;

        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        public PlotViewModel(IPlotter plotter)
        {
            _plotter = plotter;
            _plotCmd = new RelayCommand(Plot);
            _clearCmd = new RelayCommand(Clear);
            _evaluatorError = "";

            // Set defaults.
            _inputEquation = "";
            _oxyPlotModel = new PlotModel();
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;
        }

        public PlotModel OxyPlotModel
        {
            get => _oxyPlotModel;
            private set => SetProperty(ref _oxyPlotModel, value);
        }

        public string InputEquation
        {
            get => _inputEquation; 
            set => SetProperty(ref _inputEquation, value);
        }

        public string EvaluatorError
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

        /// <summary>
        ///  PlotCmd binds to a button in the plot view, executes the Plot() when clicked.
        /// </summary>
        public RelayCommand PlotCmd => _plotCmd;

        // <summary>
        ///  ClearCmd binds to a button in the plot view, executes the Clear() when clicked.
        /// </summary>
        public RelayCommand ClearCmd => _clearCmd;

        private void Plot()
        {
            var plotResult = _plotter.CreatePlot(InputEquation, XMinimum, XMaximum, XStep);
            if (plotResult.HasError)
            {
                EvaluatorError = plotResult.Error;
            }
            else
            {
                EvaluatorError = "";
                OxyPlotModel = plotResult.OxyPlotModel;
            }
        }

        private void Clear()
        {
            _plotter.ClearPlots();
        }
    }
} 
