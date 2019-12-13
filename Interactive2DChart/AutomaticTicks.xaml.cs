using System;
using System.Windows;
using System.Windows.Media;
namespace Interactive2DChart
{
    public partial class AutomaticTicks : Window
    {
  
        public AutomaticTicks()
        {
            InitializeComponent();
        }
        private void AddChart()
        {
            ChartStyle cs = new ChartStyle();

            cs.ChartCanvas = chartCanvas;
            cs.TextCanvas = textCanvas;
            cs.Title = "Sine and Cosine Chart";
            cs.Xmin = 0;
            cs.Xmax = 7;
            cs.Ymin = 699.99;
            cs.Ymax = 700.01;
            cs.GridlinePattern = ChartStyle.GridlinePatternEnum.Dot;
            cs.GridlineColor = Brushes.Black;
            //Добавляются оси координат 
            cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);
            //Draw Sine-like curve:
            DataSeries ds = new DataSeries();
            ds.LineColor = Brushes.Blue;
            ds.LineThickness = 2;
            double dx = (cs.Xmax - cs.Xmin) / 100;
            for (double x = cs.Xmin; x <= cs.Xmax + dx; x += dx)
            {
                double y = Math.Exp(-0.3 * Math.Abs(x)) * Math.Sin(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            DataCollection dc = new DataCollection();
            dc.DataList.Add(ds);
            // Draw Cosine-like curve:
            ds = new DataSeries();
            ds.LineColor = Brushes.Red;
            ds.LinePattern = DataSeries.LinePatternEnum.DashDot;
            ds.LineThickness = 2;
            for (double x = cs.Xmin; x <= cs.Xmax + dx; x += dx)
            {
                double y = Math.Exp(-0.3 * Math.Abs(x)) * Math.Cos(x);
                ds.LineSeries.Points.Add(new Point(x, y));
            }
            dc.DataList.Add(ds);
            //В dc содержатся линии графиков, которые нужно добавить,
            //cs определяет область экрана(Canvas), куда добавляем линии графиков.
            dc.AddLines(cs);
        }
        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tbDate.Text = DateTime.Now.ToShortDateString();
            textCanvas.Width = chartGrid.ActualWidth;
            textCanvas.Height = chartGrid.ActualHeight;
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            AddChart();
        }
    }
}