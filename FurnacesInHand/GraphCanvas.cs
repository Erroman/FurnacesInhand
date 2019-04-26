using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FurnacesInHand
{
    class GraphCanvas:Canvas
    {
        private Path verticalCursor = null;
        public Path VerticalCursor(Point point)
        {
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 0.1;

            LineGeometry myLineGeometry = new LineGeometry();
            myLineGeometry.StartPoint = new Point(point.X, 0);
            myLineGeometry.EndPoint = new Point(point.X, this.ActualHeight);

            myPath.Data = myLineGeometry;
            if (verticalCursor != null)
                this.Children.Remove(verticalCursor);
            verticalCursor = myPath;
            this.Children.Add(myPath);
            return myPath;


        }
    }
}
