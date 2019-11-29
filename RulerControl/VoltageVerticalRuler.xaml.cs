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
    /// Логика взаимодействия для VoltageVerticalRuler.xaml
    /// </summary>
    public partial class VoltageVerticalRuler : UserControl
    {
        private double actualWidth;
        private double actualHeight;
        const double MinHudredUnitMarksGapSize = 5;
        const double MinTenUnitMarksGapSize = 5;
        const double MinUnitMarksGapSize = 5;
        const double MinUnitMarkLabelGapSize = 2 * MinUnitMarksGapSize; //минимально допустимое расстояние между обозначениями на вольтовых отметках по вертикали
        public VoltageVerticalRuler()
        {
            InitializeComponent();
        }

        readonly static double DefaultStartVacuum = 0;
        readonly static double DefaultEndVacuum = 50;
        public double StartOfScale
        {
            get { return (double)GetValue(StartOfScaleProperty); }
            set { SetValue(StartOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartOfScaleProperty =
            DependencyProperty.Register("StartOfScale", typeof(double), typeof(VoltageVerticalRuler), new PropertyMetadata(DefaultStartVacuum));


        public double EndOfScale
        {
            get { return (double)GetValue(EndOfScaleProperty); }
            set { SetValue(EndOfScaleProperty, value); }
        }


        // Using a DependencyProperty as the backing store for EndOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndOfScaleProperty =
            DependencyProperty.Register("EndOfScale", typeof(double), typeof(VoltageVerticalRuler), new PropertyMetadata(DefaultEndVacuum));

        public void BuildAxis()
        {
            this.actualWidth = rulerBody.ActualWidth;
            this.actualHeight = rulerBody.ActualHeight;

            //Create a new geometry group for drawing the axis there
            GeometryGroup axis = new GeometryGroup();
            //And now put  a line with vacuum ticks on it in the group
            AddTheVerticalLineWithUnitsMarks(axis);

            Path axis_path = new Path();
            axis_path.StrokeThickness = 2;
            axis_path.Stroke = Brushes.Black;
            axis_path.Data = axis;
            rulerBody.Children.Clear();
            rulerBody.Children.Add(axis_path);
            //Put labels:
            string tenUnitsLabel = String.Empty;
            string unitsLabel = String.Empty;
            //devicePointUnderTheLine.Y -= 5;
            //foreach (var mark in UnitMarks)
            //{
            //    if (mark.MarkNumber % 10 != 0)
            //    {
            //        if (hourMarkDistance > MinHourMarkLabelGapSize || mark.MarkNumber % 2 != 0)
            //        {
            //            voltsLabel = (new DateTime(1, 1, 1) + new TimeSpan(0, mark.MarkNumber, 0, 0)).ToString("HH");
            //            DrawText(rulerBody, voltsLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
            //        }
            //    }
            //}

            //foreach (var mark in TenUnitMarks)6
            //{
            //    tenVoltsLabel = (new DateTime(1, 1, 1) + new TimeSpan((int)mark.MarkNumber, 0, 0, 0)).ToString("dd.MM.yy");
            //    DrawText(rulerBody, tenVoltsLabel, mark.MarkTop, 10, HorizontalAlignment.Center, VerticalAlignment.Center);
            //}


        }
        void AddTheVerticalLineWithUnitsMarks(GeometryGroup geometryGroup)
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(actualWidth, 0), new Point(actualWidth, actualHeight)));
            if (this.actualHeight != 0 & this.actualWidth != 0)
                AddHorizontalUnitsMarks(geometryGroup);

        }
        void AddHorizontalUnitsMarks(GeometryGroup geometryGroup)
        {
            TransformWorldToScreen.PrepareTransformations(0, this.actualWidth, StartOfScale, EndOfScale, 0, this.actualWidth, 0, this.actualHeight);
            AddHundredUnitMarks(geometryGroup);
            AddTenUnitMarks(geometryGroup);
            AddOneUnitMarks(geometryGroup);
        }
        void AddHundredUnitMarks(GeometryGroup geometryGroup)
        {
            int hundredUnitDistance = 100; //разность в единицах давления между соседними делениями (100 мм).
            int hundredUnitMarkLength = 15; //длина отметки для напряжения,кратного 100 мм

            int numberOfHundredUnitsMarks = (int)Math.Ceiling((EndOfScale - StartOfScale) / hundredUnitDistance); //количество делений на шкале для заданной разницы значений

            Point worldPointAtTheStartOfTheScale = new Point(this.actualWidth, StartOfScale);
            Point worldPointAtTheEndOfTheScale = new Point(this.actualWidth, EndOfScale);
            Point devicePointAtTheStartOfTheScale = WtoD(worldPointAtTheStartOfTheScale);
            Point devicePointAtTheEndtOfTheScale = WtoD(worldPointAtTheEndOfTheScale);
            double hundredMarksDistance = Math.Abs(devicePointAtTheEndtOfTheScale.Y - devicePointAtTheStartOfTheScale.Y) / numberOfHundredUnitsMarks;

            Point worldPointOnTheLine = new Point(this.actualWidth, StartOfScale);
            Point worldPointUnderTheLine = new Point(this.actualWidth - hundredUnitMarkLength, StartOfScale);
            Point devicePointOnTheLine = new Point(0, 0);
            Point devicePointUnderTheLine = new Point(0, 0);
            if (hundredMarksDistance > MinHudredUnitMarksGapSize)
            {
                for (int hundredUnitsMark = 0; hundredUnitsMark < numberOfHundredUnitsMarks; hundredUnitsMark++)
                {
                    devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                    devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                    geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                    worldPointOnTheLine.Y += hundredUnitDistance;
                    worldPointUnderTheLine.Y += hundredUnitDistance;
                }
            }
        }
        void AddTenUnitMarks(GeometryGroup geometryGroup)
        {
            int tenUnitDistance = 10; //расстояние в вольтах между соседними делениями (10В).
            int tenUnitMarkLength = 10; //длина отметки для напряжения,кратного 10В

            int numberOfTenUnitsMarks = (int)Math.Ceiling((EndOfScale - StartOfScale) / tenUnitDistance); //количество делений на шкале для заданного расстояния между ними

            Point worldPointAtTheStartOfTheScale = new Point(this.actualWidth, StartOfScale);
            Point worldPointAtTheEndOfTheScale = new Point(this.actualWidth, EndOfScale);
            Point devicePointAtTheStartOfTheScale = WtoD(worldPointAtTheStartOfTheScale);
            Point devicePointAtTheEndtOfTheScale = WtoD(worldPointAtTheEndOfTheScale);
            double tenMarksDistance = Math.Abs(devicePointAtTheEndtOfTheScale.Y - devicePointAtTheStartOfTheScale.Y) / numberOfTenUnitsMarks;

            Point worldPointOnTheLine = new Point(this.actualWidth, StartOfScale);
            Point worldPointUnderTheLine = new Point(this.actualWidth - tenUnitMarkLength, StartOfScale);
            Point devicePointOnTheLine = new Point(0, 0);
            Point devicePointUnderTheLine = new Point(0, 0);
            if (tenMarksDistance > MinTenUnitMarksGapSize)
            {
                for (int tenUnitsMark = 0; tenUnitsMark < numberOfTenUnitsMarks; tenUnitsMark++)
                {
                    devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                    devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                    geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                    worldPointOnTheLine.Y += tenUnitDistance;
                    worldPointUnderTheLine.Y += tenUnitDistance;

                }
            }
        }
        void AddOneUnitMarks(GeometryGroup geometryGroup)
        {
            int oneUnitDistance = 1; //количество единиц измерения между соседними делениями (1).
            int unitMarkLength = 7; //длина отметки количества единиц измерения,кратного 1

            int numberOfUnitMarks = (Int32)Math.Ceiling((EndOfScale - StartOfScale) / oneUnitDistance); //количество делений на шкале для заданного расстояния между ними

            Point worldPointAtTheStartOfTheScale = new Point(this.actualWidth, StartOfScale);
            Point worldPointAtTheEndOfTheScale = new Point(this.actualWidth, EndOfScale);
            Point devicePointAtTheStartOfTheScale = WtoD(worldPointAtTheStartOfTheScale);
            Point devicePointAtTheEndtOfTheScale = WtoD(worldPointAtTheEndOfTheScale);
            double marksDistance = Math.Abs(devicePointAtTheEndtOfTheScale.Y - devicePointAtTheStartOfTheScale.Y) / numberOfUnitMarks;

            Point worldPointOnTheLine = new Point(this.actualWidth, StartOfScale);
            Point worldPointUnderTheLine = new Point(this.actualWidth - unitMarkLength, StartOfScale);
            Point devicePointOnTheLine = new Point(0, 0);
            Point devicePointUnderTheLine = new Point(0, 0);

            if (marksDistance > MinUnitMarksGapSize)
            {
                for (int oneUnitMark = 0; oneUnitMark < numberOfUnitMarks; oneUnitMark++)
                {

                    devicePointOnTheLine = TransformWorldToScreen.WtoD(worldPointOnTheLine);
                    devicePointUnderTheLine = TransformWorldToScreen.WtoD(worldPointUnderTheLine);
                    geometryGroup.Children.Add(new LineGeometry(devicePointOnTheLine, devicePointUnderTheLine));
                    worldPointOnTheLine.Y += oneUnitDistance;
                    worldPointUnderTheLine.Y += oneUnitDistance;
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
