using System;
using Windows.UI.Xaml.Data;

namespace MailApp.Converters
{

    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean)
                throw new ArgumentException(nameof(value));

            return !boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean)
                throw new ArgumentException(nameof(value));

            return !boolean;
        }
    }
}
