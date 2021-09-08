using System.Collections.Generic;

namespace GroceryListHelper.Client.HelperMethods
{
    public abstract class ObservableObject
    {
        public BaseViewModel ViewModel { get; set; }

        public ObservableObject(BaseViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        protected void SetProperty<T>(ref T backingFiled, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(backingFiled, value))
            {
                backingFiled = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged()
        {
            ViewModel.OnPropertyChanged();
        }
    }
}
