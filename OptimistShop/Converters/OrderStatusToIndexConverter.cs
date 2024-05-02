using System;
using System.Globalization;
using System.Windows.Data;

namespace OptimistShop.Converters
{
    public class OrderStatusToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Подтвержден")
                return 0;
            if ((string)value == "Готовится")
                return 1;
            if ((string)value == "Готов к выдаче")
                return 2;
            if ((string)value == "Получен")
                return 3;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return "Подтвержден";
            if ((int)value == 1)
                return "Готовится";
            if ((int)value == 2)
                return "Готов к выдаче";
            if ((int)value == 3)
                return "Получен";
            else
                return "Подтвержден";
        }
    }
}
