namespace SafeMobileBrowser.Models
{
    public class PopUpMenuItem : ObservableObject
    {
        public string MenuItemTitle { get; set; }

        private string _menuItemIcon;

        public string MenuItemIcon
        {
            get => _menuItemIcon;
            set
            {
                SetProperty(ref _menuItemIcon, value);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }
    }
}
