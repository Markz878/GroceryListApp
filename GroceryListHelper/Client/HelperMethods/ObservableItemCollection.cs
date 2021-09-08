using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.HelperMethods
{
    public class ObservableItemCollection<T, U> : ObservableCollection<T> where U : BaseViewModel
    {
        public ObservableItemCollection(U viewModel)
        {

        }
    }
}
