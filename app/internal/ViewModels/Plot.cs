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

        private double[][] _points;
        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private string _inputEquation;
        

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel{ Title="Your Plot"};
            _interpretCmd = new RelayCommand(Interpret);
            _points = new double[100][];
            for (int i = 0; i < 100; i++)
            {
                _points[i] = new double[] { i, i }; // Example: y = x line
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
            (_points, string error) = _equationEvaluator.Evaluate(XMinimum,XMaximum, XStep, InputEquation);
            if (error != "")
            {
                _plotModel.Title = "Error: " + error;
                _plotModel.TextColor = OxyColor.FromRgb(255, 0, 0);
            }

            UpdatePlot();
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
