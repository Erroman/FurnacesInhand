using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using static FurnacesInHand.TransformWorldToScreen;
namespace FurnacesInHand
{
    class VoltageGraph:FrameworkElement
    {
        const int TicksInMillisecond = 10000; //
        private readonly DateTime startTime;
        private readonly DateTime finishTime;
        private readonly VisualCollection _children;
        // Provide a required override for the VisualChildrenCount property.
  
        public VoltageGraph(IEnumerable<TimeParameterPair> timeParameterPair,Rect rect, DateTime startTime, DateTime finishTime)
        {
            this.startTime = startTime;
            this.finishTime = finishTime;
            _children = new VisualCollection(this)
            {
                CreateDrawingVisualPlot(timeParameterPair,rect)
            };
        }
        private DrawingVisual CreateDrawingVisualPlot(IEnumerable<TimeParameterPair> timeParameterPairs, Rect rect)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Pen pen = new Pen(Brushes.Black, 1.0);
            double LowerLimitForTimeOnXAxis = 0; //нижняя гравница временного интервала в миллисекундах
            double UpperLimitForTimeOnXAxis = MillisecondsSinceTheBeginning(this.finishTime); //верхняя гравница временного интервала в миллисекундах
            double LowerLimitForVoltageOnYAxis = 0;
            double UpperLimitForVoltageOnYAxis = 50;
            double xmin = rect.X;
            double xmax = rect.X + rect.Width;
            double ymin = rect.Y;
            double ymax = rect.Y + rect.Height;
            PrepareTransformations(
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitForVoltageOnYAxis, UpperLimitForVoltageOnYAxis,
                xmin, xmax,
                ymin, ymax);

            Point begPoint = WtoD(new Point(20000, 10)); // миллисекунды, вольты: тестовая точка
            Point endPoint = WtoD(new Point(40000, 30)); // миллисекунды, вольты: тестовая точка
            bool first_point = true;
            foreach (var /*пара <время,значение параметра> */ time_parameter in timeParameterPairs)
            {
                DateTime? currrent_moment = time_parameter.dt; //время
                double?   parameter_value = time_parameter.parameter; //значение праметра
                
                
                if(currrent_moment!=null && parameter_value != null)
                {
                    begPoint = endPoint;
                    endPoint = WtoD(new Point(MillisecondsSinceTheBeginning((DateTime)currrent_moment), (double)time_parameter.parameter));
                    if (first_point)
                    {
                        first_point = false;
                        begPoint = endPoint;
                    }
                    drawingContext.DrawLine(pen, begPoint, endPoint);
                }

                //begPoint = endPoint ?? time_parameter;
            }
            
            drawingContext.Close();
            return drawingVisual;
        }
        private double MillisecondsSinceTheBeginning(DateTime dt)
        {

            return (dt - this.startTime).Ticks / TicksInMillisecond;
        }
        protected override int VisualChildrenCount => _children.Count;

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

    }
}
