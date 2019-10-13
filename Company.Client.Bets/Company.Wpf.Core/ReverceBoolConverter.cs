using System;
using System.Globalization;
namespace Company.Wpf.Core.Converters
{
    public class ReverceBoolConverter : BaseMarkupConverter<ReverceBoolConverter>
    {
        BoolConverter boolConverter = new BoolConverter();
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)boolConverter.Convert(value, targetType, parameter, culture);
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)boolConverter.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
