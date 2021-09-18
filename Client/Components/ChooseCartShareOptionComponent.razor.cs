using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.ViewModels;

namespace GroceryListHelper.Client.Components
{
    public class ChooseCartShareOptionComponentBase : BasePage<IndexViewModel>
    {
        public string LeftButtonActive => ViewModel.ShareMode == ShareModeType.Self ? "activeButton" : "";
        public string RightButtonActive => ViewModel.ShareMode == ShareModeType.Join ? "activeButton" : "";

        public void SelectShareMode(ShareModeType shareMode)
        {
            ViewModel.ShareMode = shareMode;
        }

        public void SelectSelfShare()
        {
            ViewModel.ShareMode = ShareModeType.Self;
        }

        public void SelectJoinShare()
        {
            ViewModel.ShareMode = ShareModeType.Join;
        }
    }
}