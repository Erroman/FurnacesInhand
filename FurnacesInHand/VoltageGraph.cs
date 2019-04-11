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
        private DrawingVisual CreateDrawingVisualPlot(IEnumerable<TimeParameterPair> timeParameterPair, Rect rect)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Pen pen = new Pen(Brushes.Black, 1.0);
            Point begPoint = new Point(rect.X, rect.Y);
            Point endPoint = new Point(rect.Width, rect.Height);
            double LowerLimitForTimeOnXAxis = 0;
            double UpperLimitForTimeOnXAxis = 60000; //верхняя гравница
            double LowerLimitForCurrentOnYAxis = 0;
            double UpperLimitForCurrentOnYAxis = 0;
            double xmin = rect.X;
            double xmax = rect.X+rect.Width;
            double ymin = rect.Y;
            double ymax = rect.Y+rect.Height;
            PrepareTransformations(
                LowerLimitForTimeOnXAxis, UpperLimitForTimeOnXAxis,
                LowerLimitForCurrentOnYAxis, UpperLimitForCurrentOnYAxis,
                xmin, xmax,
                ymin, ymax);
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
