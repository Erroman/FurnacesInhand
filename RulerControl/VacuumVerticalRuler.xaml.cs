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
    /// Логика взаимодействия для VacuumVerticalRuler.xaml
    /// </summary>
    public partial class VacuumVerticalRuler : UserControl
    {
        private double actualWidth;
        private double actualHeight;
        const double MinHudredUnitMarksGapSize = 5;
        const double MinTenUnitMarksGapSize = 5;
        const double MinUnitMarksGapSize = 5;
        const double MinUnitMarkLabelGapSize = 2 * MinUnitMarksGapSize; //минимально допустимое расстояние между обозначениями на вольтовых отметках по вертикали
        public VacuumVerticalRuler()
        {
            InitializeComponent();
        }

        readonly static double DefaultStartVacuum = 90;
        readonly static double DefaultEndVacuum = 100;
        public double StartOfScale
        {
            get { return (double)GetValue(StartOfScaleProperty); }
            set { SetValue(StartOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartOfScaleProperty =
            DependencyProperty.Register("StartOfScale", typeof(double), typeof(VacuumVerticalRuler), new PropertyMetadata(DefaultStartVacuum));


        public double EndOfScale
        {
            get { return (double)GetValue(EndOfScaleProperty); }
            set { SetValue(EndOfScaleProperty, value); }
        }


        // Using a DependencyProperty as the backing store for EndOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndOfScaleProperty =
            DependencyProperty.Register("EndOfScale", typeof(double), typeof(VacuumVerticalRuler), new PropertyMetadata(DefaultEndVacuum));

        public void BuildAxis()
        {
            this.actualWidth = rulerBody.ActualWidth;
            this.actualHeight = rulerBody.ActualHeight;
            const int Xmin = 0;
            const int Xmax = 1;
            //Create a new geometry group for drawing the axis there
            GeometryGroup axis = new GeometryGroup();
            //And now put  a line with vacuum ticks on it in the group
            PrepareScaling(StartOfScale, EndOfScale, 0, this.ActualHeight);                                //преобразование масштаба без учёта сдвига шкалы
            PrepareTransformations(Xmin, Xmax, StartOfScale, EndOfScale, Xmin, Xmax, 0, this.actualHeight);//с учётом сдвига начала шкалы
            AddTheVerticalLineWithUnitsMarks(axis);
            //AddHorizontalUnitsMarksWithLabels(axis);
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
        void AddHorizontalUnitsMarksWithLabels(GeometryGroup geometryGroup)
        {
            double optimalYSpacing = 20;   //оптимальное расстояние между соседними метками по Y в машинных единицах
            double ySpacing = DtoWScale(optimalYSpacing); //преобразуем это расстояние в единицы измерения параметра (мм рт.ст.)
            double yTick = OptimalSpacing(ySpacing);  //округляем до удобной величины
            int yStart = (int)Math.Ceiling(StartOfScale / yTick);
            int yEnd = (int)Math.Floor(EndOfScale / yTick);
            Point pt1;
            Point pt2;
            TextBlock tb;
            Size size;
            LineGeometry tick;
            double dy;
            for (int i = yStart; i <= yEnd; i++)
            {
                dy = i * yTick;
                pt1 = WtoD(new Point(actualWidth , dy));
                pt2 = new Point(pt1.X - 5, pt1.Y);
                tick = new LineGeometry(pt1,pt2);
                geometryGroup.Children.Add(tick);
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

             }
        }
        double[] dashes;
         void Dashes(double N1,double N2, double n,out double[] dashes) // заполняем массив значениями между N1 и N2, кратными n
        {
            dashes = new double[] { 1, 2, 3 }; 
        }
        void AddTheVerticalLineWithUnitsMarks(GeometryGroup geometryGroup)
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(actualWidth, 0), new Point(actualWidth, actualHeight)));
            if (this.actualHeight != 0 & this.actualWidth != 0) 
            { 
                AddHorizontalUnitsMarks(geometryGroup);
                AddHorizontalUnitsMarksWithLabels(geometryGroup);
            }
        }
        void AddHorizontalUnitsMarks(GeometryGroup geometryGroup)
        {
            AddHundredUnitMarks(geometryGroup);
            AddTenUnitMarks(geometryGroup);
            AddOneUnitMarks(geometryGroup);
        }
        void AddHundredUnitMarks(GeometryGroup geometryGroup)
        {
            int hundredUnitDistance = 100; //разность в единицах давления между соседними делениями (100 мм рт.ст.).
            int hundredUnitMarkLength = 15; //длина отметки для напряжения,кратного 100 мм.рт.ст.

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
