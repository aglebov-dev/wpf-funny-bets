using System;
using System.Globalization;

namespace Company.Wpf.Core.Converters
{
	public class BoolConverter : BaseMarkupConverter<BoolConverter>
    {
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return value;
            }
            else if (value is int || value is double || value is decimal || value is long)
            {
                var numeric = 0.0;
                return double.TryParse(value.ToString(), out numeric) ? numeric != 0 : false;
            }
            else if (value is DateTime)
            {
                return ((DateTime)value) != DateTime.MinValue;
            }
            else if (value is string)
            {
                return !string.IsNullOrWhiteSpace(value?.ToString());
            }
            else if (value is System.Windows.Visibility)
            {
                return ((System.Windows.Visibility)value) == System.Windows.Visibility.Visible ? true : false;
            }
            else
            {
                return value != null ? true : false;
            }
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return value;
            throw new ArgumentException("Can not convert a value other than bool");
        }
    }
}
