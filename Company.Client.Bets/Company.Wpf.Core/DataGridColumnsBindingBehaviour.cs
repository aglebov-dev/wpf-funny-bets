using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace Company.Wpf.Core
{
    public class DataGridMetaItems
    {
        public Type DataType { get; set; }
        public Style ColumnStyle { get; set; }
        public Style CellStyle { get; set; }
        public DataTemplate ColumnTemplate { get; set; }
        public DataTemplate CellTemplate { get; set; }
    }

    [ContentProperty(nameof(MetaItems))]
    public class DataGridColumnsBindingBehaviour : Behavior<DataGrid>
    {
        public static readonly DependencyProperty AttachedColumnsProperty;
        private static readonly DependencyProperty InternalMarkerProperty;
        public static readonly DependencyProperty RowDataCollectionPathProperty;

        static DataGridColumnsBindingBehaviour()
        {
            var attachedColumnsMetaData = new FrameworkPropertyMetadata
            {
                DefaultValue = null,
                BindsTwoWayByDefault = false,
                PropertyChangedCallback = AttachedColumnsPropertyChanged
            };

            AttachedColumnsProperty = DependencyProperty.Register("AttachedColumns", typeof(IList), typeof(DataGridColumnsBindingBehaviour), attachedColumnsMetaData);
            RowDataCollectionPathProperty = DependencyProperty.Register("RowDataCollectionPath", typeof(string), typeof(DataGridColumnsBindingBehaviour), new PropertyMetadata(null));
            InternalMarkerProperty = DependencyProperty.RegisterAttached("InternalMarker", typeof(object), typeof(DataGridColumnsBindingBehaviour), new PropertyMetadata(null));
        }

        private static void AttachedColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behaviour = (DataGridColumnsBindingBehaviour)sender;
            var oldSource = args.OldValue as INotifyCollectionChanged;
            var newSource = args.NewValue as INotifyCollectionChanged;
            if (newSource != null) newSource.CollectionChanged += behaviour.ColumnsCollectionChanged;
            if (oldSource != null) oldSource.CollectionChanged -= behaviour.ColumnsCollectionChanged;

            behaviour.RemoveAttachColumns();
            behaviour.AppendAttachColumns();
        }

        //properties and fields

        /// <summary> коллекция колонок DataGrid </summary>
        ObservableCollection<DataGridColumn> InternalDataGridColumns;
        /// <summary> Буффер для хранения колонок пока грид инициализируется </summary>
        List<DataGridColumn> TempBuffer;

        /// <summary>
        /// Элементы для описания внешнего вида присоединенных столбцов
        /// </summary>
        public IList MetaItems { get; private set; }

        /// <summary>
        /// Динамические столбцы
        /// </summary>
        public IList AttachedColumns
        {
            get { return (IList)GetValue(AttachedColumnsProperty); }
            set { SetValue(AttachedColumnsProperty, value); }
        }
        /// <summary>
        /// Путь до коллекции дынных на которую выполняется паминг динамический столбцов (по индексу)
        /// </summary>
        public string RowDataCollectionPath
        {
            get { return (string)GetValue(RowDataCollectionPathProperty); }
            set { SetValue(RowDataCollectionPathProperty, value); }
        }

        //ctor
        public DataGridColumnsBindingBehaviour()
        {
            MetaItems = new List<DataGridMetaItems>();
            TempBuffer = new List<DataGridColumn>();

        }

        protected override void OnAttached()
        {
            InternalDataGridColumns = AssociatedObject.Columns;
            CheckTempBuffer();
            base.OnAttached();
        }
        protected override void OnDetaching()
        {
            RemoveAttachColumns();
            InternalDataGridColumns = null;
            AttachedColumns = null; //NOTE: если при байдинге указан mode=TwoWay то данная операция сотрет источник

            base.OnDetaching();
        }
        private void CheckTempBuffer()
        {
            for (int i = 0; i < TempBuffer.Count; i++)
                InternalDataGridColumns.Add(TempBuffer[i]);
            TempBuffer.Clear();
        }

        /// <summary> 
        /// добавить присоединенные столбцы в коллекцию колонок DataGrid
        /// </summary>
        private void AppendAttachColumns()
        {
            if (AttachedColumns != null)
            {
                for (int i = 0; i < AttachedColumns.Count; i++)
                {
                    CreateColumn(AttachedColumns[i]);
                }
            }
        }
        /// <summary> 
        /// удалить все присоединенные столбцы 
        /// </summary>
        private void RemoveAttachColumns()
        {
            if (InternalDataGridColumns != null)
            {
                var existItems = InternalDataGridColumns.Where(x => x.GetValue(InternalMarkerProperty) != null).ToArray();
                for (int i = 0; i < existItems.Length; i++)
                {
                    DestroyColumn(existItems[i]);
                }
            }
        }
        /// <summary> 
        /// обработчик изменения содержимого коллекции 
        /// </summary>
        private void ColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                var existItems = InternalDataGridColumns.Where(x => x.GetValue(InternalMarkerProperty) != null).ToArray();
                for (int i = 0; i < existItems.Length; i++)
                {
                    DestroyColumn(existItems[i]);
                }
            }

            if (args.NewItems != null)
            {
                for (int i = 0; i < args.NewItems.Count; i++)
                {
                    CreateColumn(args.NewItems[i]);
                }
            }

            if (args.OldItems != null)
            {
                for (int i = 0; i < args.OldItems.Count; i++)
                {
                    DestroyColumn(args.OldItems[i]);
                }
            }
        }


        /// <summary> 
        /// логика создания и добавления в DataGrid присоединенного столбца 
        /// </summary>
        private void CreateColumn(object column)
        {
            var dataGridColumn = column as DataGridColumn;
            if (dataGridColumn == null)
            {
                var columnMetaData = MetaItems.Cast<DataGridMetaItems>().LastOrDefault(x => x.DataType == column.GetType());

                dataGridColumn = new DataGridTemplateColumn
                {
                    Header = column,
                    CellTemplate = CreateInternalCellTemplate(column, columnMetaData)
                };

                if (columnMetaData != null)
                {
                    dataGridColumn.HeaderStyle = columnMetaData.ColumnStyle;
                    dataGridColumn.HeaderTemplate = columnMetaData.ColumnTemplate;
                    dataGridColumn.CellStyle = columnMetaData.CellStyle;
                }
            }

            //TODO надо как то различать одинаковые объекты (когда 2 столбца привязаны к одному объекту)
            dataGridColumn.SetValue(InternalMarkerProperty, new object());
            if (InternalDataGridColumns != null)
                InternalDataGridColumns.Add(dataGridColumn);
            else
                TempBuffer.Add(dataGridColumn);
        }

        DataTemplate CreateInternalCellTemplate(object column, DataGridMetaItems meta)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(ContentControl));
            var index = AttachedColumns.IndexOf(column);

            var bindingPath = GetBindingPath(index);
            var binding = new Binding()
            {
                Path = new PropertyPath(bindingPath),
                Mode = BindingMode.OneWay
            };

            factory.SetBinding(ContentControl.ContentProperty, binding);
            if (meta?.CellTemplate != null)
            {
                factory.SetValue(ContentControl.ContentTemplateProperty, meta.CellTemplate);
            }
            template.VisualTree = factory;

            return template;
        }

        string GetBindingPath(int index)
        {
            return string.IsNullOrWhiteSpace(RowDataCollectionPath)
                ? $"[{index}]"
                : $"{RowDataCollectionPath}[{index}]";
        }

        /// <summary> логика удаления из DataGrid присоединенного столбца </summary>
        private void DestroyColumn(object column)
        {
            var dataGridColumn = column as DataGridColumn;
            if (dataGridColumn == null)
            {
                dataGridColumn = InternalDataGridColumns.FirstOrDefault(x => x.Header == column);
            }

            if (dataGridColumn != null)
            {
                dataGridColumn.SetValue(InternalMarkerProperty, null);
                InternalDataGridColumns.Remove(dataGridColumn);
            }
        }
    }
}
