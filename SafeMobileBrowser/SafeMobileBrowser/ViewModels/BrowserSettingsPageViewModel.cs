using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SafeMobileBrowser.ViewModels
{
    public class BrowserSettingsPageViewModel : BaseViewModel
    {
        public ICommand GoBackCommand { get; set; }
    }
}
