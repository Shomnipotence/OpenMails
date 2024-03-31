using System;
using MailApp.Enums;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MailApp.Converters
{
    public class MailFolderIconConverter : IValueConverter
    {
        public bool UseSymbol { get; set; } = false;

        private IconElement CreateIcon(Symbol symbol)
        { 
            return new SymbolIcon() { Symbol = symbol };
        }
        private IconElement CreateIcon(string glyph)
        {
            return new FontIcon()
            {
                FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"),
                Glyph = glyph,
            };
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not MailFolderIcon icon)
                throw new ArgumentException(nameof(value));

            if (!UseSymbol)
            {
                return icon switch
                {
                    MailFolderIcon.Default => CreateIcon("\xE8B7"),
                    MailFolderIcon.Inbox => CreateIcon("\xE896"),
                    MailFolderIcon.Archive => CreateIcon("\xE7B8"),
                    MailFolderIcon.Drafts => CreateIcon("\xEC87"),
                    MailFolderIcon.SentItems => CreateIcon("\xE898"),
                    MailFolderIcon.DeletedItems => CreateIcon("\xE74D"),
                    MailFolderIcon.JunkEmail => CreateIcon("\xE74D"),

                    _ => CreateIcon("\xE8B7"),
                };
            }
            else
            {
                return icon switch
                {
                    MailFolderIcon.Default => CreateIcon(Symbol.Folder),
                    MailFolderIcon.Inbox => CreateIcon(Symbol.Download),
                    MailFolderIcon.Archive => CreateIcon(Symbol.Save),
                    MailFolderIcon.Drafts => CreateIcon(Symbol.Page2),
                    MailFolderIcon.SentItems => CreateIcon(Symbol.Send),
                    MailFolderIcon.DeletedItems => CreateIcon(Symbol.Delete),
                    MailFolderIcon.JunkEmail => CreateIcon(Symbol.Delete),

                    _ => CreateIcon(Symbol.Folder),
                };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
