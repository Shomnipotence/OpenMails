using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenMails.Converters
{

    public class CollectionToVisibilityConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not ICollection collection ||
                collection.Count == 0)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException();
        }
    }
}
