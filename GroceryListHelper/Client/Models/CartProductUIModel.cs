using GroceryListHelper.Client.HelperMethods;

namespace GroceryListHelper.Client.Models
{
    public class CartProductUIModel : ObservableObject
    {
        public int Id { get; set; }
        public bool IsCollected { get => isCollected; set => SetProperty(ref isCollected, value); }
        private bool isCollected;
        public double Total => UnitPrice * Amount;
        public double Amount { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public CartProductUIModel(BaseViewModel viewModel) : base(viewModel)
        {
        }
    }
}
