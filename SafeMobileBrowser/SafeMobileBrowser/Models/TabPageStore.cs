using System.Collections.ObjectModel;
using System.Linq;

namespace SafeMobileBrowser.Models
{
    public class TabPageStore
    {
        public ObservableCollection<TabPage> TabPages { get; set; }

        public TabPageStore()
        {
            TabPages = new ObservableCollection<TabPage>();
        }

        public void AddPage(TabPage page)
        {
            TabPages.Add(page);
        }
    }
}
