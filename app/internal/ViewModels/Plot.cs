using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.FSharp.Collections;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        private double[][] _points;
        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private string _inputEquation;

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel { };
            _interpretCmd = new RelayCommand(Interpret);
            _evaluatorError = "";
            
            _points = new double[100][];
            // Default 'y = x' line
            // so we don't hit null pointers.
            for (int i = 0; i < 100; i++)
            {
                _points[i] = new double[] { i, i };
            }

            // Set defaults.
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;

            SetUpAxis();
        }

        private void SetUpAxis()
        {
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = AxisMin, Maximum = AxisMax });
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = AxisMin, Maximum = AxisMax });
        }

        public void UpdatePlot()
        {
            _plotModel.Series.Clear();
            Plot();
            _plotModel.InvalidatePlot(true);
        }

        public void UpdatePlotPoints(double[][] p)
        {
            _points = p;
            UpdatePlot();
        }

        private void Plot()
        {
            var lineSeries = new LineSeries();

            for (int i = 0; i < _points.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(_points[i][0], _points[i][1]));
            }
            _plotModel.Series.Add(lineSeries);
        }

        /// <summary>
        ///  InterpretCmd binds to a button in the plot view, executes the Interpret() when clicked.
        /// </summary>
        public RelayCommand InterpretCmd => _interpretCmd;

        private void Interpret()
        {
            var result = _equationEvaluator.Evaluate(XMinimum,XMaximum, XStep, InputEquation);
            if (result.HasError)
            {
                DisplayError(result.Error);
                return;
            } else
            {
                UpdatePlotPoints(result.Points);
            }
        }

        private void DisplayError(string error)
        {
            EvaluatorError = error;
        }


        // ------------------------------------------------------------------------------------------------------
        // -------------------------------------- Getters & Setters below. --------------------------------------
        // ------------------------------------------------------------------------------------------------------

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
            set
            {
                if (SetProperty(ref _xMinimum, value))
                {
                    UpdatePlot();
                }
            }
        }

        public double XMaximum
        {
            get => _xMaximum;
            set
            {
                if (SetProperty(ref _xMaximum, value))
                {
                    UpdatePlot();
                }
            }
        }

        public double XStep
        {
            get => _xStep;
            set
            {
                if (SetProperty(ref _xStep, value))
                {
                    UpdatePlot();
                }
            }
        }

    }
} 
