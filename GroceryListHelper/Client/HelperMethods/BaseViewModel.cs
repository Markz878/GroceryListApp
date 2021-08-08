using System;
using System.Collections.Generic;

namespace GroceryListHelper.Client.HelperMethods
{
    public abstract class BaseViewModel
    {
        public event Action StateChanged;
        public Guid Guid { get; set; } = Guid.NewGuid();
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        private bool isBusy;

        public void OnPropertyChanged()
        {
            Console.WriteLine($"Called {nameof(BaseViewModel)} OnPropertyChanged with GUID {Guid}");
            StateChanged?.Invoke();
        }

        protected void SetProperty<T>(ref T backingFiled, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(backingFiled, value))
            {
                backingFiled = value;
                OnPropertyChanged();
            }
        }
    }
}
