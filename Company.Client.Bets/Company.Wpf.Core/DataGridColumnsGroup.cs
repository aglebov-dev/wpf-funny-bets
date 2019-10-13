using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Company.Wpf.Core
{
    public static class DataGridColumnsGroup
    {
        public static object GetGroupData(DependencyObject obj) => (obj.GetValue(GroupDataProperty));
        public static void SetGroupData(DependencyObject obj, object value) => obj.SetValue(GroupDataProperty, value);

        public static readonly DependencyProperty GroupDataProperty =
            DependencyProperty.RegisterAttached("GroupData", typeof(object), typeof(DataGridColumnsGroup), new PropertyMetadata(null));
    }

    public enum ColumnsGroupInjectMode
    {
        Default,
        Footer,
        Header
    }

    public class DataGridColumnsGroupBehaviour : Behavior<DataGrid>
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplateSelector DataTemplateSelector { get; set; }
        public ColumnsGroupInjectMode InjectMode { get; set; }

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.OnAttached();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;

            var headersPresenter = XamlUtils.FindFirstChild<DataGridColumnHeadersPresenter>(AssociatedObject);

            var presenterParent = default(FrameworkElement);
            var presenter = default(FrameworkElement);

            if (InjectMode == ColumnsGroupInjectMode.Footer)
            {
                presenter = XamlUtils.FindFirstChild<ScrollContentPresenter>(AssociatedObject) as FrameworkElement;
                presenterParent = presenter.Parent as FrameworkElement;
            }
            else
            {
                presenter = headersPresenter;
                presenterParent = headersPresenter?.Parent as FrameworkElement;
            }

            if (presenterParent != null)
            {
                var index = XamlUtils.RemoveChild(presenterParent, presenter);

                var properties = XamlUtils.GetAttachedProperties(presenterParent);

                var dock = new DockPanel() {
                    LastChildFill = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top
                };
                var groupPressenter = new ColumnGroupPresentor(AssociatedObject, headersPresenter, this, InjectMode);

                if (InjectMode == ColumnsGroupInjectMode.Footer)
                {
                    var hLine = new Rectangle
                    {
                        Height = 1,
                        Fill = AssociatedObject.HorizontalGridLinesBrush,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    DockPanel.SetDock(hLine, Dock.Bottom);
                    dock.Children.Add(hLine);
                }


                var groupDock = InjectMode == ColumnsGroupInjectMode.Default ? Dock.Top : Dock.Bottom;
                var columnsDock = InjectMode == ColumnsGroupInjectMode.Default ? Dock.Bottom : Dock.Top;


                DockPanel.SetDock(groupPressenter, groupDock);
                DockPanel.SetDock(presenter, columnsDock);
                dock.Children.Add(groupPressenter);
                dock.Children.Add(presenter);

                XamlUtils.CoppyAttachedProperties(properties, presenter, dock);
                XamlUtils.AddChild(presenterParent, dock, index);
            }
        }
    }


    internal class ColumnGroupPresentor : Panel
    {
        DataGrid OwnerDataGrid;
        DataGridColumnsGroupBehaviour OwnerBehaviour;
        DataGridColumnHeadersPresenter TargetDataGridColumnHeadersPresenter;
        DataGridCellsPanel TargetDataGridCellsPanel { get; set; }

        //bool IsFooter { get; set; }
        ColumnsGroupInjectMode InjectMode { get; set; }

        IEnumerable<DataGridColumnHeader> QueryColumnHeaders;
        IEnumerable<InformationMetaItem> QueryActualInformationMetaItems;
        IEnumerable<InformationMetaItem> QueryFixedSizeItems;
        List<InformationMetaItem> InformationMetaItems;



        public ColumnGroupPresentor(
            DataGrid ownerDataGrid,
            DataGridColumnHeadersPresenter ownerDataGridColumnHeadersPresenter,
            DataGridColumnsGroupBehaviour ownerBehaviour,
            ColumnsGroupInjectMode injectMode)
        {
            InjectMode = injectMode;
            OwnerDataGrid = ownerDataGrid;
            OwnerBehaviour = ownerBehaviour;
            TargetDataGridColumnHeadersPresenter = ownerDataGridColumnHeadersPresenter;
            TargetDataGridCellsPanel = XamlUtils.FindFirstChild<DataGridCellsPanel>(ownerDataGridColumnHeadersPresenter);

            InitPanel();
            InformationMetaItems = new List<InformationMetaItem>();
            QueryActualInformationMetaItems = InformationMetaItems
                .Where(x => x.Source.Column != null)
                .Where(x => x.Index > -1)
                .Where(x => x.Source.Visibility == Visibility.Visible)
                .OrderBy(x => x.Index);
            QueryFixedSizeItems = Enumerable.Empty<InformationMetaItem>();
            QueryColumnHeaders = TargetDataGridCellsPanel.Children.OfType<DataGridColumnHeader>();

            Loaded += OnLoaded;
        }

        private void InitPanel()
        {
            if (TargetDataGridCellsPanel != null)
            {
                var notifyCollection = (TargetDataGridColumnHeadersPresenter.Items as INotifyCollectionChanged);
                if (notifyCollection != null)
                {
                    notifyCollection.CollectionChanged += delegate { UpdateAcion(); };
                }
                else
                {
                    //если колекция без уведомлений, то наверно придется подменять панельку с хидерами
                    //это более долгая операция т.к. влияет на переотрисовку (влияет на скорость загрузки UI)
                    var factory = new FrameworkElementFactory(typeof(DataGridCellsPanelEx));
                    factory.SetValue(IsItemsHostProperty, true);
                    TargetDataGridColumnHeadersPresenter.ItemsPanel = new ItemsPanelTemplate(factory);
                    TargetDataGridColumnHeadersPresenter.UpdateLayout();

                    //переопределяем панель
                    TargetDataGridCellsPanel = XamlUtils.FindFirstChild<DataGridCellsPanelEx>(TargetDataGridColumnHeadersPresenter);
                    ((DataGridCellsPanelEx)TargetDataGridCellsPanel).ItemsChange += delegate { UpdateAcion(); };
                }
            }
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            SetOnInitializedBindings();
            UpdateAcion();
        }
        private void UpdateAcion()
        {
            if (IsLoaded)
            {
                TargetDataGridCellsPanel.UpdateLayout();
                UpdateInformationMetaItems();
                UpdateGroupItems();
            }
        }
        private void SetOnInitializedBindings()
        {
            var scrollViewer = XamlUtils.FindParent<ScrollViewer>(TargetDataGridColumnHeadersPresenter);
            if (scrollViewer != null)
            {
                var binding = new Binding
                {
                    Source = scrollViewer,
                    Path = new PropertyPath(ScrollViewer.ContentHorizontalOffsetProperty),
                    Mode = BindingMode.OneWay
                };
                SetBinding(ContentHorizontalOffsetProperty, binding);
            }

            //перерисовка
            var binding2 = new Binding
            {
                Source = OwnerDataGrid,
                Path = new PropertyPath(DataGrid.FrozenColumnCountProperty),
                Mode = BindingMode.OneWay
            };
            SetBinding(FrozenColumnsCountProperty, binding2);
        }


        #region Dp

        static readonly DependencyProperty ContentHorizontalOffsetProperty;
        static readonly DependencyProperty FrozenColumnsCountProperty;

        static ColumnGroupPresentor()
        {
            var hOffsetMeta = new PropertyMetadata(0d, ContentHorizontalOffsetChange);
            var frozenMeta = new PropertyMetadata(0, FrozenColumnsCountChanged);

            ContentHorizontalOffsetProperty = DependencyProperty.Register("ContentHorizontalOffset", typeof(double), typeof(ColumnGroupPresentor), hOffsetMeta);
            FrozenColumnsCountProperty = DependencyProperty.Register("FrozenColumnsCount", typeof(int), typeof(ColumnGroupPresentor), frozenMeta);
        }

        private static void FrozenColumnsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = ((ColumnGroupPresentor)d);
            sender.QueryFixedSizeItems = sender.QueryActualInformationMetaItems.Take(sender.FrozenColumnsCount);
            sender.UpdateMeasureAndArrange();
        }

        private static void ContentHorizontalOffsetChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnGroupPresentor)sender).UpdateMeasureAndArrange();
        }

        double ContentHorizontalOffset
        {
            get { return (double)GetValue(ContentHorizontalOffsetProperty); }
            set { SetValue(ContentHorizontalOffsetProperty, value); }
        }
        int FrozenColumnsCount
        {
            get { return (int)GetValue(FrozenColumnsCountProperty); }
            set { SetValue(FrozenColumnsCountProperty, value); }
        }

        #endregion

        void UpdateMeasureAndArrange()
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        //create new size items //TODO Merge
        void UpdateInformationMetaItems()
        {
            ClearInformationMetaItems();
            foreach (var item in QueryColumnHeaders.ToList())
            {
                var info = new InformationMetaItem(item);
                info.WidthChange += HeaderWidthChange;
                info.IndexChange += HeaderIndexChange;
                InformationMetaItems.Add(info);
            }
        }
        void ClearInformationMetaItems()
        {
            foreach (var info in InformationMetaItems)
            {
                BindingOperations.ClearAllBindings(info);
                info.WidthChange -= HeaderWidthChange;
                info.IndexChange -= HeaderIndexChange;
            }
            InformationMetaItems.Clear();
        }
        void HeaderIndexChange(object sender, int e) => UpdateGroupItems();
        void HeaderWidthChange(object sender, double e) => UpdateMeasureAndArrange();

        //create footer header items //TODO Merge
        private void UpdateGroupItems()//создаем контент
        {
            //просто создаем контент без всякой там размерности
            //сначала надо определиться что делать с тем контентом который уже есть
            Children.Clear();

            var informationItems = QueryActualInformationMetaItems.ToList();
            var previewGroupData = default(object);
            var curentPressentor = default(ContentPresenter);
            foreach (var informationItem in informationItems)
            {
                var curentGroupData = informationItem.GetGroupData();

                if (previewGroupData == null || !previewGroupData.Equals(curentGroupData))
                {
                    curentPressentor = new ContentPresenter { Content = curentGroupData };
                    var datatemplate = GetContentTemplate(curentGroupData, curentPressentor);
                    curentPressentor.ContentTemplate = datatemplate;

                    InternalChildren.Add(curentPressentor);
                }

                informationItem.Visual = curentPressentor;

                previewGroupData = curentGroupData;
            }
        }

        private DataTemplate GetContentTemplate(object curentGroupData, DependencyObject container)
        {
            var dataTemplate = default(DataTemplate);

            if (OwnerBehaviour != null)
            {
                dataTemplate = OwnerBehaviour.DataTemplateSelector?.SelectTemplate(curentGroupData, container) ??
                    OwnerBehaviour.DefaultDataTemplate;
            }

            //создаем шаблон по умолчанию
            if (dataTemplate == null && curentGroupData != null)
            {
                dataTemplate = TryFindResource(curentGroupData.GetType()) as DataTemplate;
                if (dataTemplate == null)
                {
                    var textFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textFactory.SetBinding(TextBlock.TextProperty, new Binding());
                    textFactory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                    textFactory.SetValue(MarginProperty, new Thickness(3));

                    var borderFactory = new FrameworkElementFactory(typeof(Border));
                    borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 1, 0));
                    borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Gray));
                    borderFactory.AppendChild(textFactory);

                    dataTemplate = new DataTemplate();
                    dataTemplate.VisualTree = borderFactory;
                }

            }
            return dataTemplate;
        }

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0 || IsLoaded == false)
                return base.MeasureOverride(availableSize);

            foreach (UIElement child in InternalChildren.OfType<ContentPresenter>())
            {
                //TODO - расширить UI контейнер доп. данными для уменьшения расчетов
                //тут нам важен элемент который пересекает границу
                var INTERSECT = QueryFixedSizeItems.Intersect(QueryActualInformationMetaItems.Where(x => x.Visual == child));

                if (INTERSECT.Any()) //это итемы которые находятся (полностью или частично) в зоне заморозки
                {
                    var EXPECT = QueryActualInformationMetaItems.Where(x => x.Visual == child).Except(INTERSECT); //разность
                    var head = INTERSECT.Aggregate(0d, (s, x) => s += x.Width);
                    var tail = EXPECT.Aggregate(0d, (s, x) => s += x.Width);
                    var delta = tail - (InjectMode == ColumnsGroupInjectMode.Footer ? 0 : ContentHorizontalOffset);
                    var size = new Size
                    {
                        Width = head + (delta > 0 ? delta : 0),
                        Height = availableSize.Height
                    };

                    child.Measure(size);
                }
                else
                {
                    var w = QueryActualInformationMetaItems.Where(x => x.Visual == child).Aggregate(0d, (s, x) => s += x.Width);
                    child.Measure(new Size { Width = w, Height = availableSize.Height });

                }
            }

            var width = QueryActualInformationMetaItems.Aggregate(0d, (s, x) => s += x.Width);
            var height = InternalChildren.OfType<ContentPresenter>().Max(x => x.DesiredSize.Height);
            return new Size { Width = width, Height = height };
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0 || IsLoaded == false)
                return base.ArrangeOverride(finalSize);

            double offset = 0d;
            foreach (UIElement child in InternalChildren.OfType<ContentPresenter>())
            {
                var INTERSECT = QueryFixedSizeItems.Intersect(QueryActualInformationMetaItems.Where(x => x.Visual == child)); //пересечение
                if (INTERSECT.Any()) //это итемы которые находятся (полностью или частично) в зоне заморозки
                {
                    var EXPECT = QueryActualInformationMetaItems.Where(x => x.Visual == child).Except(INTERSECT); //разность

                    var head = INTERSECT.Aggregate(0d, (s, x) => s += x.Width);
                    var tail = EXPECT.Aggregate(0d, (s, x) => s += x.Width);
                    var delta = tail - (InjectMode == ColumnsGroupInjectMode.Footer ? 0 : ContentHorizontalOffset);
                    var size = new Size
                    {
                        Width = head + (delta > 0 ? delta : 0),
                        Height = finalSize.Height
                    };

                    var point = new Point
                    {
                        X = offset + (InjectMode == ColumnsGroupInjectMode.Footer ? ContentHorizontalOffset : 0),
                        Y = 0
                    };

                    child.Arrange(new Rect(point, size));
                    offset = offset + head + tail;
                }
                else
                {
                    var size = new Size
                    {
                        Width = QueryActualInformationMetaItems.Where(x => x.Visual == child).Aggregate(0d, (s, x) => s += x.Width),
                        Height = finalSize.Height
                    };

                    var point = new Point
                    {
                        X = offset - (InjectMode == ColumnsGroupInjectMode.Footer ? 0 : ContentHorizontalOffset),
                        Y = 0
                    };

                    child.Arrange(new Rect(point, size));
                    offset = offset + size.Width;

                    #region clip
                    var fixOffset = QueryFixedSizeItems.Aggregate(0d, (s, x) => s += x.Width) + (InjectMode == ColumnsGroupInjectMode.Footer ? ContentHorizontalOffset : 0);
                    if (point.X <= fixOffset)
                    {
                        var clipPoint = new Point { X = Math.Min(fixOffset - point.X, size.Width), Y = 0 };
                        var clipWidth = size.Width - clipPoint.X;

                        var clipSize = new Size { Width = clipWidth, Height = size.Height };
                        child.Clip = new RectangleGeometry { Rect = new Rect(clipPoint, clipSize) };
                    }
                    else
                        child.Clip = null;
                    #endregion
                }
            }

            return finalSize;
        }

        #endregion


        /// <summary>
        /// Панель подменяющая
        /// </summary>
        private class DataGridCellsPanelEx : DataGridCellsPanel
        {
            protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
            {
                base.OnItemsChanged(sender, args);
                ItemsChange?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler ItemsChange;
        }
    }

    internal class InformationMetaItem : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty WidthProperty;
        public static readonly DependencyProperty DisplayIndexProperty;

        static InformationMetaItem()
        {
            WidthProperty = FrameworkElement.WidthProperty.AddOwner(typeof(InformationMetaItem), new FrameworkPropertyMetadata(0d) { BindsTwoWayByDefault = false, PropertyChangedCallback = WidthChanged });
            DisplayIndexProperty = DependencyProperty.Register("DisplayIndex", typeof(int), typeof(InformationMetaItem), new PropertyMetadata(-1, DisplayIndexChanged));
        }

        static void WidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InformationMetaItem).WidthChange?.Invoke(d, (double)e.NewValue);
        }
        static void DisplayIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as InformationMetaItem).IndexChange?.Invoke(d, (int)e.NewValue);
        }

        public double Width => (double)GetValue(WidthProperty) + Source.Margin.Left + Source.Margin.Right;
        public int Index => (int)GetValue(DisplayIndexProperty);

        public event EventHandler<double> WidthChange;
        public event EventHandler<int> IndexChange;

        public DataGridColumnHeader Source { get; set; }
        public ContentPresenter Visual { get; internal set; }

        public InformationMetaItem(DataGridColumnHeader source)
        {
            Source = source;
            CreateBindings();
        }
        /// <summary>
        /// Получить объект данных для группировки столбцов
        /// </summary>
        public object GetGroupData()
        {
            return DataGridColumnsGroup.GetGroupData(Source) ?? DataGridColumnsGroup.GetGroupData(Source.Column);
        }
        private void CreateBindings()
        {
            var b1 = new Binding { Source = Source, Path = new PropertyPath(FrameworkElement.ActualWidthProperty), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, InformationMetaItem.WidthProperty, b1);

            var b2 = new Binding { Source = Source, Path = new PropertyPath(DataGridColumnHeader.DisplayIndexProperty), Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(this, InformationMetaItem.DisplayIndexProperty, b2);
        }

        public void Dispose()
        {
            BindingOperations.ClearAllBindings(this);
        }
    }

}
