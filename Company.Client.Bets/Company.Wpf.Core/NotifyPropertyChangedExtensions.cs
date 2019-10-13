namespace Company.Client.Common.Extensions
{
    using System;
    using System.ComponentModel;

    public static class NotifyPropertyChangedExtensions
    {
        public static INotifyPropertyChanged OfInpc(this object inpc, Action<INotifyPropertyChanged> whenSuccess)
        {
            var value = inpc as INotifyPropertyChanged;
            if (value != null)
                whenSuccess?.Invoke(value);

            return value;
        }

        public static PropertyChangedEventArgs WhenProperty(this PropertyChangedEventArgs args, string propertyName, Action action)
        {
            if (propertyName != null && args.PropertyName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                action?.Invoke();

            return args;
        }
    }
}
