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
using static RulerControls.TransformWorldToScreen;


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
            public Mark(Point bottom,Point top,int markNumber)
            {
                MarkTop = top;
                MarkBottom = bottom;
                MarkNumber = markNumber;
            }
            public Point MarkTop;
            public Point MarkBottom;
            public int MarkNumber;
 
        }
        List<Mark> DayMarks = new List<Mark> { };
        List<Mark> HourMarks = new List<Mark> { };

        const double MinHourMarksGapSize = 5;
        const double MinHourMarkLabelGapSize = 2 * MinHourMarksGapSize;
        double hourMarkDistance;

        public HorizontalRuler()
        {
            InitializeComponent();
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
            string dayLabel  = String.Empty;
            string hourLabel = String.Empty;
            //devicePointUnderTheLine.Y -= 5;
            foreach (var mark in HourMarks)
            {
                if(mark.MarkNumber % 24 != 0) 
                {
                    if(hourMarkDistance> MinHourMarkLabelGapSize || mark.MarkNumber % 2 != 0) 
                    {
                        hourLabel = (new DateTime(1, 1, 1) + new TimeSpan(0, mark.MarkNumber, 0, 0)).ToString("HH");
                        DrawText(rulerBody, hourLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
                    }
                }
            }

            foreach (var mark in DayMarks)
            {
                dayLabel = (new DateTime(1, 1, 1) + new TimeSpan((int)mark.MarkNumber, 0, 0, 0)).ToString("dd.MM.yy");
                DrawText(rulerBody, dayLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center); 
            }


        }
        void AddTheHorizontalLineWithTimeMarks(GeometryGroup geometryGroup) 
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(actualWidth, 0)));
            if (this.actualHeight != 0 & this.actualWidth != 0)
                AddVerticalTimeMarks(geometryGroup);

        }
        void AddVerticalTimeMarks(GeometryGroup geometryGroup) 
        {
            AddVerticalDayMarks(geometryGroup);
            AddVerticalHourMarks(geometryGroup);
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
                DayMarks.Add(new Mark(devicePointOnTheLine,devicePointUnderTheLine,dayNumber));
                dayNumber--;

            }
        }
        void AddVerticalHourMarks(GeometryGroup geometryGroup)
        {
            DateTime dtStart = StartOfScale;
            DateTime dtEnd = EndOfScale;
            Int64 dtStartTicks = dtStart.Ticks;
            Int64 dtEndTicks = dtEnd.Ticks;
            Int32 dtStartNumberOfHours = (Int32)(dtStartTicks / TimeSpan.TicksPerHour);
            Int32 dtEndNumberOfHours = (Int32)(dtEndTicks / TimeSpan.TicksPerHour);

            int numberOfHourMarks = (Int32)(dtEndNumberOfHours - dtStartNumberOfHours);
            HourMarks = new List<Mark>(numberOfHourMarks);


            TransformWorldToScreen.PrepareTransformations(dtStartTicks, dtEndTicks, 0, this.actualHeight, 0, this.actualWidth, this.actualHeight, 0);
            int hourNumber = dtEndNumberOfHours;
            Point worldPointOnTheLineAtTheStart = new Point(dtStartTicks, 0);
            Point worldPointOnTheLineAtTheEnd = new Point(dtEndTicks, 0);
            hourMarkDistance = (WtoD(worldPointOnTheLineAtTheEnd).X - WtoD(worldPointOnTheLineAtTheStart).X)/ numberOfHourMarks;
            if (hourMarkDistance > MinHourMarksGapSize) { 
            Point worldPointOnTheLine = new Point(0, 0);
            Point worldPointUnderTheLine = new Point(0, 0);
            Point devicePointOnTheLine = new Point(0, 0);
            Point devicePointUnderTheLine = new Point(0, 0);
            for (int hourMark = numberOfHourMarks; hourMark > 0; hourMark--)
            {
                worldPointOnTheLine.X = hourNumber * TimeSpan.TicksPerHour;
                worldPointOnTheLine.Y = 0;
                worldPointUnderTheLine.X = hourNumber * TimeSpan.TicksPerHour;
                worldPointUnderTheLine.Y = 5;
                devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                HourMarks.Add(new Mark(devicePointOnTheLine, devicePointUnderTheLine, hourNumber));
                hourNumber--;

            }
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
            Canvas.SetTop(label, y+4);
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
