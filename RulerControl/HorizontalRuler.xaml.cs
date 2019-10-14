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
        public HorizontalRuler()
        {
            InitializeComponent();
            this.DataContext = new DateTimeRangesViewModel();
        }


        public DateTime StartOfScale
        {
            get { return (DateTime)GetValue(StartOfScaleProperty); }
            set { SetValue(StartOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartOfScaleProperty =
            DependencyProperty.Register("StartOfScale", typeof(DateTime), typeof(HorizontalRuler));


        public DateTime EndOfScale
        {
            get { return (DateTime)GetValue(EndOfScaleProperty); }
            set { SetValue(EndOfScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndOfScale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndOfScaleProperty =
            DependencyProperty.Register("EndOfScale", typeof(DateTime), typeof(HorizontalRuler));




    }
}
