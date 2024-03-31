using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailApp.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MailApp.TemplateSelectors
{
    public class NavigationItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MailFolderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is MailFolderWrapper)
                return MailFolderTemplate;

            return SeparatorTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }

}
