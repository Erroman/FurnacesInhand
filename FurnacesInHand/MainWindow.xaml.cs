using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace FurnacesInHand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbConnection conn;
        public MainWindow()
        {
            InitializeComponent();
            using (var context = new FurnacesModel())
            {
                conn = context.Database.Connection;
                conn.Open();
                MessageBox.Show(String.Format("PostgreSQL version is {0}",conn.ServerVersion));
                //  var cars = context.Cars.ToArray();
                //  Console.WriteLine($"We have {cars.Length} car(s).");
            }
        }
    }
}
