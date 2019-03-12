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
using System.Windows.Media;
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
                //var pars = context.Database.SqlQuery(typeof(vdp03),"Select tagname,val from vdp03 order by id limit 10 offset 0");
                //select * from vdp04 order by id limit 100 offset 19592897; -- this SQL string should be put into call
                //select * from vdp06 order by id limit 10 offset 30348545;  -- or this one
                conn = context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                var pars = context.vdp08.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
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
            using (var context = new FurnacesModelLocalNext()) //создали контекст взаимодействия с базой данных
            {
                var pars = context.vdp03.Where(x=>x.tagname=="Arc_U").OrderBy(x=>x.id).Take(9).ToArray<vdp03>();
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
                contextToWhich.Database.Delete();
                contextToWhich.Database.Create();
                using (var contextFromWhich = new FurnacesModel()) //создали контекст взаимодействия с базой данных
                {
                    var inMemory = from x in contextFromWhich.vdp03 select x ;
                    int chunkSize = 100000;
                    IQueryable<vdp03> restOfTheElements = inMemory;
                    while (restOfTheElements.Count() > 0)
                    {
                        contextToWhich.vdp03.AddRange(restOfTheElements.Take(chunkSize));
                        restOfTheElements = restOfTheElements.OrderBy(x=>x.id).Skip(chunkSize);
                        

                    }
                    
                    contextToWhich.SaveChanges();
                    MessageBox.Show(String.Format("Number of entries {0}",inMemory.Count<vdp03>()));


                }

            }

        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = ListOfFurnaces.SelectedItem as TreeViewItem;
            if(item.Parent is TreeViewItem)
            {
                
                string pech = item.Header.ToString();
                int pechNumber = Int32.Parse(pech.Substring(pech.IndexOf("№")+1));
                MessageBox.Show($"A furnace # {pechNumber} is chosen!");
            }
                
        }
        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
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
