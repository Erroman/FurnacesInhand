using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Interactive2DChart
{
    public class ChartStyle : ChartStyleBase
    {
        private string title;
        private string xLabel;
        private string yLabel;
        private Canvas textCanvas;
        private bool isXGrid = true;
        private bool isYGrid = true;
        private Brush gridlineColor = Brushes.LightGray;
        private double xTick = 1;
        private double yTick = 0.5;
        private GridlinePatternEnum gridlinePattern;
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 10;
        private Line gridline = new Line();
        public ChartStyle()
        {
            title = "Title";
            xLabel = "X Axis";
            yLabel = "Y Axis";
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string XLabel
        {
            get { return xLabel; }
            set { xLabel = value; }
        }
        public string YLabel
        {
            get { return yLabel; }
            set { yLabel = value; }
        }
        public GridlinePatternEnum GridlinePattern
        {
            get { return gridlinePattern; }
            set { gridlinePattern = value; }
        }
        public double XTick
        {
            get { return xTick; }
            set { xTick = value; }
        }
        public double YTick
        {
            get { return yTick; }
            set { yTick = value; }
        }
        public Brush GridlineColor
        {
            get { return gridlineColor; }
            set { gridlineColor = value; }
        }
        public Canvas TextCanvas
        {
            get { return textCanvas; }
            set { textCanvas = value; }
        }
        public bool IsXGrid
        {
            get { return isXGrid; }
            set { isXGrid = value; }
        }
        public bool IsYGrid
        {
            get { return isYGrid; }
            set { isYGrid = value; }
        }
        public void AddChartStyle(TextBlock tbTitle, TextBlock tbXLabel, TextBlock tbYLabel)
        {
            Point pt = new Point();
            Line tick = new Line();
            double offset = 0;
            double dx, dy;
            TextBlock tb = new TextBlock();
            double optimalXSpacing = 100;  //оптимальное расстояние между соседними метками по X в машинных единицах
            double optimalYSpacing = 80;   //оптимальное расстояние между соседними метками по Y в машинных единицах
            // determine right offset:
            tb.Text = Math.Round(Xmax, 0).ToString();
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size size = tb.DesiredSize;
            rightOffset = size.Width / 2 + 2;
            // Determine left offset:
            double xScale = 0.0, yScale = 0.0;
            double xSpacing = 0.0, ySpacing = 0.0;
            double xTick = 0.0, yTick = 0.0;
            int xStart = 0, xEnd = 1;
            int yStart = 0, yEnd = 1;
            double offset0 = 30;
            while (Math.Abs(offset - offset0) > 1)
            {
                if (Xmin != Xmax)
                    xScale = (TextCanvas.Width - offset0 - rightOffset - 5) /
                    (Xmax - Xmin);
                if (Ymin != Ymax)
                    yScale = TextCanvas.Height / (Ymax - Ymin); //коффициент Device/World по Y
                xSpacing = optimalXSpacing / xScale;
                xTick = OptimalSpacing(xSpacing);
                ySpacing = optimalYSpacing / yScale; //оптимальное расстояние между соседними значениями параметра по Y
                yTick = OptimalSpacing(ySpacing);//оптимизированное(округлённое) оптимальное расстояние между соседними значениями параметра по Y
                xStart = (int)Math.Ceiling(Xmin / xTick);
                xEnd = (int)Math.Floor(Xmax / xTick);
                yStart = (int)Math.Ceiling(Ymin / yTick);
                yEnd = (int)Math.Floor(Ymax / yTick);
                //Определяется величина сдвига внутреннего холста(textCanvas) по отношению к внешнему(chartCanvas)
                //по горизонтали(leftOffset) и вертикали(bottomOffset).Горизонтальный сдвиг высчитывается, исходя из размеров 
                //самого длинного нименования метки, вертикальный остаётся неизменным с самого начала и не расчитывается
                for (int i = yStart; i <= yEnd; i++)
                {
                    dy = i * yTick;
                    pt = NormalizePoint(new Point(Xmin, dy));
                    tb = new TextBlock();
                    tb.Text = dy.ToString();
                    tb.TextAlignment = TextAlignment.Right;
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    if (offset < size.Width)
                        offset = size.Width;
                }
                if (offset0 > offset)
                    offset0 -= 0.5;
                else if (offset0 < offset)
                    offset0 += 0.5;
            }
            leftOffset = offset + 5;
            Canvas.SetLeft(ChartCanvas, leftOffset);     //горизонтальный сдвиг
            Canvas.SetBottom(ChartCanvas, bottomOffset); //вертикальный сдвиг 

            ChartCanvas.Width = TextCanvas.Width - leftOffset - rightOffset;
            ChartCanvas.Height = TextCanvas.Height - bottomOffset - size.Height / 2;
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);
            if (Xmin != Xmax)
                xScale = ChartCanvas.Width / (Xmax - Xmin);
            if (Ymin != Ymax)
                yScale = ChartCanvas.Height / (Ymax - Ymin);
            xSpacing = optimalXSpacing / xScale;
            xTick = OptimalSpacing(xSpacing);
            ySpacing = optimalYSpacing / yScale;
            yTick = OptimalSpacing(ySpacing);
            xStart = (int)Math.Ceiling(Xmin / xTick);
            xEnd = (int)Math.Floor(Xmax / xTick);
            yStart = (int)Math.Ceiling(Ymin / yTick);
            yEnd = (int)Math.Floor(Ymax / yTick);
            // Create vertical gridlines and x tick marks:
            if (IsYGrid == true)
            {
                for (int i = xStart; i <= xEnd; i++)
                {
                    gridline = new Line();
                    AddLinePattern();   //выбираем рисунок разметочной линии, устанавливая параметры для gridline
                    dx = i * xTick;
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline); //ставим вертикальную линию разметки на внутреннем холсте chartCanvas
                    //Evidently adding vertical tick
                    //расставляем вертикальные деления шкалы на оси X
                    pt = NormalizePoint(new Point(dx, Ymin));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X;
                    tick.Y2 = pt.Y - 5; //величина штриха отметки деления = 5
                    ChartCanvas.Children.Add(tick); //ставится вертикальное деление на оси X, принадлежит внутреннему холсту chartCanvas
                    tb = new TextBlock();
                    tb.Text = dx.ToString();
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);
                    Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                    Canvas.SetTop(tb, pt.Y + 2 + size.Height / 2);
                }
            }
            // Create horizontal gridlines and y tick marks:
            //IsXGrid = false;
            if (IsXGrid == true)
            {
                for (int i = yStart; i <= yEnd; i++)
                {
                    gridline = new Line();
                    AddLinePattern();   //выбираем рисунок разметочной линии, устанавливая параметры для gridline
                    dy = i * yTick;
                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                    ChartCanvas.Children.Add(gridline);
                    pt = NormalizePoint(new Point(Xmin, dy));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X + 5;
                    tick.Y2 = pt.Y;
                    ChartCanvas.Children.Add(tick);
                    tb = new TextBlock();
                    tb.Text = dy.ToString();
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);
                    Canvas.SetRight(tb, ChartCanvas.Width + 10);//устанавливаем наименование метки в 10 ед. влево от правого края внутреннего холста. 
                    Canvas.SetTop(tb, pt.Y);
                }
            }
            // Add title and labels:
            tbTitle.Text = Title;
            tbXLabel.Text = XLabel;
            tbYLabel.Text = YLabel;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
        }
        public void AddLinePattern()
        {
            gridline.Stroke = GridlineColor;
            gridline.StrokeThickness = 1;
            switch (GridlinePattern)
            {
                case GridlinePatternEnum.Dash:
                    gridline.StrokeDashArray =
                    new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case GridlinePatternEnum.Dot:
                    gridline.StrokeDashArray =
                    new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case GridlinePatternEnum.DashDot:
                    gridline.StrokeDashArray =
                    new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }
        public enum GridlinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4
        }
        public double TimeSpanToDouble(TimeSpan ts)
        {
            DateTime dt = DateTime.Parse("1 Jan");
            double d1 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            dt += ts;
            double d2 = BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
            return d2 - d1;
        }
        public double DateToDouble(string date)
        {
            DateTime dt = DateTime.Parse(date);
            return BitConverter.ToDouble(BitConverter.GetBytes(dt.Ticks), 0);
        }
        public DateTime DoubleToDate(double d)
        {
            return new DateTime(BitConverter.ToInt64(BitConverter.GetBytes(d), 0));
        }
        public double OptimalSpacing(double original)
        {
            double[] da = { 1.0, 2.0, 5.0 };
            double multiplier = Math.Pow(10, Math.Floor(Math.Log(original) / Math.Log(10)));
            double dmin = 100 * multiplier;
            double spacing = 0.0;
            double mn = 100;
            foreach (double d in da)
            {
                double delta = Math.Abs(original - d * multiplier);
                if (delta < dmin)
                {
                    dmin = delta;
                    spacing = d * multiplier;
                }
                if (d < mn)
                {
                    mn = d;
                }
            }
            if (Math.Abs(original - 10 * mn * multiplier) < Math.Abs(original - spacing))
                spacing = 10 * mn * multiplier;
            return spacing;
        }
    }
}