using MailApp.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MailApp.TemplateSelectors
{
    public class NavigationViewContentTempletTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MailsViewTemplate { get; set; }
        public DataTemplate SettingsViewTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is TreeNode<MailFolder>)
                return MailsViewTemplate;

            if (item is NavigationViewItem { Content: "Settings" })
                return SettingsViewTemplate;

            return null;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

    }

}
