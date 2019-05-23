using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using static FurnacesInHand.EnumerableExtensions;

namespace FurnacesInHand
{
    class VoltageTimeConverter : IValueConverter
    {
        private App _application;
        private MainWindow _window;
        public VoltageTimeConverter()
        {
            _application = (App)Application.Current;
            _window = (MainWindow)_application.MainWindow;
        }

        private DateTime _lastMeasuredValue;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ///Check the state of the left mouse button! And if not pressed, pass back the same value of the voltage,
            ///else transform the X-coordinate coming in argument 'value'  to the voltage value corresponding to it
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                //При помощи массива значений параметров находим ближайший по времени
                if (_window.Voltage_graph_pairs != null)
                {
                    TimeParameterPair tpp = _window.Voltage_graph_pairs.Where(x => Math.Abs(x.screenPoint.X - (double)value) < 1).Select(x => x).FirstOrDefault();
                    _lastMeasuredValue = tpp.dt;
                }

            }
            return _lastMeasuredValue; //presumably get it from the parameter argument
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
//делаем так: при нажатой мышке ищем Х-координату её курсора в перечислении Voltage_graph_pairs<ParameterPair>, 
//где в структуру ParameterPair добавлено поле Point screenPoint с занесёнными туда уже при рисовании графика значениями.