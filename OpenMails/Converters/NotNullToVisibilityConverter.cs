using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenMails.Converters
{
    /// <summary>
    /// 任意值转换为 Visibility <br/>
    /// 当值不为空时返回 Visibility.Visible, 否则时 Collapsed
    /// </summary>
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
