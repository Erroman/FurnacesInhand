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
using RulerControls;
using static RulerControls.TransformWorldToScreen;
namespace MatrixTransformation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            const int Xmin = 0;
            const int Xmax = 1;
            TransformWorldToScreen.PrepareTransformations(Xmin, Xmax, 2,10, Xmin, Xmax, 10,15);
            Point pW = new Point(-1001,5);
            Point pD = WtoD(pW);

            this.show.Content = $"pD.X = {pD.X} " + $"pD.Y = {pD.Y}";

        }
    }
}
