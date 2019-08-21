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
    class EdgeTimeConverter : IValueConverter
    {
        private App _application;
        private MainWindow _window;
        private FurnacesInHandViewModel _datacontext;
        public EdgeTimeConverter()
        {
            _application = (App)Application.Current;
            _window = (MainWindow)_application.MainWindow;
            _datacontext = (FurnacesInHandViewModel)(_window.DataContext);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string UpperOrLower = (string)parameter;
            DateTime timeValue;
            TimeSpan timeFullSpan = _datacontext.DtEdgeEndTime - _datacontext.DtEdgeBegTime;
            double timeRangeSliderFullSpan = _window.timeRangeSlider.Maximum - _window.timeRangeSlider.Minimum;
            double thumbPosition;
            long l;
            if (UpperOrLower == "LowerValue") 
            {
                thumbPosition = (double)value - _window.timeRangeSlider.Minimum;
                l = (long)(timeFullSpan.Ticks * thumbPosition / timeRangeSliderFullSpan);
                timeValue = _datacontext.DtEdgeBegTime + TimeSpan.FromTicks(l);

            }
     
            else
            {
                thumbPosition = (double)value - _window.timeRangeSlider.Maximum;
                l = (long)(timeFullSpan.Ticks * thumbPosition / timeRangeSliderFullSpan);
                timeValue = _datacontext.DtEdgeEndTime + TimeSpan.FromTicks(l);
            }
                
            return timeValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt;
            dt = DateTime.Parse((string)value);
            return dt;
        }
    }
}
//делаем так: при нажатой мышке ищем Х-координату её курсора в перечислении SolenoidI_graph_pairs<ParameterPair>, 
//где в структуру ParameterPair добавлено поле Point screenPoint с занесёнными туда уже при рисовании графика значениями.