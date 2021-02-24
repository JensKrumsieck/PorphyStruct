using OxyPlot;
using OxyPlot.Axes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PorphyStruct.Core.Plot
{
    public class LinearAxis : OxyPlot.Axes.LinearAxis, INotifyPropertyChanged
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


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
