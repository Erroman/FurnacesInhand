using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
namespace FurnacesInHand
{
    class CursorConverter : IValueConverter
    {
        FurnacesInHandViewModel finhViewModel;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            finhViewModel = (FurnacesInHandViewModel)parameter;
            if (finhViewModel.DrawCursorWhenMousButtonUp)
                return (double)value;
            else
                return finhViewModel.CursorXCoordinate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
