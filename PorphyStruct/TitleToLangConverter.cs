using System;
using System.Globalization;
using System.Windows.Data;

namespace PorphyStruct
{
    public class TitleToLangConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Properties.Language.Save.ResourceManager.GetString(value.ToString()!) ?? throw new InvalidOperationException();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
    }
}
