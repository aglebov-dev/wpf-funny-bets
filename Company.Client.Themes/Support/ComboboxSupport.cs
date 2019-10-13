namespace Company.Client.Themes.Support
{
    using System.Windows;

    static class ComboboxSupport
    {
        public static readonly DependencyProperty MultySelectedContentTemplateProperty;

        static ComboboxSupport()
        {
            MultySelectedContentTemplateProperty = DependencyProperty.RegisterAttached("MultySelectedContentTemplate", typeof(DataTemplate), typeof(ComboboxSupport), new PropertyMetadata(null));
        }

        public static DataTemplate GetMultySelectedContentTemplate(DependencyObject obj) 
            => (DataTemplate)obj.GetValue(MultySelectedContentTemplateProperty);
        public static void SetMultySelectedContentTemplate(DependencyObject obj, DataTemplate value) 
            => obj.SetValue(MultySelectedContentTemplateProperty, value);
    }
}
