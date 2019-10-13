using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;

namespace Company.Client.Common.Support
{
    public abstract class AbstractMarkupConverter<TConverter> : MarkupExtension, IValueConverter
        where TConverter : class, new()
    {
        private static TConverter _converter;
        static AbstractMarkupConverter()
        {
            _converter = new TConverter();
        }

        abstract public object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override object ProvideValue(IServiceProvider serviceProvider) => _converter;
    }

    public class ToTypeConverter : AbstractMarkupConverter<ToTypeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }
    }
}
