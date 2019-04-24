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
    class SolenoidUGraph : FrameworkElement
    {
        const int TicksInMillisecond = 10000; //
        private readonly DateTime startTime;
        private readonly DateTime finishTime;
        private readonly VisualCollection _children;
        // Provide a required override for the VisualChildrenCount property.

        public SolenoidUGraph(IEnumerable<TimeParameterPair> timeParameterPair, Rect rect, DateTime startTime, DateTime finishTime)
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
            double LowerLimitForsolenoidUOnYAxis = 0;
            double UpperLimitForsolenoidUOnYAxis = 20;
            double xmin = rect.X;
            double xmax = rect.X + rect.Width;
            double ymin = rect.Y;
            double ymax = rect.Y + rect.Height;
            PrepareTransformations(
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitForsolenoidUOnYAxis, UpperLimitForsolenoidUOnYAxis,
                xmin, xmax,
                ymin, ymax);

            Point WPoint = new Point(0, 0); // миллисекунды, мм рт.ст.
            Point DPoint;                  // экранные координаты
            Point previousDPoint = new Point(0, 0); //предыдущая точка на графике, с которой соединяемся отрезком
            bool FirstDot = true;
            Point minDPoint = new Point(0, 0);
            Point maxDPoint = new Point(0, 0);
            bool Clashed = false;

            foreach (var /*пара <время,значение параметра> */ time_parameter in timeParameterPairs)
            {
                WPoint.X = MillisecondsSinceTheBeginning(time_parameter.dt);
                WPoint.Y = time_parameter.parameter;
                DPoint = WtoD(WPoint);
                if (FirstDot) { previousDPoint = DPoint; FirstDot = false; }
                else
                if (Math.Round(DPoint.X) != Math.Round(previousDPoint.X)) //алгоритм сглаживания(разрежения)
                {
                    //if (!Clashed)
                    //{
                    //    drawingContext.DrawLine(pen, previousDPoint, DPoint);
                    //    previousDPoint = DPoint;
                    //}
                    //else
                    //{
                    //    Clashed = false;
                    //    //Соединяем минимальную и максимальную точки,
                    //    //из них последнюю по времени соединяем с текущей.
                    //    drawingContext.DrawLine(pen, minDPoint, maxDPoint);
                    //    previousDPoint = minDPoint.X <= maxDPoint.X ? maxDPoint : minDPoint;
                    drawingContext.DrawLine(pen, previousDPoint, DPoint);
                    previousDPoint = DPoint;
                    //}

                }
                //else
                //{
                //    if (!Clashed)
                //    {
                //        Clashed = true;
                //        //определяем максимальную и минимальную точки
                //        //на неразличимом временном отрезке
                //        minDPoint = LowerPoint(previousDPoint, DPoint);
                //        maxDPoint = UpperPoint(previousDPoint, DPoint);
                //    }
                //    else
                //    {
                //        minDPoint = LowerPoint(minDPoint, DPoint);
                //        maxDPoint = UpperPoint(maxDPoint, DPoint);
                //    }


                //}
                //if (Clashed)
                //    drawingContext.DrawLine(pen, minDPoint, maxDPoint);
            }
            drawingContext.Close();
            return drawingVisual;
        }
        private Point LowerPoint(Point p1, Point p2) => p1.Y < p2.Y ? p1 : p2;
        private Point UpperPoint(Point p1, Point p2) => p1.Y > p2.Y ? p1 : p2;
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
