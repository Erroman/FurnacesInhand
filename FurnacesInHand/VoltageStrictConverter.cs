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
    class VoltageStrictConverter:IValueConverter
    {
        private App _application;
        private MainWindow _window;
        public VoltageStrictConverter()
        {
            _application = (App)Application.Current;
            _window = (MainWindow)_application.MainWindow;
        }

        private object _lastMeasuredValue;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //value parameter holds a DateTime value obtained from TimeMover control
            DateTime dt = (DateTime)value;
            string timeOrvalue = (string)parameter;
            //Ближайшая по времени структура из считанного набора параметров
            TimeParameterPair tpp;
            if (_window.Voltage_graph_pairs != null)
            {
                //tpp = _window.Voltage_graph_pairs.Where(x => x.dt == _window.Voltage_graph_pairs.Max(x1 => x1.dt)).FirstOrDefault();
                tpp = _window.Voltage_graph_pairs.Where(x=>x.dt<=dt).OrderBy(x=>x.dt).LastOrDefault();
                int index = _window.Voltage_graph_pairs.FindIndex(a => a.dt == tpp.dt);
                if (index >= 0)
                {
                    _window.voltageValues.SelectedIndex = index;
                    _window.voltageValues.ScrollIntoView(_window.voltageValues.Items[index]);
                    if (timeOrvalue == "Value")
                        _lastMeasuredValue = tpp.parameter;
                    else
                    {
                        _lastMeasuredValue = tpp.dt;
                    }
                    //_window.PutTheCursor(tpp.screenPoint);
                    _window.VoltagePlot.VerticalCursor(tpp.screenPoint);
                }
                else
                    _lastMeasuredValue = String.Empty;
            }
            else
                _lastMeasuredValue = String.Empty;

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