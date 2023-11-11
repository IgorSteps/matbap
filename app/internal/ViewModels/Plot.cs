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

        public PlotViewModel()
        {
            _plotModel = new PlotModel{ Title="Line Graph"};
            _slope = 1; // Default slope.
            _intercept = 0; // Default intercept.
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

            var lineSeries = new LineSeries
            {
                Title = $"y = {_slope}x + {_intercept}",
                MarkerType = MarkerType.None
            };

            // Generate points for the line
            for (double x = -10; x <= 10; x += 1)
            {
                double y = _slope * x + _intercept;
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            _plotModel.Series.Add(lineSeries);

            // Update the model which in turn updates the view
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
    }
} 
