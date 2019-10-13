using System.Windows;

namespace Company.Wpf.Core.XAML
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty;

        static BindingProxy()
        {
            DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = false });
        }
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
}
