using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Company.Wpf.Core
{
    static class XamlUtils
    {
        /// <summary>
        /// Поиск родительского элемента
        /// </summary>
        /// <typeparam name="T">Тип родителя</typeparam>
        /// <param name="item">Объект относительно которого выполняется поиск</param>
        /// <returns>Найденный элемент</returns>
        public static T FindParent<T>(DependencyObject item) where T : DependencyObject
        {
            var curent = item;
            while (curent != null)
            {
                curent = GetParent(curent);
                if (curent is T)
                    return (T)curent;
            }

            return null;
        }

        public static DependencyObject GetParent(DependencyObject item) => item != null ? VisualTreeHelper.GetParent(item) : null;

        /// <summary>
        /// Поиск родительских элементов
        /// </summary>
        /// <typeparam name="T">Тип родителя</typeparam>
        /// <param name="item">Объект относительно которого выполняется поиск</param>
        /// <returns>Последовательность найденных элементов</returns>
        public static IEnumerable<T> FindParents<T>(DependencyObject item) where T : DependencyObject
        {
            var curent = item;
            while (curent != null)
            {
                curent = VisualTreeHelper.GetParent(curent);
                if (curent is T)
                    yield return (T)curent;
            }
        }
        /// <summary>
        /// Выполнить поиск первого найденного потомка родительского элемента (поиск по ширине)
        /// </summary>
        /// <typeparam name="T">Тип потомка</typeparam>
        /// <param name="source">Объект относительно которого выполняется поиск</param>
        /// <returns>Найденный элемент</returns>
        public static T FindFirstChild<T>(DependencyObject source) where T : DependencyObject
        {
            var queue = new Queue<DependencyObject>();
            var curent = source;
            while (curent != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(curent);
                for (int index = 0; index < count; index++)
                {
                    var child = VisualTreeHelper.GetChild(curent, index);
                    queue.Enqueue(child);
                }

                curent = queue.Count > 0 ? queue.Dequeue() : null;

                if (curent is T) return (T)curent;
            }

            return null;
        }
        /// <summary>
        /// Выполнить поиск потомков родительского элемента
        /// </summary>
        /// <typeparam name="T">Тип потомка</typeparam>
        /// <param name="source">Объект относительно которого выполняется поиск</param>
        /// <returns>Последовательность найденных элементов</returns>
        public static IEnumerable<T> FindChildren<T>(DependencyObject source) where T : DependencyObject
        {
            var queue = new Queue<DependencyObject>();
            var curent = source;
            while (curent != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(curent);
                for (int index = 0; index < count; index++)
                {
                    var child = VisualTreeHelper.GetChild(curent, index);
                    queue.Enqueue(child);
                }

                curent = queue.Count > 0 ? queue.Dequeue() : null;

                if (curent is T) yield return (T)curent;
            }
        }
        /// <summary>
        /// Выполнить поиск шаблона данных по визуальному дереву xaml
        /// </summary>
        /// <typeparam name="TData">Тип для которого осуществляется поиск ресурсов</typeparam>
        /// <param name="instance">Объект относительно которого выполняется поиск</param>
        /// <returns>Шаблон данных</returns>
        public static DataTemplate FindDataTemplate<TData>(FrameworkElement instance)
        {
            return FindDataTemplate(instance, typeof(TData));
        }
        /// <summary>
        /// Выполнить поиск шаблона данных по визуальному дереву xaml
        /// </summary>
        /// <param name="instance">Объект относительно которого выполняется поиск</param>
        /// <param name="dataType">Тип для которого осуществляется поиск ресурсов</param>
        /// <returns>Шаблон данных</returns>
        public static DataTemplate FindDataTemplate(FrameworkElement instance, Type dataType)
        {
            var key = new DataTemplateKey(dataType);
            return instance.TryFindResource(key) as DataTemplate;
        }
        /// <summary>
        /// Удаляет визуальный элемент
        /// </summary>
        /// <param name="parent">Родитель</param>
        /// <param name="child">Удаляемый элемент</param>
        /// <returns>индекс удаленного элемента если родитель списковый контрол или null для всех остальных</returns>
        public static int? RemoveChild(DependencyObject parent, UIElement child)
        {
            var type = parent.GetType();
            if (parent is Panel)
            {
                ((Panel)parent).Children.Remove(child);
            }
            else if (parent is Decorator)
            {
                ((Decorator)parent).Child = null;
            }
            else if (parent is ContentControl)
            {
                ((ContentControl)parent).Content = null;
            }
            else if (parent is ContentPresenter)
            {
                ((ContentPresenter)parent).Content = null;
            }
            else if (parent is ItemsControl)
            {
                var items = ((ItemsControl)parent).Items;
                var index = items.IndexOf(child);
                if (index >= 0)
                {
                    items.RemoveAt(index);
                    return index;
                }
            }

            return null;
        }
        /// <summary>
        /// Дабавляет визуальный элемент
        /// </summary>
        /// <param name="parent">Родитель</param>
        /// <param name="child">Удаляемый элемент</param>
        /// <param name="index">Индекс вставки для списковых элементов</param>
        public static void AddChild(DependencyObject parent, UIElement child, int? index = null)
        {
            //Panel, Decorator, ContentControl, ContentPresenter, ItemsControl
            var type = parent.GetType();
            if (parent is Panel)
            {
                ((Panel)parent).Children.Add(child);
            }
            else if (parent is Decorator)
            {
                ((Decorator)parent).Child = child;
            }
            else if (parent is ContentControl)
            {
                ((ContentControl)parent).Content = child;
            }
            else if (parent is ContentPresenter)
            {
                ((ContentPresenter)parent).Content = child;
            }
            else if (parent is ItemsControl)
            {
                var items = ((ItemsControl)parent).Items;
                if (items != null)
                    items.Insert(index.Value, child);
                else
                    items.Add(child);
            }
        }
        /// <summary>
        /// Получить все с attach свойства, определенные в типе
        /// </summary>
        /// <param name="instance">объект по типу которого ведется поиск свойст</param>
        /// <param name="includeReadOnlyProperties">включить в результат свойства только для чтения</param>
        /// <returns></returns>
        public static DependencyProperty[] GetAttachedProperties(DependencyObject instance)
        {
            var result = new LinkedList<DependencyProperty>();
            if (instance != null)
            {
                var properties = TypeDescriptor
                  .GetProperties(instance, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) })
                  .Cast<PropertyDescriptor>()
                  .ToArray();

                foreach (var descriptor in properties)
                {
                    var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(descriptor);

                    if (dependencyPropertyDescriptor != null && dependencyPropertyDescriptor.IsAttached)
                    {
                        result.AddLast(dependencyPropertyDescriptor.DependencyProperty);
                    }
                }
            }

            return result.ToArray();
        }
        /// <summary>
        /// Спопировать attach свойства
        /// </summary>
        /// <param name="properties">свойства</param>
        /// <param name="source">источник коппирования</param>
        /// <param name="target">цель коппирования</param>
        public static void CoppyAttachedProperties(DependencyProperty[] properties, DependencyObject source, DependencyObject target)
        {
            foreach (var property in properties)
            {
                if (!property.ReadOnly)
                {
                    var value = source.GetValue(property);
                    target.SetValue(property, value);
                }
            }
        }
    }
}
