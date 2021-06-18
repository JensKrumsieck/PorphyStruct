using OxyPlot;
using OxyPlot.Axes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PorphyStruct.Core.Plot
{
    public sealed class LinearAxis : OxyPlot.Axes.LinearAxis, INotifyPropertyChanged
    {
        private double _titleAngle = Settings.Instance.LabelAngle;

        public double TitleAngle
        {
            get => _titleAngle;
            set => _titleAngle = value;
        }

        /// <summary>
        /// Not perfect but i need access to <see cref="ActualMaximumAndMinimumChangedOverride"/>
        /// </summary>

        #region BindableProperties
        public double BindableActualMinimum
        {
            get => ActualMinimum;
            set
            {
                ActualMinimum = value;
                OnPropertyChanged();
                Zoom(ActualMinimum, ActualMaximum);
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableActualMaximum
        {
            get => ActualMaximum;
            set
            {
                ActualMaximum = value;
                OnPropertyChanged();
                Zoom(ActualMinimum, ActualMaximum);
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableMajorStep
        {
            get => MajorStep;
            set
            {
                MajorStep = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        public double BindableMinorStep
        {
            get => MinorStep;
            set
            {
                MinorStep = value;
                OnPropertyChanged();
                PlotModel.InvalidatePlot(true);
            }
        }

        private bool _isInverted;
        public bool IsInverted
        {
            get => _isInverted;
            set
            {
                _isInverted = value;
                OnPropertyChanged();
                Invert();
            }
        }
        #endregion

        /// <summary>
        /// Raise Update notification
        /// </summary>
        protected override void ActualMaximumAndMinimumChangedOverride()
        {
            base.ActualMaximumAndMinimumChangedOverride();
            OnPropertyChanged(nameof(BindableActualMaximum));
            OnPropertyChanged(nameof(BindableActualMinimum));
        }

        public override void Render(IRenderContext rc, int pass)
        {
            if (Position == AxisPosition.None) return;

            var r = new HorizontalAndVerticalAxisRenderer(rc, PlotModel);
            r.Render(this, pass);
        }

        /// <summary>
        /// Inverts Axis
        /// </summary>
        public void Invert()
        {
            StartPosition = IsInverted ? 1 : 0;
            EndPosition = IsInverted ? 0 : 1;
            PlotModel.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
