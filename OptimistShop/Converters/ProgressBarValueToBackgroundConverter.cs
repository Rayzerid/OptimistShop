using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OptimistShop.Converters
{
    public class ProgressBarValueToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)parameter)
            {
                case "Confirm":
                    if ((double)value == 13 || (double)value == 50 || (double)value == 87 || (double)value == 100)
                        return (Brush)new BrushConverter().ConvertFrom("#188851");
                    break;
                case "Cooking":
                    if ((double)value == 50 || (double)value == 87 || (double)value == 100)
                        return (Brush)new BrushConverter().ConvertFrom("#188851");
                    break;
                case "Ready":
                    if ((double)value == 87 || (double)value == 100)
                        return (Brush)new BrushConverter().ConvertFrom("#188851");
                    break;
                case "Got":
                    if ((double)value == 100)
                        return (Brush)new BrushConverter().ConvertFrom("#188851");
                    break;
            };
            return (Brush)new BrushConverter().ConvertFrom("#ededed");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Brush)new BrushConverter().ConvertFrom("#ededed");
        }
    }
}
