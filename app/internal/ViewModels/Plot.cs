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
        private readonly IPlotEquationEvaluator _equationEvaluator;

        private PlotModel _plotModel;
        private RelayCommand _interpretCmd;

        private double _slope;
        private double _intercept;
        // @NOTE: Must be populated with coefficients in descending order of their corresponding powers of.
        private ObservableCollection<double> _polynomialCoefficients;
        private double _xMinimum;
        private double _xMaximum;
        private double _xStep;

        private string _inputEquation;
        

        public PlotViewModel(IPlotEquationEvaluator p)
        {
            _equationEvaluator = p;
            _plotModel = new PlotModel{ Title="Your Plot"};
            _interpretCmd = new RelayCommand(Interpret);
            _polynomialCoefficients = new ObservableCollection<double>();

            // Set defaults.
            _slope = 1;
            _intercept = 0;
            _xMinimum = -10;
            _xMaximum = 10;
            _xStep = 0.1;
            SetUpAxis();
            UpdatePlot();
        }

        private void SetUpAxis()
        {
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 });
            _plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 });
        }

        public void UpdatePlot()
        {
            _plotModel.Series.Clear();

            PlotLine();

            if (_polynomialCoefficients.Count > 0)
            {
                PlotPolynomial();
            }
           
            // Update the model which in turn updates the view.
            _plotModel.InvalidatePlot(true);
        }

        private void PlotLine()
        {
            var lineSeries = new LineSeries();

            // Generate points for the line.
            //for (double x = -10; x <= 10; x += 1)
            //{
            //    // y = ax + b
            //    double y = _slope * x + _intercept;
            //    lineSeries.Points.Add(new DataPoint(x, y));
            //}

            _plotModel.Series.Add(lineSeries);
        }

        private void PlotPolynomial()
        {

                var polynomialSeries = new LineSeries();
                // Using user input of range and step sizes, calculate y values for range of x values.
                //for (double x = _xMinimum; x <= _xMaximum; x += _xStep)
                //{
                //    // Evaluate polynomial using Horner's method.
                //    double y = Horner(x);
                //    polynomialSeries.Points.Add(new DataPoint(x, y));
                //}
                _plotModel.Series.Add(polynomialSeries);
        }

        // Horner Method for Polynomial Evaluation.
        public double Horner(double x)
        {
            double y = _polynomialCoefficients[0];
            for (int i = 1; i < _polynomialCoefficients.Count; i++)
            {
                y = y * x + _polynomialCoefficients[i];
            }
            return y;
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


        public double Slope
        {
            get => _slope;
            set
            {
                if (SetProperty(ref _slope, value))
                {
                    UpdatePlot();
                }
            }
        }

        public double Intercept
        {
            get => _intercept;
            set
            {
                if (SetProperty(ref _intercept, value))
                {
                    UpdatePlot();
                }
            }
        }

        public ObservableCollection<double> PolynomialCoefficients
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
