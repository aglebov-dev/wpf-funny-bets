using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Company.Wpf.Core.Converters
{
    public abstract class BaseMarkupConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        private static T _converter;
        static BaseMarkupConverter()
        {
            _converter = new T();
        }

        abstract public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter;
        }
    }
}
