using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Company.Wpf.Core
{
    /// <summary>
    /// Горизонтальное выравнивание
    /// </summary>
    public enum HorizontalPlacement
    {
        /// <summary>
        /// Влево
        /// </summary>
        Left = 0,
        /// <summary>
        /// По центру
        /// </summary>
        Center = 1,
        /// <summary>
        /// Вправо
        /// </summary>
        Right = 2
    }
    /// <summary>
    /// Вертикальное выравнивание
    /// </summary>
    public enum VerticalPlacement
    {
        /// <summary>
        /// Вверх
        /// </summary>
        Top = 0,
        /// <summary>
        /// По центру
        /// </summary>
        Center = 1,
        /// <summary>
        /// Вниз
        /// </summary>
        Bottom = 2
    }
    /// <summary>
    /// Поведение для выравнивания tooltip
    /// </summary>
    public class ToolTipPlacementBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        public HorizontalPlacement HorizontalPlacement { get; set; }
        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        public VerticalPlacement VerticalPlacement { get; set; }

        public bool InvertDirections { get; set; }
        public bool ToolTipOnDemand { get; set; }

        /// <summary>
        /// Команда выполняемы при открытии ToolTip
        /// </summary>
        public static readonly DependencyProperty ShowCommandProperty;
        /// <summary>
        /// Команда выполняемы при закрытии ToolTip
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty;

        static ToolTipPlacementBehavior()
        {
            ShowCommandProperty = DependencyProperty.Register("ShowCommand", typeof(ICommand), typeof(ToolTipPlacementBehavior), new PropertyMetadata(null));
            CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(ToolTipPlacementBehavior), new PropertyMetadata(null));
        }


        /// <summary>
        /// OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            var toolTip = AssociatedObject.ToolTip as ToolTip;
            if (toolTip != null)
            {
                toolTip.Placement = PlacementMode.Custom;
                toolTip.CustomPopupPlacementCallback = CalculatePopupPlacement;
                toolTip.Opened += ToolTip_Opened;
                toolTip.Closed += ToolTip_Closed;
            }
        }
        protected override void OnDetaching()
        {
            var toolTip = AssociatedObject.ToolTip as ToolTip;
            if (toolTip != null)
            {
                toolTip.Opened -= ToolTip_Opened;
                toolTip.Closed -= ToolTip_Closed;
            }
            base.OnDetaching();
        }
        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {
            if (CloseCommand != null)
                if (CloseCommand.CanExecute(AssociatedObject?.DataContext))
                    CloseCommand.Execute(AssociatedObject?.DataContext);
        }

        private void ToolTip_Opened(object sender, RoutedEventArgs e)
        {
            if (ToolTipOnDemand)
            {
                var toolTip = sender as ToolTip;
                toolTip.Visibility = toolTip.ActualWidth < AssociatedObject.ActualWidth ? Visibility.Hidden : Visibility.Visible;
                //toolTip.MinWidth = AssociatedObject.ActualWidth;
                //toolTip.MinHeight = AssociatedObject.ActualHeight;
            }

            if (ShowCommand != null)
                if (ShowCommand.CanExecute(AssociatedObject?.DataContext))
                    ShowCommand.Execute(AssociatedObject?.DataContext);
        }
        /// <summary>
        /// Переопределение CustomPopupPlacement
        /// </summary>
        protected virtual CustomPopupPlacement[] CalculatePopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            var verticalOffsets = GetVerticalOffsets(VerticalPlacement, popupSize.Height, targetSize.Height, offset.Y);
            var horizontalOffsets = GetHorizontalOffsets(HorizontalPlacement, popupSize.Width, targetSize.Width, offset.X);

            var placement1 = new CustomPopupPlacement(new Point(horizontalOffsets[0], verticalOffsets[0]), PopupPrimaryAxis.Vertical);
            var placement2 = new CustomPopupPlacement(new Point(horizontalOffsets[1], verticalOffsets[1]), PopupPrimaryAxis.Horizontal);

            return new[] { placement1, placement2 };
        }

        private double[] GetHorizontalOffsets(HorizontalPlacement horizontalPlacement, double popupWidth, double targetWidth, double offsetWidth)
        {
            var horizontalOffsets = Enumerable.Repeat(offsetWidth, 2).ToArray();
            switch (horizontalPlacement)
            {
                case HorizontalPlacement.Left:
                    horizontalOffsets[0] += InvertDirections ? 0.0 : -popupWidth;
                    horizontalOffsets[1] += InvertDirections ? 0.0 : targetWidth;
                    break;

                case HorizontalPlacement.Center:
                    horizontalOffsets[0] += targetWidth / 2 - popupWidth / 2;
                    horizontalOffsets[1] = horizontalOffsets[0];
                    break;

                case HorizontalPlacement.Right:
                    horizontalOffsets[0] += InvertDirections ? targetWidth - popupWidth : targetWidth;
                    horizontalOffsets[1] += InvertDirections ? 0.0 : -popupWidth;
                    break;

                default:
                    throw new Exception("Invalid Vertical Placement");
            }

            return horizontalOffsets;
        }
        private static double[] GetVerticalOffsets(VerticalPlacement verticalPlacement, double popupHeight, double targetHeight, double offsetHeight)
        {
            var verticalOffsets = Enumerable.Repeat<double>(offsetHeight, 2).ToArray();

            switch (verticalPlacement)
            {
                case VerticalPlacement.Top:
                    verticalOffsets[0] += -popupHeight;
                    verticalOffsets[1] += targetHeight;
                    break;

                case VerticalPlacement.Center:
                    verticalOffsets[0] += targetHeight / 2 - popupHeight / 2;
                    verticalOffsets[1] += verticalOffsets[0];
                    break;

                case VerticalPlacement.Bottom:
                    verticalOffsets[0] += targetHeight;
                    verticalOffsets[1] += -popupHeight;
                    break;

                default:
                    throw new Exception("Invalid Vertical Placement");
            }
            return verticalOffsets;
        }

        /// <summary>
        /// Команда выполняемы при открытии ToolTip
        /// </summary>
        public ICommand ShowCommand
        {
            get { return (ICommand)GetValue(ShowCommandProperty); }
            set { SetValue(ShowCommandProperty, value); }
        }
        /// <summary>
        /// Команда выполняемы при закрытии ToolTip
        /// </summary>
        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }
    }
}
