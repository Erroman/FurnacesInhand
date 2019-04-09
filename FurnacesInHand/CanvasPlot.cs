using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FurnacesInHand
{
    class CanvasPlot:Canvas
    {
        private readonly VisualCollection _children;
        // Provide a required override for the VisualChildrenCount property.
        public CanvasPlot()
        {
            _children = new VisualCollection(this)
            {
                CreateDrawingVisualPlot()
            };

        }
        private DrawingVisual CreateDrawingVisualPlot()
        {
            return new DrawingVisual();
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
