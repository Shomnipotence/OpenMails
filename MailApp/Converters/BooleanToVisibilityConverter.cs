using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MailApp.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean)
                throw new ArgumentException(nameof(value));

            return boolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibility)
                throw new ArgumentException(nameof(value));

            return Visibility.Visible == visibility ? true : false;
        }
    }
}
