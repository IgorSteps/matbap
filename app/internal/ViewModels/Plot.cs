using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.FSharp.Collections;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace app
{
    public class PlotViewModel : ObservableObject
    {
        const double AxisMin = -10;
        const double AxisMax = 10; 

        private readonly IPlotEquationEvaluator _equationEvaluator;`

        private PlotModel _plotModel;
        private RelayCommand _interpretCmd;

        private float[,] _points;
        private float _xMinimum;
        private float _xMaximum;
        private float _xStep;

        private string _inputEquation;
        

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel{ Title="Your Plot"};
            _interpretCmd = new RelayCommand(Interpret);
            

            // Set defaults.
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1f;

            SetUpAxis();
            UpdatePlot();
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

            int rows = _points.GetLength(0);
            int cols = _points.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) 
                {
                    lineSeries.Points.Add(new DataPoint(i, j));
                }
            }
            _plotModel.Series.Add(lineSeries);
        }

        /// <summary>
        ///  InterpretCmd binds to a button in the plot view, executes the Interpret() when clicked.
        /// </summary>
        public RelayCommand InterpretCmd => _interpretCmd;

        private void Interpret()
        {
            _points = _equationEvaluator.Evaluate(XMinimum,XMaximum, XStep, InputEquation);
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

        public float XMinimum
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

        public float XMaximum
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

        public float XStep
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
