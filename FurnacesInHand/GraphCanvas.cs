using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FurnacesInHand
{
    class GraphCanvas:Canvas
    {
        public GraphCanvas()
     : base()
        {
            this.SizeChanged += new SizeChangedEventHandler(GraphCanvas_SizeChanged);
            //this.MouseLeftButtonDown += GraphCanvas_MouseLeftButtonDown;
        }

        private void GraphCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        void GraphCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PanelHeight = this.ActualHeight;
            PanelWidth = this.ActualWidth;
        }

        public double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelWidth. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelWidthProperty =
          DependencyProperty.Register("PanelWidth", typeof(double), typeof(GraphCanvas),
          new PropertyMetadata(0d));

        public double PanelHeight
        {
            get { return (double)GetValue(PanelHeightProperty); }
            set { SetValue(PanelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelHeight. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelHeightProperty =
          DependencyProperty.Register("PanelHeight", typeof(double), typeof(GraphCanvas),
          new PropertyMetadata(0d));

        private Path verticalCursor = null;
        public Path VerticalCursor(Point point)
        {
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 0.2;

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
