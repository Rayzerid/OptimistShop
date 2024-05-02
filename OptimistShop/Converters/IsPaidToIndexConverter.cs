using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimistShop.Converters
{
    public class IsPaidToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
                return 0;
            if ((bool)value == false)
                return 1;
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 1)
                return false;
            if ((int)value == 0)
                return true;
            else
                return false;
        }
    }
}
