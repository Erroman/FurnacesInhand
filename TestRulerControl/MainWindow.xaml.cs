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

namespace TestRulerControl
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.StartTime.Dt = new DateTime(2019, 10, 20,5,5,5,515);
            this.EndTime.Dt =   new DateTime(2019, 10, 22,15,15,15,715);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.rC.BuildTimeAxis();
        }
  
    }
}
