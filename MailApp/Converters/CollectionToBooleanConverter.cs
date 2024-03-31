using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MailApp.Converters
{

    /// <summary>
    /// 集合转换为 Visibility <br/>
    /// 当集合不为空时, 返回 Visibility.Visible, 否则是 Visibility.Collapsed
    /// </summary>
    public class CollectionToBooleanConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not ICollection collection ||
                collection.Count == 0)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
