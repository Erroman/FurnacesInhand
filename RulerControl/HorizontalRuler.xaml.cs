using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RulerControls
{
    /// <summary>
    /// Логика взаимодействия для HorizontalRuler.xaml
    /// </summary>
    public partial class HorizontalRuler : UserControl
    {
        private double actualWidth;
        private double actualHeight;
        struct Mark
        {   
            public Mark(Point bottom,Point top)
            {
                MarkTop = top;
                MarkBottom = bottom;
            }
            public Point MarkTop;
            public Point MarkBottom;
 
        }
        List<Mark> DayMarks = new List<Mark> { };

        public HorizontalRuler()
        {
            InitializeComponent();
            this.DataContext = new DateTimeRangesViewModel();
    
        }

        readonly static DateTime DefaultStartTime = new DateTime(2019, 10, 19);
        readonly static DateTime DefaultEndTime = new DateTime(2019, 10, 21);
        public DateTime StartOfScale
        {
            get { return (DateTime)GetValue(StartOfScaleProperty); }
            set { SetValue(StartOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartOfScaleProperty =
            DependencyProperty.Register("StartOfScale", typeof(DateTime), typeof(HorizontalRuler),new PropertyMetadata(DefaultStartTime));


        public DateTime EndOfScale
        {
            get { return (DateTime)GetValue(EndOfScaleProperty); }
            set { SetValue(EndOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndOfScaleProperty =
            DependencyProperty.Register("EndOfScale", typeof(DateTime), typeof(HorizontalRuler),new PropertyMetadata(DefaultEndTime));

        public void BuildTimeAxis() 
        {
            this.actualWidth = rulerBody.ActualWidth;
            this.actualHeight = rulerBody.ActualHeight;

            //Create a new geometry group for drawing the axis there
            GeometryGroup axisX = new GeometryGroup();
            //And now put  a line with time ticks on it in the group
            AddTheHorizontalLineWithTimeMarks(axisX);
            
            Path axisX_path = new Path();
            axisX_path.StrokeThickness = 2;
            axisX_path.Stroke = Brushes.Black;
            axisX_path.Data = axisX;
            rulerBody.Children.Clear();
            rulerBody.Children.Add(axisX_path);
            //Put labels:
            string dayLabel = "Some text";
            //devicePointUnderTheLine.Y -= 5;
            //DrawText(rulerBody, dayLabel, devicePointUnderTheLine, 12, HorizontalAlignment.Center, VerticalAlignment.Center);

        }
        void AddTheHorizontalLineWithTimeMarks(GeometryGroup geometryGroup) 
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(actualWidth, 0)));
            AddVerticalTimeMarks(geometryGroup);

        }
        void AddVerticalTimeMarks(GeometryGroup geometryGroup) 
        {
            AddVerticalDayMarks(geometryGroup);
        }
        void AddVerticalDayMarks(GeometryGroup geometryGroup) 
        {
            DateTime dtStart = StartOfScale;
            DateTime dtEnd   = EndOfScale;
            Int64 dtStartTicks = dtStart.Ticks;
            Int64 dtEndTicks = dtEnd.Ticks;
            Int32 dtStartNumberOfDays = (Int32)(dtStartTicks / TimeSpan.TicksPerDay);
            Int32 dtEndNumberOfDays = (Int32)(dtEndTicks / TimeSpan.TicksPerDay);
 
            int numberOfDayMarks = (Int32)(dtEndNumberOfDays - dtStartNumberOfDays);
            DayMarks = new List<Mark>(numberOfDayMarks);


            TransformWorldToScreen.PrepareTransformations(dtStartTicks, dtEndTicks, 0, this.actualHeight, 0, this.actualWidth, this.actualHeight,0);
            int dayNumber = dtEndNumberOfDays;
            Point worldPointOnTheLine = new Point(0,0);
            Point worldPointUnderTheLine = new Point(0, 0);
            Point devicePointOnTheLine = new Point(0,0);
            Point devicePointUnderTheLine = new Point(0,0);
            for (int dayMark = numberOfDayMarks; dayMark > 0; dayMark--) 
            {
                worldPointOnTheLine.X = dayNumber * TimeSpan.TicksPerDay;
                worldPointOnTheLine.Y = 0;
                worldPointUnderTheLine.X = dayNumber * TimeSpan.TicksPerDay;
                worldPointUnderTheLine.Y = 10;
                devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                DayMarks.Add(new Mark(devicePointOnTheLine,devicePointUnderTheLine));
                dayNumber--;

            }

        }
        // http://csharphelper.com/blog/2014/09/draw-a-graph-with-labels-wpf-c/ 
        // Position a label at the indicated point.
        private void DrawText(Canvas can, string text, Point location,
            double font_size,
            HorizontalAlignment halign, VerticalAlignment valign)
        {
            // Make the label.
            Label label = new Label();
            label.Content = text;
            label.FontSize = font_size;
            can.Children.Add(label);

            // Position the label.
            label.Measure(new Size(double.MaxValue, double.MaxValue));

            double x = location.X;
            if (halign == HorizontalAlignment.Center)
                x -= label.DesiredSize.Width / 2;
            else if (halign == HorizontalAlignment.Right)
                x -= label.DesiredSize.Width;
            Canvas.SetLeft(label, x);

            double y = location.Y;
            if (valign == VerticalAlignment.Center)
                y -= label.DesiredSize.Height / 2;
            else if (valign == VerticalAlignment.Bottom)
                y -= label.DesiredSize.Height;
            Canvas.SetTop(label, y);
        }
        private void rulerBody_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildTimeAxis();
        }

        private void rulerBody_Loaded(object sender, RoutedEventArgs e)
        {
            BuildTimeAxis();
            this.SizeChanged += this.rulerBody_SizeChanged;
        }
    }
}
