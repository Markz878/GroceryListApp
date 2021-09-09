using System;
using System.Collections.Generic;

namespace GroceryListHelper.Client.HelperMethods
{
    public abstract class ObservableObject
    {
        public event Action StateChanged;

        public void OnPropertyChanged()
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
