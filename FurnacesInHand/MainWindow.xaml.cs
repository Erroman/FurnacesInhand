using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
 
        public ObservableCollection<vdp03> inList;
        public ObservableCollection<string> inListString;
        public String parameter = "Arc_U";
        public DateTime startTime;
        public DateTime finishTime;
        public MainWindow()
        {
            InitializeComponent();

            this.numberOfFurnace = Properties.Settings.Default.numberOfFurnace;
            ChooseTheItemInTheTreeForTheFurnace(this.numberOfFurnace);

            begTime.Text = Properties.Settings.Default.begTime;
            begTimeMin.Text = Properties.Settings.Default.begTimeMin;
            endTime.Text = Properties.Settings.Default.endTime;
            endTimeMin.Text = Properties.Settings.Default.endTimeMin;

            SetDigitalStartAndFinishTimes();

            firstDataBase.IsChecked = Properties.Settings.Default.firstDatabase;
            secondDataBase.IsChecked = Properties.Settings.Default.secondDatabase;

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
            Properties.Settings.Default.begTime = begTime.Text;
            Properties.Settings.Default.endTime = endTime.Text;
            Properties.Settings.Default.begTimeMin = begTimeMin.Text;
            Properties.Settings.Default.endTimeMin = endTimeMin.Text;

            Properties.Settings.Default.Save();
        }
        private void Base_Chosen()
        {
            txtb.Text = "Выбрана ";
            if ((bool)firstDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)firstDataBase.Content); MapTheRemoteBase(); }
            else if ((bool)secondDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)secondDataBase.Content); MapTheLocalBase(); }
            else txtb.Text = "Произведите выбор базы данных!";
        }
        private void MapTheLocalBase()
        {
            using (this.context = new FurnacesModelLocal()) //создали контекст взаимодействия с базой данных
            {
                conn = this.context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                //MessageBox.Show(String.Format("PostgreSQL version is {0}", conn.ServerVersion));
                SetDigitalStartAndFinishTimes();
                switch (this.numberOfFurnace)
                {
                    case 1:
                        var par1 = this.context.vdp01.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par1;
                        MessageBox.Show($"We have {par1.Length} par(s).");
                        break;
                    case 2:
                        var par2 = this.context.vdp02.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par2;
                        MessageBox.Show($"We have {par2.Length} par(s).");
                        break;
                    case 3:
                        parameter = "Arc_U";
                        var par3 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = par3;
                        var graph_pairs = from par in par3 select new TimeParameterPair(){ dt = (DateTime)par.dateandtime, parameter = (double)par.val};
                        MessageBox.Show($"We have {par3.Length} par(s).");
                        voltagePlot(graph_pairs);
                        parameter = "Arc_I";
                        par3 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = par3;
                        graph_pairs = from par in par3 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val };
                        currentPlot(graph_pairs);
                        parameter = "Pressure";
                        par3 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = par3;
                        graph_pairs = from par in par3 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val };
                        vacuumPlot(graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        par3 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = par3;
                        graph_pairs = from par in par3 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val };
                        solenoidUPlot(graph_pairs);
                        parameter = "Sol_I";
                        solCurrentValues.ItemsSource = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        break;
                    case 7:
                        var par7 = this.context.vdp07.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par7;

                        MessageBox.Show($"We have {par7.Length} par(s).");
                        break;
                    case 8:
                        var par8 = this.context.vdp08.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par8;

                        MessageBox.Show($"We have {par8.Length} par(s).");
                        break;

                    case 9:
                        var par9 = this.context.vdp09.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par9;

                        MessageBox.Show($"We have {par9.Length} par(s).");
                        break;
                    case 10:
                        var par10 = this.context.vdp10.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par10;

                        MessageBox.Show($"We have {par10.Length} par(s).");
                        break;
                    case 15:
                        var par15 = this.context.vdp15.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par15;

                        MessageBox.Show($"We have {par15.Length} par(s).");
                        break;
                    case 16:
                        var par16 = this.context.vdp16.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par16;

                        MessageBox.Show($"We have {par16.Length} par(s).");
                        break;
                    case 17:
                        var par17 = this.context.vdp17.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par17;

                        MessageBox.Show($"We have {par17.Length} par(s).");
                        break;
                    case 18:
                        var par18 = this.context.vdp18.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par18;

                        MessageBox.Show($"We have {par18.Length} par(s).");
                        break;
                    case 19:
                        var par19 = this.context.vdp19.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par19;

                        MessageBox.Show($"We have {par19.Length} par(s).");
                        break;
                    case 20:
                        var par20 = this.context.vdp20.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par20;

                        MessageBox.Show($"We have {par20.Length} par(s).");
                        break;
                    case 29:
                        var par29 = this.context.vdp29.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par29;

                        MessageBox.Show($"We have {par29.Length} par(s).");
                        break;
                    case 30:
                        var par30 = this.context.vdp30.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par30;

                        MessageBox.Show($"We have {par30.Length} par(s).");
                        break;
                    case 31:
                        var par31 = this.context.vdp31.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par31;

                        MessageBox.Show($"We have {par31.Length} par(s).");
                        break;
                    case 32:
                        var par32 = this.context.vdp32.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par32;

                        MessageBox.Show($"We have {par32.Length} par(s).");
                        break;
                    case 33:
                        var par33 = this.context.vdp33.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par33;

                        MessageBox.Show($"We have {par33.Length} par(s).");
                        break;
                    case 44:
                        var par44 = this.context.vdp44.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Skip(1000000).Take(25).ToArray();
                        voltageValues.ItemsSource = par44;

                        MessageBox.Show($"We have {par44.Length} par(s).");
                        break;
                }

            }
        }
        private void SetDigitalStartAndFinishTimes()
        {

            startTime = DateTime.Parse(begTime.Text == "" ? "2000-01-01" : begTime.Text);
            DateTime startTimeMin = DateTime.Parse(begTimeMin.Text);
            startTime = startTime.AddHours(startTimeMin.Hour);
            startTime = startTime.AddMinutes(startTimeMin.Minute);

            finishTime = DateTime.Parse(endTime.Text == "" ? "2050-01-01" : endTime.Text);
            DateTime finishTimeMin = DateTime.Parse(endTimeMin.Text);
            finishTime = finishTime.AddHours(finishTimeMin.Hour);
            finishTime = finishTime.AddMinutes(finishTimeMin.Minute);

        }
        private void MapTheRemoteBase()
        {
            using (var context = new FurnacesModelLocalNext()) //создали контекст взаимодействия с базой данных
            {
                var pars = context.vdp03.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Take(9).ToArray<vdp03>();
                conn = context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                MessageBox.Show(String.Format("PostgreSQL version is {0}", conn.ServerVersion));
                MessageBox.Show($"We have {pars.Length} par(s).");
                //for (int i = 0; i < 10; i++)
                //MessageBox.Show(pars[i].dateandtime.ToString()+ " " +pars[i].id+" "+pars[i].mks+" "+pars[i].tagname);
                //context.vdp03.Load();
                //inList = context.vdp03.Local;
                //Binding b = new Binding();
                //b.Source = new string[] { "11", "22", "33", "11", "22", "33" };
                //b.Source = new MyData();
                //b.Source = pars;

                voltageValues.ItemsSource = pars;




            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MapTheLocalBase();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = ListOfFurnaces.SelectedItem as TreeViewItem;
            if (item.Parent is TreeViewItem)
            {

                numberOfFurnace = extractNumberOfFurnaceFromItsNameInTheTreeMenu(item.Header.ToString());
                PutTheNumberOfTheFurnaceIntoTheTitle();
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
            return labelText.Substring(0, labelText.IndexOf("№") + 1) + this.numberOfFurnace;
        }
        private void PutTheNumberOfTheFurnaceIntoTheTitle()
        {
            this.Title = "Выбрана печь №" + this.numberOfFurnace;
        }
        private Int32 extractNumberOfFurnaceFromItsNameInTheTreeMenu(string nameOfFurnace)
        {
            return Int32.Parse(nameOfFurnace.Substring(nameOfFurnace.IndexOf("№") + 1));
        }

        private Int32 extractNumberOfFurnaceFromTheNameOfTheProperty(string nameOfProperty)
        {
            return nameOfProperty.Contains("vdp") ? Int32.Parse(nameOfProperty.Substring(3)) : -1;
        }

    }


}
