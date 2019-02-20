using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Common;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Data;

namespace FurnacesInHand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbConnection conn;
        public ObservableCollection<vdp03> inList;
        public ObservableCollection<string> inListString;
        public MainWindow()
        {
            InitializeComponent();

            using (var context = new FurnacesModel()) //создали контекст взаимодействия с базой данных
            {
                
                conn = context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                             //MessageBox.Show(String.Format("PostgreSQL version is {0}",conn.ServerVersion));

                //for (int i = 0; i < 10; i++)
                //MessageBox.Show(pars[i].dateandtime.ToString()+ " " +pars[i].id+" "+pars[i].mks+" "+pars[i].tagname);
                //context.vdp03.Load();
                //inList = context.vdp03.Local;
                //Binding b = new Binding();
                //b.Source = new string[] { "11", "22", "33", "11", "22", "33" };
                //b.Source = new MyData();
                //b.Source =pars;

                //parameterValues.SetBinding(ListBox.ItemsSourceProperty, b);
                var pars = context.vdp03.ToArray();
                parameterValues.ItemsSource = pars;
                //MessageBox.Show($"We have {pars.Length} par(s).");



            }

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //inList = new MyData();
            //Binding b = new Binding();
            //b.Source = inList;
            //parameterValues.SetBinding(ListBox.ItemsSourceProperty, b);
            //var be = parameterValues.GetBindingExpression(ListBox.ItemsSourceProperty);
            //be.UpdateTarget();
            //be.UpdateSource();

            //parameterValues.ItemsSource = inList;
        }
    }
    public class MyData : ObservableCollection<string>
    {
        public MyData()
        {
            Add("Item 1");
            Add("Item 2");
            Add("Item 3");
        }
    }

}
