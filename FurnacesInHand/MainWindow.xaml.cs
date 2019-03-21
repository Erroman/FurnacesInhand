using System;
using System.Reflection;
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
        FurnacesModelLocal context;
        DbConnection conn;
        Int32 numberOfFurnace;
        Dictionary<int, object> ParameterData;
        public ObservableCollection<vdp03> inList;
        public ObservableCollection<string> inListString;
        public MainWindow()
        {
            InitializeComponent();
            this.numberOfFurnace = Properties.Settings.Default.numberOfFurnace;
            firstDataBase.IsChecked = Properties.Settings.Default.firstDatabase;
            secondDataBase.IsChecked = Properties.Settings.Default.secondDatabase;

            ChooseTheItemInTheTreeForTheFurnace(this.numberOfFurnace);

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
            Properties.Settings.Default.firstDatabase = (bool)firstDataBase.IsChecked;
            Properties.Settings.Default.secondDatabase = (bool)secondDataBase.IsChecked;
            Properties.Settings.Default.numberOfFurnace = this.numberOfFurnace;
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
            using (this.context = new FurnacesModelLocal()) //создали контекст взаимодействия с базой данных
            {
                //var pars = context.Database.SqlQuery(typeof(vdp03),"Select tagname,val from vdp03 order by id limit 10 offset 0");
                //select * from vdp04 order by id limit 100 offset 19592897; -- this SQL string should be put into call
                //select * from vdp06 order by id limit 10 offset 30348545;  -- or this one
                conn = this.context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                ParameterData = new Dictionary<int, object>()
                {
                    [1] = this.context.vdp01,
                    [2] = this.context.vdp02,
                    [3] = this.context.vdp03,
                    [4] = this.context.vdp04,
                    [5] = this.context.vdp05,
                    [6] = this.context.vdp06,
                    [7] = this.context.vdp07,
                    [8] = this.context.vdp08,

                };
                object furnacedata=null;
                Type furnaceDataType=null;
                //extract the property <number-of-furnace> from the context of DbSet type.
                Type contextType = typeof(FurnacesModelLocal);
                foreach (PropertyInfo pi in contextType.GetProperties())
                {
                    //get the name of the property
                    if (extractNumberOfFurnaceFromTheNameOfTheProperty(pi.Name) == this.numberOfFurnace)
                    {
                        furnacedata = pi.GetValue(this.context, null);
                        furnaceDataType = furnacedata.GetType();
                        
                    }
                }
                var pech = Convert.ChangeType(furnacedata, furnaceDataType);
                //var pars = pech.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                //var pars = furnacedata.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                //var pars = this.context.vdp08.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                MessageBox.Show(String.Format("PostgreSQL version is {0}", conn.ServerVersion));
                switch (this.numberOfFurnace)
                {
                    case 1:
                        var par1 = this.context.vdp01.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        parameterValues.ItemsSource = par1;
                        numberOfFurnaceLabel.Content = putNumberOfFurnaceIntoTheLabel(numberOfFurnaceLabel.Content as string);
                        MessageBox.Show($"We have {par1.Length} par(s).");
                        break;
                    case 2:
                        var par2 = this.context.vdp02.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        parameterValues.ItemsSource = par2;
                        numberOfFurnaceLabel.Content = putNumberOfFurnaceIntoTheLabel(numberOfFurnaceLabel.Content as string);
                        MessageBox.Show($"We have {par2.Length} par(s).");
                        break;
                    case 7:
                        var par7 = this.context.vdp07.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        parameterValues.ItemsSource = par7;
                        numberOfFurnaceLabel.Content = putNumberOfFurnaceIntoTheLabel(numberOfFurnaceLabel.Content as string);
                        MessageBox.Show($"We have {par7.Length} par(s).");
                        break;
                    case 8:
                        var par8 = this.context.vdp08.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        parameterValues.ItemsSource = par8;
                        numberOfFurnaceLabel.Content = putNumberOfFurnaceIntoTheLabel(numberOfFurnaceLabel.Content as string);
                        MessageBox.Show($"We have {par8.Length} par(s).");
                        break;

                    case 9:
                        var par9 = this.context.vdp09.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        parameterValues.ItemsSource = par9;
                        numberOfFurnaceLabel.Content = putNumberOfFurnaceIntoTheLabel(numberOfFurnaceLabel.Content as string);
                        MessageBox.Show($"We have {par9.Length} par(s).");
                        break;
                }
                //var pars = ParameterData[8].Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();


                //for (int i = 0; i < 10; i++)
                //MessageBox.Show(pars[i].dateandtime.ToString()+ " " +pars[i].id+" "+pars[i].mks+" "+pars[i].tagname);
                //context.vdp03.Load();
                //inList = context.vdp03.Local;
                //Binding b = new Binding();
                //b.Source = new string[] { "11", "22", "33", "11", "22", "33" };
                //b.Source = new MyData();
                //b.Source = pars;

                //parameterValues.SetBinding(ListBox.ItemsSourceProperty, b);





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

                numberOfFurnace = extractNumberOfFurnaceFromItsNameInTheTreeMenu(item.Header.ToString());
                MessageBox.Show($"A furnace # {numberOfFurnace} is chosen!");
                //выбор адреса данных печи в контексте

            }
                
        }

        private void ChooseTheItemInTheTreeForTheFurnace(int numberOfFurnace)
        {
            ListOfFurnaces.Items.MoveCurrentToFirst();
            ItemCollection listOfFurnaces = ((TreeViewItem)ListOfFurnaces.Items.CurrentItem).Items;
            while (listOfFurnaces.MoveCurrentToNext())
                if (extractNumberOfFurnaceFromItsNameInTheTreeMenu((listOfFurnaces.CurrentItem as TreeViewItem).Header.ToString()) == this.numberOfFurnace)
                    (listOfFurnaces.CurrentItem as TreeViewItem).IsSelected = true;
        }
        //"Печь №3".Substring(0,"Печь №3".IndexOf("№"))
        private string putNumberOfFurnaceIntoTheLabel(string labelText)
        {
            return labelText.Substring(0, labelText.IndexOf("№")+1) + this.numberOfFurnace;
        }
        private Int32 extractNumberOfFurnaceFromItsNameInTheTreeMenu(string nameOfFurnace)
        {
            return Int32.Parse(nameOfFurnace.Substring(nameOfFurnace.IndexOf("№") + 1));
        }
  
        private Int32 extractNumberOfFurnaceFromTheNameOfTheProperty(string nameOfProperty)
        {
            return nameOfProperty.Contains("vdp")?Int32.Parse(nameOfProperty.Substring(3)):-1;
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
