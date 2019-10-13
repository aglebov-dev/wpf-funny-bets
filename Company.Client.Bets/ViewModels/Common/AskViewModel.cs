using System;
using Prism.Commands;

namespace Company.Client.Bets.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    sealed class AskViewModel
    {
        public string Title { get;  }
        public string Message { get; }

        public DelegateCommand ApplyCommand { get;  }
        public DelegateCommand CancelCommand { get; }

        private readonly Action<bool> CallBack;

        public AskViewModel(string title, string message, Action<bool> callBack)
        {
            Title = title;
            Message = message;
            CallBack = callBack;
            ApplyCommand = new DelegateCommand(() => callBack?.Invoke(true));
            CancelCommand = new DelegateCommand(() => callBack?.Invoke(false));
        }
    }
}
