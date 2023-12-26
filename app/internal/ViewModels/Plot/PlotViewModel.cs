using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace app
{
    public struct EquationColourPair
    {
        public string Equation { get; set; }
        public string Colour { get; set; }
        public EquationColourPair(string equation, string colour)
        {
            Equation = equation;
            Colour = colour;
        }
    }

    public class PlotViewModel : ObservableObject
    {
        private readonly IPlotter _plotter;
        private PlotModel _oxyPlotModel;
        private readonly RelayCommand _plotCmd, _clearCmd, _addTangentCmd;

        private ObservableCollection<EquationColourPair> _equationColours;
        private string _inputEquation;
        private string _evaluatorError;

        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private double _tangentX;

        public PlotViewModel(IPlotter plotter)
        {
            _plotter = plotter;
            _plotCmd = new RelayCommand(Plot);
            _clearCmd = new RelayCommand(Clear);
            _addTangentCmd = new RelayCommand(AddTangent);
            _evaluatorError = "";
            _equationColours = new ObservableCollection<EquationColourPair>();

            // Set defaults.
            _inputEquation = "";
            _oxyPlotModel = new PlotModel();
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;
        }

        public ObservableCollection<EquationColourPair> EquationColors
        {
            get => _equationColours;
            set => SetProperty(ref _equationColours, value);
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

        private void Plot()
        {
            var result = _plotter.CreatePlot(InputEquation, XMinimum, XMaximum, XStep);
            PlotResult(result);
        }

        /// <summary>
        ///  AddTangentCmd binds to a button in the plot view, executes the AddTangnet() when clicked.
        /// </summary>
        public RelayCommand AddTangentCmd => _addTangentCmd;

        private void AddTangent()
        {
            var result = _plotter.AddTangent(TangentX, InputEquation, XMinimum, XMaximum, XStep);
            PlotResult(result);
        }     

        /// <summary>
        ///  ClearCmd binds to a button in the plot view, executes the Clear() when clicked.
        /// </summary>
        public RelayCommand ClearCmd => _clearCmd;

        private void Clear()
        {
            _plotter.ClearPlots();
            _equationColours.Clear();
        }

        private void PlotResult(PlotResult result)
        {
            if (result.HasError)
            {
                EvaluatorError = result.Error;
            }
            else
            {
                ClearError();
                OxyPlotModel = result.OxyPlotModel;
                OxyPlotModel.InvalidatePlot(true);

                UpdateSeriesColours();
            }
        }

        /// <summary>
        /// Set error to empty string.
        /// </summary>
        private void ClearError()
        {
            EvaluatorError = "";
        }

        /// <summary>
        /// Update list of Pairs of equations and their colours to display
        /// alongside plotting area.
        /// </summary>
        private void UpdateSeriesColours()
        {
            _equationColours.Clear();
            foreach (var series in OxyPlotModel.Series)
            {
                if (series is LineSeries lineSeries)
                {
                    string colour = ColourToHex(lineSeries.ActualColor);
                    var pair = new EquationColourPair(lineSeries.Title, colour);
                    _equationColours.Add(pair);
                }
            }
        }

        private string ColourToHex(OxyColor c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
} 
