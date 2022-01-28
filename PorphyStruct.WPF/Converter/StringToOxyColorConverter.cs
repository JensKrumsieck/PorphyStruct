using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using OxyPlot;

namespace PorphyStruct.WPF.Converter
{
    [ValueConversion(typeof(OxyColor), typeof(string))]
    public class StringToOxyColorConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            ((OxyColor)value).ToString();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => OxyColor.Parse((string)value);
    }
}
