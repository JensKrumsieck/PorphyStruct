using OxyPlot.Series;
using PorphyStruct.Core.Plot;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PorphyStruct.WPF.Template
{
    public partial class TextBoxBindings : ResourceDictionary
    {
        /// <summary>
        /// Explicit Binding on Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BindingOnEnterUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Tab) return;
            UpdateSource((TextBox)sender);
            e.Handled = true;
        }

        /// <summary>
        /// Occurs on lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxOnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateSource((TextBox)sender);
            e.Handled = true;
        }

        private static void UpdateSource(FrameworkElement textBox)
        {
            var expr = textBox.GetBindingExpression(TextBox.TextProperty);
            switch (expr?.ResolvedSource)
            {
                case DefaultPlotModel model:
                    model.InvalidatePlot(false);
                    break;
                case Series series:
                    series.PlotModel.InvalidatePlot(false);
                    break;
                case OxyPlot.Axes.LinearAxis axis:
                    axis.PlotModel.InvalidatePlot(false);
                    break;
            }
            expr?.UpdateSource();
        }
    }
}
