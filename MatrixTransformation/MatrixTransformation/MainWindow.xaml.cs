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
            
            TransformWorldToScreen.PrepareTransformations(-5,-100,0,10,5.0,1000,0,15);
            Point pW = new Point(-1001,2);
            Point pD = WtoD(pW);

            this.show.Content = $"pD.X = {pD.X} " + $"pD.Y = {pD.Y}";

        }
    }
}
