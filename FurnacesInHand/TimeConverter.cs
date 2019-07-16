using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FurnacesInHand
{
    class TimeConverter : IValueConverter
    {
        DateTime? dt = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                dt = DateTime.Now;
            return dt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
