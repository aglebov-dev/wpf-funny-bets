using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using Company.Client.Bets.Models;

namespace Company.Client.Bets.Desktop.Converters
{
    class DomainKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DomainKind
                   ? new StaticResourceExtension($"DomainKind.{Enum.GetName(typeof(DomainKind), value)}")
                   : new StaticResourceExtension($"DomainKind.Default");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
