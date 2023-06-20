using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using PorphyStruct.Core.Analysis;
using PorphyStruct.ViewModel;

namespace PorphyStruct.WPF.Converter
{
    public class SufficientSymmetryConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => (object) this;

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is not AnalysisViewModel vm) return false;
            return vm.Analysis.Type != MacrocycleType.Corrole && vm.Analysis.Type != MacrocycleType.Corrphycene;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}