using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;

namespace FurnacesInHand
{
    class VacuumConverter : IValueConverter
    {
        private double _lastMeasuredValue;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ///Check the state of the left mouse button! And if not pressed, pass back the same value of the voltage,
            ///else transform the X-coordinate coming in argument 'value'  to the voltage value corresponding to it
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                _lastMeasuredValue = (double)value;
            return _lastMeasuredValue; //presumably get it from the parameter argument
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
