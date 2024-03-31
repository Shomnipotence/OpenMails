using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MailApp.Converters
{
    /// <summary>
    /// 任意值转换为 bool <br/>
    /// 当值不为空时返回 true, 否则时 false
    /// </summary>
    public class ValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
