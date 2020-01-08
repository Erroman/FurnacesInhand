using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            //And now put  a line with  ticks on it to the group
            PrepareScaling(StartOfScale, EndOfScale, 0, this.ActualHeight);                                //преобразование масштаба без учёта сдвига шкалы
            PrepareTransformations(Xmin, Xmax, StartOfScale, EndOfScale, Xmin, Xmax, 0, this.actualHeight);//с учётом сдвига начала шкалы
            rulerBody.Children.Clear();
            AddHorizontalUnitsMarksWithLabels(axis);
            Path axis_path = new Path();
            axis_path.StrokeThickness = 2;
            axis_path.Stroke = Brushes.Black;
            axis_path.Data = axis;
            
            rulerBody.Children.Add(axis_path);
        }        
        void AddTheVerticalLineWithUnitsMarks(GeometryGroup geometryGroup)
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(actualWidth, 0), new Point(actualWidth, actualHeight)));
            if (this.actualHeight != 0 & this.actualWidth != 0) 
            { 
                AddHorizontalUnitsMarksWithLabels(geometryGroup);
            }
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
                rulerBody.Children.Add(tb);
                Canvas.SetRight(tb,10);
                Canvas.SetTop(tb,pt1.Y - size.Height/2);
             }
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
