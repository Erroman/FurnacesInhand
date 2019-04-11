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
            double LowerLimitForTimeOnXAxis = 0;
            double UpperLimitForTimeOnXAxis = 60000; //верхняя гравница
            double LowerLimitForVoltageOnYAxis = 0;
            double UpperLimitForVoltageOnYAxis = 50;
            double xmin = rect.X;
            double xmax = rect.X+rect.Width;
            double ymin = rect.Y;
            double ymax = rect.Y+rect.Height;
            PrepareTransformations(
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitForVoltageOnYAxis, UpperLimitForVoltageOnYAxis,
                xmin, xmax,
                ymin, ymax);

            Point begPoint = WtoD(new Point(20000, 10)); // миллисекунды, вольты: тестовая точка
            Point endPoint = WtoD(new Point(40000, 30)); // миллисекунды, вольты: тестовая точка

            drawingContext.DrawLine(pen, begPoint, endPoint);
            drawingContext.Close();
            return drawingVisual;
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
