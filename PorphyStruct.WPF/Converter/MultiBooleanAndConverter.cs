using System.Windows.Data;
using System.Windows.Markup;

namespace PorphyStruct.WPF.Converter
{
    public class MultiBooleanAndConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) => values.OfType<IConvertible>().All(System.Convert.ToBoolean);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
