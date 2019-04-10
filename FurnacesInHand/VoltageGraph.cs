using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FurnacesInHand
{
    class VoltageGraph:FrameworkElement
    {
        private readonly VisualCollection _children;
        // Provide a required override for the VisualChildrenCount property.
        public VoltageGraph()
        {
            _children = new VisualCollection(this)
            {
                CreateDrawingVisualPlot()
            };
        }
        private DrawingVisual CreateDrawingVisualPlot()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            Pen pen = new Pen(Brushes.Black, 1.0);
            Point begPoint = new Point(0, 0);
            Point endPoint = new Point(100, 100);
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
