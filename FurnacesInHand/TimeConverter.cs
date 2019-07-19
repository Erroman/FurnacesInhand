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
        Point p = new Point(0,0);
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
                PrepareTransform(finhViewModel);
                p.X = (double)value;
                dt = finhViewModel.DtBegTime + TimeSpan.FromMilliseconds(DtoW(p).X);
            }
              
            return dt;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            finhViewModel = (FurnacesInHandViewModel)parameter;
            PrepareTransform(finhViewModel);
            dt = (DateTime)value;
            TimeSpan ts = (TimeSpan)(dt - finhViewModel.DtBegTime);
            p.X = ts.TotalMilliseconds;
             p.X = WtoD(p).X;
            return p.X;
        }
        private void PrepareTransform(FurnacesInHandViewModel finhViewModel)
        {
            xmin = 0;
            xmax = finhViewModel.CanvasVoltageWidth;
            LowerLimitForTimeOnXAxis = MillisecondsSinceTheBeginning(finhViewModel.DtBegTime);
            UpperLimitForTimeOnXAxis = MillisecondsSinceTheBeginning(finhViewModel.DtEndTime);
            PrepareTransformations
                (
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitOnYAxis, UpperLimitOnYAxis,
                xmin, xmax,
                ymin, ymax
                );
        }
        private double MillisecondsSinceTheBeginning(DateTime dt)
        {

            return (dt - finhViewModel.DtBegTime).Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
