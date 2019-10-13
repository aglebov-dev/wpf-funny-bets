using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Company.Client.Bets.Models
{
    abstract class BaseValidatedModel : INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> Errors;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public BaseValidatedModel()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
        protected void ClearErrors()
        {
            var properties = Errors.Select(x => x.Key).ToList();
            Errors.Clear();
            foreach (var property in properties)
            {
                OnErrorChanged(property);
            }
        }
        protected void RemoveErrors(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }
        protected void AppendErrors(string propertyName, params string[] errors)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors[propertyName].AddRange(errors);
            }
            else
            {
                Errors.Add(propertyName, errors.ToList());
            }

            OnErrorChanged(propertyName);
        }
        protected void ReplaceErrors(string propertyName, params string[] errors)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors[propertyName].Clear();
                Errors[propertyName].AddRange(errors);
                OnErrorChanged(propertyName);
            }
            else
            {
                AppendErrors(propertyName, errors);
            }
        }

        public bool HasErrors => Errors.Count > 0;
        public bool HasAnyError(params string[] properties) => properties != null ? Errors.Any(x => properties.Contains(x.Key)) : false;
        public string[] GetNotValidProperties() => Errors.Select(x => x.Key).ToArray();
        public IEnumerable GetErrors(string propertyName) => !string.IsNullOrWhiteSpace(propertyName) && Errors.ContainsKey(propertyName) ? Errors[propertyName] : null;
        public IEnumerable<string> GetErrorText(string propertyName) => Errors.ContainsKey(propertyName) ? Errors[propertyName] : new List<string>();
    }
}
