using CommunityToolkit.Mvvm.ComponentModel;
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
        private PlotModel _plotModel;
        private double _slope;
        private double _intercept;
        private ObservableCollection<double> _polynomialCoefficients;
        private double _xMinimum = -10;
        private double _xMaximum = 10;
        private double _xStep = 0.1;

        public PlotViewModel()
        {
            _plotModel = new PlotModel{ Title="Line Graph"};
            _slope = 1; // Default slope.
            _intercept = 0; // Default intercept.
            _polynomialCoefficients = new ObservableCollection<double>();

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

            var lineSeries = new LineSeries();

            // Generate points for the line.
            for (double x = -10; x <= 10; x += 1)
            {
                // y = ax + b
                double y = _slope * x + _intercept;
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            _plotModel.Series.Add(lineSeries);

            // Update polynomial series using new range and step size properties
            if (_polynomialCoefficients.Count > 0)
            {
                var polynomialSeries = new LineSeries();
                for (double x = _xMinimum; x <= _xMaximum; x += _xStep)
                {
                    double y = 0;
                    for (int i = 0; i < _polynomialCoefficients.Count; i++)
                    {
                        y += _polynomialCoefficients[i] * Math.Pow(x, i);
                    }
                    polynomialSeries.Points.Add(new DataPoint(x, y));
                }
                _plotModel.Series.Add(polynomialSeries);
            }

            // Update the model which in turn updates the view.
            _plotModel.InvalidatePlot(true);
        }

        public PlotModel PlotModel
        {
            get => _plotModel;
            private set => SetProperty(ref _plotModel, value);
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
