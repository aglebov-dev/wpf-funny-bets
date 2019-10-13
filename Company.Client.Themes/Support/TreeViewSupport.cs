namespace Company.Client.Themes.Support
{
    using System.Windows;
    using System.Windows.Controls;

    static class TreeViewSupport
    {
        public static readonly DependencyProperty LeftOffsetProperty;

        static TreeViewSupport()
        {
            var meta = new PropertyMetadata
            {
                DefaultValue = 0,
                PropertyChangedCallback = LeftOffsetCallBack
            };
            LeftOffsetProperty = DependencyProperty.RegisterAttached("LeftOffset", typeof(int), typeof(TreeViewSupport), meta);
        }

        private static void LeftOffsetCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as FrameworkElement;
            var margin = sender.Margin;

            if (sender != null)
            {
                var depth = -1;
                var curent = XamlUtils.FindParent<TreeViewItem>(sender);

                while (curent != null)
                {
                    depth++;
                    curent = XamlUtils.FindParent<TreeViewItem>(curent);
                }

                sender.Margin = new Thickness
                {
                    Left = margin.Left + depth * ((int?)e.NewValue ?? 0),
                    Right = margin.Right,
                    Top = margin.Top,
                    Bottom = margin.Bottom
                };
            }
        }


        public static int GetLeftOffset(DependencyObject obj)
        {
            return (int)obj.GetValue(LeftOffsetProperty);
        }

        public static void SetLeftOffset(DependencyObject obj, int value)
        {
            obj.SetValue(LeftOffsetProperty, value);
        }
    }
}
