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
using static FurnacesInHand.ServiceFunctions;

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
            firstDataBase.IsChecked = Properties.Settings.Default.firstDatabase;
            secondDataBase.IsChecked = Properties.Settings.Default.secondDatabase;
            Base_Chosen();
   

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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton li = (sender as RadioButton);
            Base_Chosen();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.firstDatabase = firstDataBase.IsChecked;
            Properties.Settings.Default.secondDatabase = secondDataBase.IsChecked;
            Properties.Settings.Default.Save();
        }
        private void Base_Chosen()
        {
            txtb.Text = "Выбрана ";
            if ((bool)firstDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)firstDataBase.Content); MapTheRemoteBase(); }
            else if ((bool)secondDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)secondDataBase.Content); MapTheLocalBase();}
            else txtb.Text = "Произведите выбор базы данных!";
        }
        private void MapTheLocalBase()
        {
            using (var context = new FurnacesModelLocal()) //создали контекст взаимодействия с базой данных
            {
                var pars = context.vdp03.ToArray();
                conn = context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                             MessageBox.Show(String.Format("PostgreSQL version is {0}",conn.ServerVersion));
                             MessageBox.Show($"We have {pars.Length} par(s).");
                             //for (int i = 0; i < 10; i++)
                             //MessageBox.Show(pars[i].dateandtime.ToString()+ " " +pars[i].id+" "+pars[i].mks+" "+pars[i].tagname);
                             //context.vdp03.Load();
                             //inList = context.vdp03.Local;
                             //Binding b = new Binding();
                             //b.Source = new string[] { "11", "22", "33", "11", "22", "33" };
                             //b.Source = new MyData();
                             //b.Source = pars;

                //parameterValues.SetBinding(ListBox.ItemsSourceProperty, b);
                parameterValues.ItemsSource = pars;




            }
        }
        private void MapTheRemoteBase()
        {
            using (var context = new FurnacesModel()) //создали контекст взаимодействия с базой данных
            {
                var pars = context.vdp03.ToArray<vdp03>();
                conn = context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                             MessageBox.Show(String.Format("PostgreSQL version is {0}",conn.ServerVersion));
                             MessageBox.Show($"We have {pars.Length} par(s).");
                             //for (int i = 0; i < 10; i++)
                             //MessageBox.Show(pars[i].dateandtime.ToString()+ " " +pars[i].id+" "+pars[i].mks+" "+pars[i].tagname);
                             //context.vdp03.Load();
                             //inList = context.vdp03.Local;
                             //Binding b = new Binding();
                             //b.Source = new string[] { "11", "22", "33", "11", "22", "33" };
                             //b.Source = new MyData();
                             //b.Source = pars;

                parameterValues.ItemsSource = pars;




            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var contextToWhich = new FurnacesModelLocal()) //создали контекст взаимодействия с базой данных
            {
                using (var contextFromWhich = new FurnacesModel()) //создали контекст взаимодействия с базой данных
                {
                    var inMemory = from x in contextFromWhich.vdp03 select x ;
                    contextToWhich.vdp03.RemoveRange(from x in contextToWhich.vdp03 select x);
                    //contextToWhich.vdp03.AddRange(inMemory.Take<vdp03>(100));
                    contextToWhich.SaveChanges();
                    MessageBox.Show("Done!");


                }

            }

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
