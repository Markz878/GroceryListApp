using System;
using System.Collections.Generic;

namespace GroceryListHelper.Client.HelperMethods
{
    public class BaseViewModel
    {
        public event Action StateChanged;

        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }
        private bool isBusy;

        public virtual void OnPropertyChanged()
        {
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
