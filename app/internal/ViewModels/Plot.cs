using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Series;
using System;

namespace app
{
    public class PlotViewModel : ObservableObject
    {
        const double AxisMin = 0;
        const double AxisMax = 10;

        private readonly IPlotEquationEvaluator _equationEvaluator;

        private PlotModel _plotModel;
        private RelayCommand _interpretCmd;
        private string _evaluatorError;
        private EvaluationResult _evaluationResult;
        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private string _inputEquation;

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel();
            _interpretCmd = new RelayCommand(Interpret);
            _evaluatorError = "";

            // Set defaults.
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;

            SetUpAxis();
            UpdatePlot();
        }

        private void SetUpAxis()
        {
            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Minimum = AxisMin, Maximum = AxisMax });
            _plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Minimum = AxisMin, Maximum = AxisMax });
        }

        public void UpdatePlot()
        {
            if (_evaluationResult != null && !_evaluationResult.HasError)
            {
                _plotModel.Series.Clear();
                Plot(_evaluationResult);
                _plotModel.InvalidatePlot(true);
            }
        }

        public RelayCommand InterpretCmd => _interpretCmd;

        public void Interpret()
        {
            var result = _equationEvaluator.Evaluate(XMinimum, XMaximum, XStep, InputEquation);
            _evaluationResult = result;

            if (_evaluationResult.HasError)
            {
                DisplayError(_evaluationResult.Error);
                return;
            }
            else
            {
                UpdatePlot();
            }
        }

        private void DisplayError(string error)
        {
            EvaluatorError = error;
        }

        private void Plot(EvaluationResult result)
        {
            if (result.PositiveSegment != null)
            {
                var positiveLineSeries = new LineSeries();

                foreach (var point in result.PositiveSegment)
                {
                    positiveLineSeries.Points.Add(new DataPoint(point[0], point[1]));
                }

                _plotModel.Series.Add(positiveLineSeries);
            }

            if (result.NegativeSegment != null)
            {
                var negativeLineSeries = new LineSeries();

                foreach (var point in result.NegativeSegment)
                {
                    negativeLineSeries.Points.Add(new DataPoint(point[0], point[1]));
                }

                _plotModel.Series.Add(negativeLineSeries);
            }
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            private set => SetProperty(ref _plotModel, value);
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
    }
}
