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
        private Int32[] dayMarks;

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
        }
        void AddTheHorizontalLineWithTimeMarks(GeometryGroup geometryGroup) 
        {
            geometryGroup.Children.Add(new LineGeometry(new Point(0, 0), new Point(actualWidth, 0)));

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
            this.dayMarks = new Int32[numberOfDayMarks];

            int dayNumber = dtEndNumberOfDays;
            for (int dayMark = numberOfDayMarks; dayMark > 0; dayMark--) dayMarks[dayMark - 1] = dayNumber--;

        }

        private void rulerBody_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildTimeAxis();
        }

        private void rulerBody_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += this.rulerBody_SizeChanged;
        }
    }
}
