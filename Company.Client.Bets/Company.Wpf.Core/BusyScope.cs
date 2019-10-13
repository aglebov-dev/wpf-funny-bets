
namespace Company.Wpf.Core.Utils
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Предоставляет область для индикации работы
    /// </summary>
    public class BusyScope : INotifyPropertyChanged
    {
        /// <summary>
        /// Возращает новый экземпляр, с настройками по умолчанию
        /// </summary>
        public static BusyScope Default => new BusyScope();

        int _counter = 0;
        bool _isBusy;
        int _percentProgress;
        object lockObj = new object();

        /// <summary>
        /// Событие PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Флаг работы
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            private set { _isBusy = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy))); }
        }

        /// <summary>
        /// Прогресс выполнения работы
        /// </summary>
        public int PercentProgress
        {
            get { return _percentProgress; }
            private set { _percentProgress = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PercentProgress))); }
        }

        /// <summary>
        /// Создает область, внутри которой IsBusy = true
        /// </summary>
        public IDisposable CreateWork(Progress<int> progress = null)
        {
            lock (lockObj)
            {
                if (_counter == 0)
                    _percentProgress = 0;
                _counter++;
            }
            IsBusy = true;
            if (progress != null)
                progress.ProgressChanged += ProgressChanged;

            return new DisposableScope(WorkDone, progress);
        }

        private void ProgressChanged(object sender, int progress)
        {
            PercentProgress = progress;
        }

        private void WorkDone(Progress<int> progress)
        {
            lock (lockObj)
            {
                if (--_counter == 0)
                {
                    if (progress != null)
                        progress.ProgressChanged -= ProgressChanged;

                    IsBusy = false;
                    _percentProgress = 0;
                }
            }
        }

        private class DisposableScope : IDisposable
        {
            Action<Progress<int>> DisposableAction;
            Progress<int> param;

            public DisposableScope(Action<Progress<int>> disposableAction, Progress<int> progress)
            {
                DisposableAction = disposableAction;
                param = progress;
            }

            public void Dispose() => DisposableAction?.Invoke(param);
        }
    }
}
