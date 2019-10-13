using System;
using System.Collections.Generic;
using System.Linq;

namespace Company.Client.Common.Support
{
    class KeySubscriptionContainer<T> : IDisposable where T : class
    {
        private class Subscription : IDisposable
        {
            public event Action<Subscription, string> Disposing;
            public string SubscriptionKey { get; }
            public Subscription(string key)
            {
                SubscriptionKey = key;
            }
            public void Dispose()
            {
                Disposing?.Invoke(this, SubscriptionKey);
            }
        }

        Dictionary<Subscription, T> Subscriptions;
        public event Action<KeySubscriptionContainer<T>, T, string> SubscriptionRemoved;

        public KeySubscriptionContainer()
        {
            Subscriptions = new Dictionary<Subscription, T>();
        }

        public IDisposable AddItem(string key, T instance)
            => CreateInternal(key, instance);

        private IDisposable CreateInternal(string key, T instance)
        {
            var value = new Subscription(key);
            value.Disposing += SubscriptionDisposing;
            Subscriptions.Add(value, instance);
            return value;
        }

        private void SubscriptionDisposing(Subscription subscription, string key)
        {
            subscription.Disposing -= SubscriptionDisposing;
            if (Subscriptions.ContainsKey(subscription))
            {
                var observer = Subscriptions[subscription];
                var count = Subscriptions.Count(x => x.Key.SubscriptionKey == key);

                Subscriptions.Remove(subscription);
                SubscriptionRemoved?.Invoke(this, observer, key);
            }
        }

        public T[] GetItems(string key)
        {
            return Subscriptions
                .Where(x => x.Key.SubscriptionKey == key)
                .Select(x => x.Value)
                .OfType<T>()
                .ToArray();
        }

        public int Count()
        {
            return Subscriptions.Count;
        }
        public int Count(string key)
        {
            return Subscriptions.Where(x => x.Key.SubscriptionKey == key).Count();
        }

        public void Dispose()
        {
            Subscriptions.Clear();
        }
    }
}
