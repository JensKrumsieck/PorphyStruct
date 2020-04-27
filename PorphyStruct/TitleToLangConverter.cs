using System;
using System.Globalization;
using System.Windows.Data;

namespace PorphyStruct
{
    public class TitleToLangConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Properties.Language.Save.ResourceManager.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => "";
    }
}
