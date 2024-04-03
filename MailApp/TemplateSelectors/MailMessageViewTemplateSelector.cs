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
    public class MailMessageViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate EditingTemplate { get; set; }
        public DataTemplate EmptyTemplate { get; set; }

        private bool IsMailEditing(MailMessage mailMessage)
        {
            // TODO: 判定当前是否处于编辑状态
            return false;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is not MailMessage mailMessage)
            {
                return EmptyTemplate;
            }

            if (IsMailEditing(mailMessage))
            {
                return EditingTemplate;
            }
            else
            {
                return NormalTemplate;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
