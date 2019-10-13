using System;
using System.Collections.Generic;
using System.Linq;

namespace Company.Client.Common.Support
{
    public class SubscriptionArg
    {
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Количество обсерверов в контейнере по ключу
        /// </summary>
        public int CountOfExistingObservers { get; }
        /// <summary>
        /// Добавленный/исключенный обсервер
        /// </summary>
        public object Observer { get; }

        public SubscriptionArg(string key, int count, object observer)
        {
            Key = key;
            CountOfExistingObservers = count;
            Observer = observer;
        }
    }

    /// <summary>
    /// Контейнер для подписок
    /// </summary>
    public class SubscriptionContainer : IDisposable
    {
        List<object> Containers;

        public event Action<SubscriptionArg> SubscriptionRemoved;
        public event Action<SubscriptionArg> SubscriptionCreated;

        public SubscriptionContainer()
        {
            Containers = new List<object>();
        }

        private void SubscriptionRemovedInternal<T>(KeySubscriptionContainer<T> container, T observer, string key) where T : class
        {
            if (SubscriptionRemoved != null)
            {
                var count = container.Count(key);
                if (container.Count() == 0)
                {
                    container.SubscriptionRemoved -= SubscriptionRemovedInternal;
                    Containers.Remove(container);
                }
                var arg = new SubscriptionArg(key, count, observer);
                SubscriptionRemoved?.Invoke(arg);
            }
        }
        public IDisposable AddObserver<T>(string key, T observer) where T : class
        {
            var container = Containers.OfType<KeySubscriptionContainer<T>>().FirstOrDefault();
            if (container == null)
            {
                container = new KeySubscriptionContainer<T>();
                Containers.Add(container);
                container.SubscriptionRemoved += SubscriptionRemovedInternal;
            }
            var subscription = container.AddItem(key, observer);
            var count = container.Count(key);
            var arg = new SubscriptionArg(key, count, observer);
            SubscriptionCreated?.Invoke(arg);

            return subscription;
        }
        public T[] GetObservers<T>(string key) where T : class
        {
            var container = Containers.OfType<KeySubscriptionContainer<T>>().FirstOrDefault();
            if (container != null)
            {
                return container.GetItems(key);
            }

            return new T[] { };
        }
        public int Count<T>(string key) where T : class
        {
            var container = Containers.OfType<KeySubscriptionContainer<T>>().FirstOrDefault();
            if (container != null)
                return container.Count(key);

            return 0;
        }

        public void Dispose()
        {
            foreach (var x in Containers.OfType<IDisposable>().ToArray())
                x.Dispose();

            Containers.Clear();
        }
    }
}
