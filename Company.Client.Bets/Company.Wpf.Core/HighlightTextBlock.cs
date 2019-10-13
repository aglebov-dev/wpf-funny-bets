using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Company.Wpf.Core
{
    /// <summary>
    /// Текстовый UI контрол предоставляющий подсветку заднего фона текста
    /// </summary>
    public class HighlightTextBlock : FrameworkElement
    {
        //text
        /// <summary>
        /// Текст
        /// </summary>
        public static readonly DependencyProperty TextProperty;
        //HighlightWords
        /// <summary>
        /// Коллекция слов которые необзодимо подсветить
        /// </summary>
        public static readonly DependencyProperty HighlightWordsProperty;
        //Typeface
        /// <summary>
        /// FontStretchProperty
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty;
        /// <summary>
        /// FontWeightProperty
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty;
        /// <summary>
        /// FontStyleProperty
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty;
        /// <summary>
        /// FontFamilyProperty
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty;
        //text style
        /// <summary>
        /// FontSizeProperty
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty;
        /// <summary>
        /// TextWrappingProperty
        /// </summary>
        public static readonly DependencyProperty TextWrappingProperty;
        /// <summary>
        /// TextAlignmentProperty
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty;
        /// <summary>
        /// TextTrimmingProperty
        /// </summary>
        public static readonly DependencyProperty TextTrimmingProperty;
        //colors
        /// <summary>
        /// ForegroundProperty
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty;
        /// <summary>
        /// BackgroundProperty
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty;
        /// <summary>
        /// BackgroundHighlightProperty
        /// </summary>
        public static readonly DependencyProperty BackgroundHighlightProperty;
        /// <summary>
        /// ForegroundHighlightProperty
        /// </summary>
        public static readonly DependencyProperty ForegroundHighlightProperty;
        //layout
        /// <summary>
        /// PaddingProperty
        /// </summary>
        public static readonly DependencyProperty PaddingProperty;

        static HighlightTextBlock()
        {
            //text
            TextProperty = DependencyProperty.Register("Text", typeof(string), internalType, new FrameworkPropertyMetadata(null) { PropertyChangedCallback = TextChanged });
            //HighlightWords
            HighlightWordsProperty = DependencyProperty.Register("HighlightWords", typeof(ObservableCollection<string>), internalType, new FrameworkPropertyMetadata(null) { PropertyChangedCallback = StaticHighlightWordsChanged });
            //Typeface
            FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), internalType, new FrameworkPropertyMetadata(FontStretches.Normal, RenderText) { PropertyChangedCallback = TextParamChanged });
            FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), internalType, new FrameworkPropertyMetadata(FontWeights.Normal, RenderText) { PropertyChangedCallback = TextParamChanged });
            FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), internalType, new FrameworkPropertyMetadata(FontStyles.Normal, RenderText) { PropertyChangedCallback = TextParamChanged });
            FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), internalType, new FrameworkPropertyMetadata(new FontFamily("Segoe UI"), RenderText) { PropertyChangedCallback = TextParamChanged });
            //text
            FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), internalType, new FrameworkPropertyMetadata(12d, RenderText) { PropertyChangedCallback = TextParamChanged });
            TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), internalType, new FrameworkPropertyMetadata(TextWrapping.NoWrap, RenderText) { PropertyChangedCallback = TextParamChanged });
            TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), internalType, new FrameworkPropertyMetadata(TextAlignment.Left, RenderText) { PropertyChangedCallback = TextParamChanged });
            TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), internalType, new FrameworkPropertyMetadata(TextTrimming.CharacterEllipsis, RenderText) { PropertyChangedCallback = TextParamChanged });
            //colors
            ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), internalType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), RenderText) { PropertyChangedCallback = TextParamChanged });
            BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), internalType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent), RenderText));
            BackgroundHighlightProperty = DependencyProperty.Register("BackgroundHighlight", typeof(Brush), internalType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Yellow), RenderText));
            ForegroundHighlightProperty = DependencyProperty.Register("ForegroundHighlight", typeof(Brush), internalType, new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), RenderText));
            //layout
            PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), internalType, new FrameworkPropertyMetadata(new Thickness(1), ArrangeFlags) { PropertyChangedCallback = TextParamChanged });
        }
        /// <summary>
        /// Текстовый UI контрол предоставляющий подсветку заднего фона текста
        /// </summary>
        public HighlightTextBlock()
        {
            Loaded += HighlightTextBlock_Loaded;
        }

        void HighlightTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }
        /// <summary>
        /// override OnInitialized
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            OnApplyTemplate();
        }
        /// <summary>
        /// override OnApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            var style = TryFindResource(typeof(HighlightTextBlock));
            if (style as Style != null)
                Style = (Style)style;
        }

        #region FormattedText

        FormattedText _formattedText;
        Point _textPoint;
        double _maxTextWidth;
        double _maxTextHeight;

        void CreateFormatedText(string text)
        {
            _formattedText = new FormattedText(text ?? string.Empty, CultureInfo.GetCultureInfo("ru-ru"), FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                Foreground,
                1.0);
            
            _formattedText.TextAlignment = TextAlignment;
            _formattedText.Trimming = TextTrimming;
            _formattedText.MaxLineCount = TextWrapping == TextWrapping.NoWrap ? 1 : int.MaxValue;
            _textPoint = new Point(Padding.Left, Padding.Top);
            _maxTextWidth = _formattedText.Width + 1; //почемуто происходит трим текста (не хватает 1 пикселя)
            _maxTextHeight = _formattedText.Height;

            InvalidateVisual();
        }

        #endregion

        #region OnRender
        /// <summary>
        /// override OnRender
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            //SnapsToDevicePixels for background
            var snapX = new double[] { 0, ActualWidth };
            var snapY = new double[] { 0, ActualHeight };
            drawingContext.PushGuidelineSet(new GuidelineSet(snapX, snapY));

            //background
            drawingContext.DrawRectangle(Background, new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            //Clip
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));


            var maxTextWidth = PositiveValue(ActualWidth - Padding.Left - Padding.Right);
            if (_formattedText == null || maxTextWidth == 0)
                return;

            _formattedText.MaxTextWidth = maxTextWidth;
            _formattedText.SetForegroundBrush(Foreground);

            if (HighlightWords?.Count > 0)
            {
                var geometry = GetHighlightWordGeometry(_formattedText, _textPoint);
                geometry.ForEach(x =>
                {
                    var bounds = x.BackgroundGeometry.Bounds;
                    //SnapsToDevicePixels for Highlight
                    snapX = new double[] { bounds.Left, bounds.Right };
                    snapY = new double[] { bounds.Top, bounds.Bottom };
                    drawingContext.PushGuidelineSet(new GuidelineSet(snapX, snapY));
                    //Back Highligh
                    drawingContext.DrawGeometry(BackgroundHighlight, null, x.BackgroundGeometry);
                    //Text Highligh
                    _formattedText.SetForegroundBrush(ForegroundHighlight, x.Startindex, x.Count);
                });
            }
            drawingContext.DrawText(_formattedText, _textPoint);
        }

        private double PositiveValue(double value) => value > 0 ? value : 0;

        List<GeometryData> GetHighlightWordGeometry(FormattedText formattedText, Point textPoint)
        {
            var geometryList = new List<GeometryData>(HighlightWords.Count * 2);

            foreach (var word in HighlightWords)
            {
                var txtEnd = Text.Length;
                var curentIndex = 0;
                var lastIndex = Text.LastIndexOf(word, StringComparison.OrdinalIgnoreCase);
                while (curentIndex <= lastIndex)
                {
                    curentIndex = Text.IndexOf(word, curentIndex, StringComparison.OrdinalIgnoreCase);
                    var geometry = formattedText.BuildHighlightGeometry(textPoint, curentIndex, word.Length);
                    if (geometry != null)
                        geometryList.Add(new GeometryData(geometry, curentIndex, word.Length));
                    curentIndex += word.Length;
                }
            }

            return geometryList;
        }

        struct GeometryData
        {
            public Geometry BackgroundGeometry;
            public int Startindex;
            public int Count;
            public GeometryData(Geometry backgroundGeometry, int startindex, int count)
            {
                BackgroundGeometry = backgroundGeometry;
                Startindex = startindex;
                Count = count;
            }
        }

        #endregion

        #region Alignment (Measure)
        /// <summary>
        /// override MeasureOverride
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_formattedText == null)
                return availableSize;

            var textWidth = _maxTextWidth + Padding.Left + Padding.Right;
            var textHeight = _maxTextHeight + Padding.Top + Padding.Bottom;

            var size = new Size
            {
                Width = GetControlWidth(textWidth, availableSize.Width),
                Height = GetControlHeight(textHeight, availableSize.Height)
            };
            InvalidateArrange();
            return size;
        }
        double GetControlWidth(double textWidth, double availableWidth)
        {
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
                return double.IsInfinity(availableWidth) ? textWidth : Math.Min(availableWidth, textWidth);
            else
                return textWidth < availableWidth ? textWidth : availableWidth;
        }
        double GetControlHeight(double textHeight, double availableHeight)
        {
            if (VerticalAlignment == VerticalAlignment.Stretch)
                return double.IsInfinity(availableHeight) ? textHeight : Math.Min(availableHeight, textHeight);
            else
                return textHeight < availableHeight ? textHeight : availableHeight;
        }
        #endregion

        //Dependency Properties

        static Type internalType = typeof(HighlightTextBlock);
        static FrameworkPropertyMetadataOptions ArrangeFlags =
            FrameworkPropertyMetadataOptions.AffectsArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.AffectsRender;

        static FrameworkPropertyMetadataOptions RenderText = FrameworkPropertyMetadataOptions.AffectsRender;


        //HighlightWords
        static void StaticHighlightWordsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as HighlightTextBlock;
            var _old = e.OldValue as ObservableCollection<string>;
            var _new = e.NewValue as ObservableCollection<string>;
            if (_old != null)
                _old.CollectionChanged -= sender.HighlightWordsChanged;
            if (_new != null)
                _new.CollectionChanged += sender.HighlightWordsChanged;
            sender.InvalidateVisual();
        }

        void HighlightWordsChanged(object sender, NotifyCollectionChangedEventArgs e)
            => InvalidateVisual();

        //Text
        static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => (d as HighlightTextBlock).CreateFormatedText(e.NewValue?.ToString() ?? string.Empty);


        private static void TextParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (d as HighlightTextBlock);
            sender.CreateFormatedText(sender.Text);
        }

        #region Dependency property access methods

        /// <summary>
        /// Коллекция подсвеченных слов 
        /// </summary>
        public ObservableCollection<string> HighlightWords
        {
            get { return (ObservableCollection<string>)GetValue(HighlightWordsProperty); }
            set { SetValue(HighlightWordsProperty, value); }
        }
        /// <summary>
        /// Текст
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        /// Цвет подсвеченного текста
        /// </summary>
        public Brush BackgroundHighlight
        {
            get { return (Brush)GetValue(BackgroundHighlightProperty); }
            set { SetValue(BackgroundHighlightProperty, value); }
        }
        /// <summary>
        /// Цвет текста, в подсвеченном состоянии
        /// </summary>
        public Brush ForegroundHighlight
        {
            get { return (Brush)GetValue(ForegroundHighlightProperty); }
            set { SetValue(ForegroundHighlightProperty, value); }
        }
        /// <summary>
        /// Цвет текста
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        /// <summary>
        /// Фон текста
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        /// <summary>
        /// Перенос слов
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        /// <summary>
        /// Выравнивание текста
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        /// <summary>
        /// Урезание текста
        /// </summary>
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }
        /// <summary>
        /// Размер текста
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        /// <summary>
        /// Отступ текста
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
        /// <summary>
        /// Размещение текста
        /// </summary>
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }
        /// <summary>
        /// ТОлщина текста
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }
        /// <summary>
        /// Стиль текста
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }
        /// <summary>
        /// Шрифт текста
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }
        #endregion

    }
}
