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
    class EdgeTimeMultiConverter : IMultiValueConverter
    {
        private App _application;
        private MainWindow _window;
        private FurnacesInHandViewModel _datacontext;
        public EdgeTimeMultiConverter()
        {
            _application = (App)Application.Current;
            _window = (MainWindow)_application.MainWindow;
            _datacontext = (FurnacesInHandViewModel)(_window.DataContext);
        }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
   
        //}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
//делаем так: при нажатой мышке ищем Х-координату её курсора в перечислении SolenoidI_graph_pairs<ParameterPair>, 
//где в структуру ParameterPair добавлено поле Point screenPoint с занесёнными туда уже при рисовании графика значениями.