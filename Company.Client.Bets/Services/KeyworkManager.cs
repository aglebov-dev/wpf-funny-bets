using System.Windows;
using System.Windows.Input;
using Company.Wpf.Core;
using Company.Client.Bets.Interfaces;

namespace Company.Client.Bets.Services
{
    static class KeyworkManager
    {
        static FrameworkElement rootElement;
        static FrameworkElement focusedObject;
        public static FrameworkElement FocusedObject
        {
            get => focusedObject;
            set
            {
                rootElement.InputBindings.Clear();
                focusedObject = value;

                if (focusedObject != null)
                {
                    var scope = Find(FocusedObject);

                    if (scope != null)
                    {
                        //rootElement.InputBindings.Add
                    }
                }
            }
        }
        private static IKeyboardManagementScope Find(FrameworkElement sender)
        {
            var curent = sender;
            while (curent != null && !(curent.DataContext is IKeyboardManagementScope))
                curent = XamlUtils.GetParent(curent) as FrameworkElement;

            if (curent != null && curent.DataContext is IKeyboardManagementScope)
                return (IKeyboardManagementScope)curent.DataContext;
            else
                return null;
        }
        internal static void SetRootElement(FrameworkElement frameworkElement)
        {
            rootElement = frameworkElement;
            FocusManager.AddGotFocusHandler(rootElement, FocusChanged);
        }

        private static void FocusChanged(object sender, RoutedEventArgs e) 
            => FocusedObject = e.OriginalSource as FrameworkElement;
    }
}
