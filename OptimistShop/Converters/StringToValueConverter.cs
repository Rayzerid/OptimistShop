using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimistShop.Converters
{
    public class StringToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Подтвержден")
                return 13;
            if ((string)value == "В пути")
                return 50;
            if ((string)value == "В пункте выдачи")
                return 87;
            if ((string)value == "Получен")
                return 100;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 13)
                return "Подтвержден";
            if ((int)value == 50)
                return "В пути";
            if ((int)value == 87)
                return "В пункте выдачи";
            if ((int)value == 100)
                return "Получен";
            else
                return "Подтвержден";
        }
    }
}
