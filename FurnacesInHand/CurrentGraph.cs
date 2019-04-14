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
    class CurrentGraph : FrameworkElement
    {
        const int TicksInMillisecond = 10000; //
        private readonly DateTime startTime;
        private readonly DateTime finishTime;
        private readonly VisualCollection _children;
        // Provide a required override for the VisualChildrenCount property.

        public CurrentGraph(IEnumerable<TimeParameterPair> timeParameterPair, Rect rect, DateTime startTime, DateTime finishTime)
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
            double LowerLimitForCurrentOnYAxis = 0;
            double UpperLimitForCurrentOnYAxis = 50;
            double xmin = rect.X;
            double xmax = rect.X + rect.Width;
            double ymin = rect.Y;
            double ymax = rect.Y + rect.Height;
            PrepareTransformations(
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitForCurrentOnYAxis, UpperLimitForCurrentOnYAxis,
                xmin, xmax,
                ymin, ymax);

            Point WPoint = new Point(0, 0); // миллисекунды, вольты
            Point begDPoint = new Point(0, 0);
            Point DPoint = new Point(0, 0);
            bool FirstDot = true;

            foreach (var /*пара <время,значение параметра> */ time_parameter in timeParameterPairs)
            {
                //DateTime? currrent_moment = time_parameter.dt; //время
                //double?   parameter_value = time_parameter.parameter; //значение праметра


                WPoint.X = MillisecondsSinceTheBeginning(time_parameter.dt);
                WPoint.Y = time_parameter.parameter;
                DPoint = WtoD(WPoint);
                if (FirstDot) { begDPoint = DPoint; FirstDot = false; }
                else
                if (Math.Round(DPoint.X) != Math.Round(begDPoint.X))
                {

                    drawingContext.DrawLine(pen, begDPoint, DPoint);
                    begDPoint = DPoint;
                }


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
