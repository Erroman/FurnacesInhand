using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FurnacesInHand
{
    public partial class MainWindow
    {
        void VerticalCursor(Canvas canvas,Point point)
        {
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 0.1;

            LineGeometry myLineGeometry = new LineGeometry();
            myLineGeometry.StartPoint = new Point(point.X,0);
            myLineGeometry.EndPoint = new Point(point.X, canvas.ActualHeight);

            myPath.Data = myLineGeometry;
            if (verticalCursor != null)
                canvas.Children.Remove(verticalCursor);
            verticalCursor = myPath;
            canvas.Children.Add(myPath);


        }

    }
}
