using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        const double AxisMax = -10; 

        private readonly IPlotEquationEvaluator _equationEvaluator;

        private PlotModel _plotModel;
        private RelayCommand _interpretCmd;

        // @NOTE: Must be populated with coefficients in descending order of their corresponding powers of.
        private List<double> _polynomialCoefficients;
        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private string _inputEquation;
        

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel{ Title="Your Plot"};
            _interpretCmd = new RelayCommand(Interpret);
            _polynomialCoefficients = new List<double>();

            // Set defaults.
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;

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
            _plotModel.InvalidatePlot(true);
        }

        private void PlotLine()
        {
            throw new NotImplementedException();
        }

        private void PlotPolynomial()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  InterpretCmd binds to a button in the plot view, executes the Interpret() when clicked.
        /// </summary>
        public RelayCommand InterpretCmd => _interpretCmd;

        private void Interpret()
        {
            _equationEvaluator.Evaluate(InputEquation);
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

        public List<double> PolynomialCoefficients
        {
            get => _polynomialCoefficients;
            set
            {
                if (SetProperty(ref _polynomialCoefficients, value))
                {
                    UpdatePlot();
                }
            }
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
