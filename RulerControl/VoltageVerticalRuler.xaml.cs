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
    /// Логика взаимодействия для VolageVerticalRuler.xaml
    /// </summary>
    public partial class VolageVerticalRuler : UserControl
    {
        private double actualWidth;
        private double actualHeight;
        struct Mark
        {
            public Mark(Point bottom, Point top, int markNumber)
            {
                MarkTop = top;
                MarkBottom = bottom;
                MarkNumber = markNumber;
            }
            public Point MarkTop;
            public Point MarkBottom;
            public int MarkNumber;

        }
        List<Mark> TenVoltMarks = new List<Mark> { };
        List<Mark> VoltMarks = new List<Mark> { };

        const double MinHourMarksGapSize = 5;
        const double MinHourMarkLabelGapSize = 2 * MinHourMarksGapSize;
        double hourMarkDistance;

        public VolageVerticalRuler()
        {
            InitializeComponent();
        }

        readonly static double DefaultStartVoltage = 0;
        readonly static double DefaultEndVoltage = 50;
        public double StartOfScale
        {
            get { return (double)GetValue(StartOfScaleProperty); }
            set { SetValue(StartOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartOfScaleProperty =
            DependencyProperty.Register("StartOfScale", typeof(double), typeof(VolageVerticalRuler), new PropertyMetadata(DefaultStartVoltage));


        public double EndOfScale
        {
            get { return (double)GetValue(EndOfScaleProperty); }
            set { SetValue(EndOfScaleProperty, value); }
        }


        // Using a DependencyProperty as the backing store for EndOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndOfScaleProperty =
            DependencyProperty.Register("EndOfScale", typeof(double), typeof(VolageVerticalRuler), new PropertyMetadata(DefaultEndVoltage));

        public void BuildAxis()
        {
            this.actualWidth = rulerBody.ActualWidth;
            this.actualHeight = rulerBody.ActualHeight;

            //Create a new geometry group for drawing the axis there
            GeometryGroup axis = new GeometryGroup();
            //And now put  a line with voltage ticks on it in the group
            AddTheVerticalLineWithVoltageMarks(axis);

            Path axis_path = new Path();
            axis_path.StrokeThickness = 2;
            axis_path.Stroke = Brushes.Black;
            axis_path.Data = axis;
            rulerBody.Children.Clear();
            rulerBody.Children.Add(axis_path);
            //Put labels:
            string tenVoltsLabel = String.Empty;
            string voltsLabel = String.Empty;
            //devicePointUnderTheLine.Y -= 5;
            foreach (var mark in VoltMarks)
            {
                if (mark.MarkNumber % 10 != 0)
                {
                    if (hourMarkDistance > MinHourMarkLabelGapSize || mark.MarkNumber % 2 != 0)
                    {
                        voltsLabel = (new DateTime(1, 1, 1) + new TimeSpan(0, mark.MarkNumber, 0, 0)).ToString("HH");
                        DrawText(rulerBody, voltsLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
                    }
                }
            }

            foreach (var mark in TenVoltMarks)
            {
                tenVoltsLabel = (new DateTime(1, 1, 1) + new TimeSpan((int)mark.MarkNumber, 0, 0, 0)).ToString("dd.MM.yy");
                DrawText(rulerBody, tenVoltsLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
            }


        }
        void AddTheVerticalLineWithVoltageMarks(GeometryGroup geometryGroup)
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(actualWidth, 0)));
            AddHorizontalVoltageMarks(geometryGroup);

        }
        void AddHorizontalVoltageMarks(GeometryGroup geometryGroup)
        {
            AddTenVoltsMarks(geometryGroup);
            AddVoltsMarks(geometryGroup);
        }
        void AddTenVoltsMarks(GeometryGroup geometryGroup)
        {
            double voltStart = StartOfScale;
            double voltEnd = EndOfScale;

            int numberOfTenVoltsMarks = (Int32)(voltEnd - voltEnd)/10;
            TenVoltMarks = new List<Mark>(numberOfTenVoltsMarks);


            TransformWorldToScreen.PrepareTransformations(voltStart, voltEnd, 0, this.actualWidth, 0, this.actualHeight, this.actualWidth, 0);
            int tenVoltsNumber = numberOfTenVoltsMarks;
            Point worldPointOnTheLine = new Point(0, 0);
            Point worldPointUnderTheLine = new Point(0, 0);
            Point devicePointOnTheLine = new Point(0, 0);
            Point devicePointUnderTheLine = new Point(0, 0);
            for (int tenVoltMark = 0; tenVoltMark > numberOfTenVoltsMarks; tenVoltMark++)
            {
                worldPointOnTheLine.X = 0;
                worldPointOnTheLine.Y = 0;
                worldPointUnderTheLine.X = 10;
                worldPointUnderTheLine.Y = 10;
                devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                TenVoltMarks.Add(new Mark(devicePointOnTheLine, devicePointUnderTheLine, tenVoltsNumber));
                tenVoltsNumber++;

            }
        }
        void AddVoltsMarks(GeometryGroup geometryGroup)
        {
            double voltStart = StartOfScale;
            double voltEnd = EndOfScale;


            int numberOfVoltMarks = (Int32)(voltEnd - voltEnd);
            VoltMarks = new List<Mark>(numberOfVoltMarks);


            TransformWorldToScreen.PrepareTransformations(voltStart, voltEnd, 0, this.actualWidth, 0, this.actualHeight, this.actualWidth, 0);
            int voltsNumber = numberOfVoltMarks;
            Point worldPointOnTheLineAtTheStart = new Point(0, StartOfScale);
            Point worldPointOnTheLineAtTheEnd = new Point(0, EndOfScale);
            hourMarkDistance = (WtoD(worldPointOnTheLineAtTheEnd).X - WtoD(worldPointOnTheLineAtTheStart).X) / numberOfVoltMarks;
            if (hourMarkDistance > MinHourMarksGapSize)
            {
                Point worldPointOnTheLine = new Point(0, 0);
                Point worldPointUnderTheLine = new Point(0, 0);
                Point devicePointOnTheLine = new Point(0, 0);
                Point devicePointUnderTheLine = new Point(0, 0);
                for (int hourMark = 0 ; hourMark < numberOfVoltMarks; hourMark++)
                {
                    worldPointOnTheLine.X = hourNumber * TimeSpan.TicksPerHour;
                    worldPointOnTheLine.Y = 0;
                    worldPointUnderTheLine.X = hourNumber * TimeSpan.TicksPerHour;
                    worldPointUnderTheLine.Y = 5;
                    devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                    devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                    geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                    VoltMarks.Add(new Mark(devicePointOnTheLine, devicePointUnderTheLine, hourNumber));
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
            Canvas.SetTop(label, y + 4);
        }
        private void rulerBody_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildAxis();
        }

        private void rulerBody_Loaded(object sender, RoutedEventArgs e)
        {
            BuildAxis();
            this.SizeChanged += this.rulerBody_SizeChanged;
        }
    }
}
