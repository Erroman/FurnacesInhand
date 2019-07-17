using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static FurnacesInHand.TransformWorldToScreen;

namespace FurnacesInHand
{
    class TimeConverter : IValueConverter
    {
        DateTime? dt = null;
        FurnacesInHandViewModel finhViewModel;
        double LowerLimitForTimeOnXAxis; //нижняя гравница временного интервала в миллисекундах
        double UpperLimitForTimeOnXAxis; //верхняя гравница временного интервала в миллисекундах
        double LowerLimitOnYAxis=0;
        double UpperLimitOnYAxis=100;
        double xmin;  //крайняя левая экранная координата на оси Х
        double xmax;  //крайняя правая экранная координата на оси Х
        double ymin=0;
        double ymax=100;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                finhViewModel = (FurnacesInHandViewModel)parameter;
                //PrepareTransformations
                //    (
                //    LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                //    LowerLimitOnYAxis, UpperLimitOnYAxis,
                //    xmin, xmax,
                //    ymin, ymax
                //    );
                dt = DateTime.Now;
            }
              
            return dt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
