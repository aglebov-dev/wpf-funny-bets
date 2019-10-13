using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Company.Client.Common.Support
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    class SearchWrapper
    {
        private Action _callback;

        public string SearchText { get; set; }
        public ObservableCollection<string> SearchParts { get; }

        public SearchWrapper(Action callback)
        {
            _callback = callback;
            SearchParts = new ObservableCollection<string>();
            ((INotifyPropertyChanged)this).PropertyChanged += SelfPropertyChanged;
        }

        private void SelfPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchText))
            {
                var parts = SearchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var remove = SearchParts.Where(x => !parts.Contains(x)).ToArray();
                var add = parts.Where(x => !SearchParts.Contains(x)).ToArray();

                foreach (var x in remove)
                    SearchParts.Remove(x.ToLower());
                foreach (var x in add)
                    SearchParts.Add(x.ToLower());

                _callback?.Invoke();
            }
        }
    }
}
