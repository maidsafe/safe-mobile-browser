using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SafeMobileBrowser.Models;
using SafeMobileBrowser.Services;
using SafeMobileBrowser.Views;
using Xamarin.Forms;

namespace SafeMobileBrowser.ViewModels
{
    public class MenuPopUpViewModel : BaseViewModel
    {
        ModelPage _modelPage;

        public INavigation Navigation { get; set; }

        public MenuPopUpViewModel(INavigation navigation)
        {
            Navigation = navigation;
            AppService = new AppService();
        }

        public async void MakeRegisteredConnection()
        {
            if (AppService.FetchSession() == null)
                await AuthService.ProcessNonMockAuthentication();
        }

        public async Task<bool> OpenBookmarkPage()
        {
            if (AppService.FetchSession() == null)
            {
                return false;
            }
            else
            {
                bookMarkManager = new BookmarkManager(AppService.FetchSession());
                bookMarkManager.SetMdInfo(await AppService.GetMdInfoAsync());
                await GetWebsiteBookmark();
                return true;
            }
        }

        private async Task GetWebsiteBookmark()
        {
            // MenuPopUp popUp;
            try
            {
                // if (popUp == null)
                // popUp = new MenuPopUp();
                // await Navigation.PushPopupAsync(popUp);
                if (_modelPage == null)
                    _modelPage = new ModelPage();
                await Navigation.PushModalAsync(_modelPage);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
