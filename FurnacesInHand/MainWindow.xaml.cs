using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static FurnacesInHand.ServiceFunctions;
using System.Diagnostics;

namespace FurnacesInHand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DownLoad_script_file_name="copy_script.i";
        const string Download_server_credetials = @"-h 10.10.48.24 -U Reader -d fttm -p 5432"; 
        const string UpLoad_script_file_name = "restore_script.i";
        const string Upload_server_credetials =   @"-h localhost -U postgres -d fttm -p 5432";
        FurnacesModelLocal context;
        DbConnection conn;
        public Int32 numberOfFurnace;
        public List<TimeParameterPair> Voltage_graph_pairs;
        public List<TimeParameterPair> Current_graph_pairs;
        public List<TimeParameterPair> Vacuum_graph_pairs;
        public List<TimeParameterPair> SolenoidU_graph_pairs;
        public List<TimeParameterPair> SolenoidI_graph_pairs;
 
        public String parameter;
        public DateTime startTime;
        public DateTime finishTime;

        enum Parameters
        {Напряжение,Ток,Вакуум,Ток_соленоида, Напряжение_на_соленоиде}
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
                        parameter = "Arc_U";
                        var read_parameters_vdp01 = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp01;
                        Voltage_graph_pairs = (from par in read_parameters_vdp01 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp01 = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp01;
                        Current_graph_pairs = (from par in read_parameters_vdp01 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp01 = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp01;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp01 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp01 = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp01;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp01 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp01 = this.context.vdp01.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp01;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp01 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 2:
                        parameter = "Arc_U";
                        var read_parameters_vdp02 = this.context.vdp02.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp02;
                        Voltage_graph_pairs = (from par in read_parameters_vdp02 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp02 = this.context.vdp02.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp02;
                        Current_graph_pairs = (from par in read_parameters_vdp02 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp02 = this.context.vdp02.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp02;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp02 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        parameter = "Sol_U";
                        read_parameters_vdp02 = this.context.vdp02.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp02;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp02 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp02 = this.context.vdp02.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp02;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp02 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 3:
                        parameter = "Arc_U";
                        var read_parameters_vdp03 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp03;
                        Voltage_graph_pairs = (from par in read_parameters_vdp03 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp03 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp03;
                        Current_graph_pairs = (from par in read_parameters_vdp03 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp03 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp03;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp03 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp03 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp03;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp03 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp03 = this.context.vdp03.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp03;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp03 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 4:
                        parameter = "Arc_U";
                        var read_parameters_vdp04 = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp04;
                        Voltage_graph_pairs = (from par in read_parameters_vdp04 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp04 = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp04;
                        Current_graph_pairs = (from par in read_parameters_vdp04 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp04 = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp04;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp04 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp04 = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp04;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp04 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp04 = this.context.vdp04.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp04;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp04 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 5:
                        parameter = "Arc_U";
                        var read_parameters_vdp05 = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp05;
                        Voltage_graph_pairs = (from par in read_parameters_vdp05 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp05 = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp05;
                        Current_graph_pairs = (from par in read_parameters_vdp05 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp05 = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp05;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp05 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp05 = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp05;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp05 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp05 = this.context.vdp05.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp05;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp05 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 6:
                        parameter = "Arc_U";
                        var read_parameters_vdp06 = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp06;
                        Voltage_graph_pairs = (from par in read_parameters_vdp06 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp06 = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp06;
                        Current_graph_pairs = (from par in read_parameters_vdp06 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp06 = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp06;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp06 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp06 = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp06;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp06 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp06 = this.context.vdp06.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp06;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp06 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 7:
                        parameter = "Arc_U";
                        var read_parameters_vdp07 = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp07;
                        Voltage_graph_pairs = (from par in read_parameters_vdp07 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp07 = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp07;
                        Current_graph_pairs = (from par in read_parameters_vdp07 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp07 = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp07;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp07 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp07 = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp07;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp07 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp07 = this.context.vdp07.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp07;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp07 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 8:
                        parameter = "Arc_U";
                        var read_parameters_vdp08 = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp08;
                        Voltage_graph_pairs = (from par in read_parameters_vdp08 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp08 = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp08;
                        Current_graph_pairs = (from par in read_parameters_vdp08 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp08 = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp08;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp08 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp08 = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp08;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp08 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp08 = this.context.vdp08.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp08;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp08 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 9:
                        parameter = "Arc_U";
                        var read_parameters_vdp09 = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp09;
                        Voltage_graph_pairs = (from par in read_parameters_vdp09 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp09 = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp09;
                        Current_graph_pairs = (from par in read_parameters_vdp09 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp09 = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp09;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp09 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp09 = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp09;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp09 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp09 = this.context.vdp09.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp09;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp09 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 10:
                        parameter = "Arc_U";
                        var read_parameters_vdp10 = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp10;
                        Voltage_graph_pairs = (from par in read_parameters_vdp10 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp10 = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp10;
                        Current_graph_pairs = (from par in read_parameters_vdp10 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp10 = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp10;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp10 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp10 = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp10;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp10 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp10 = this.context.vdp10.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp10;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp10 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 15:
                        parameter = "Arc_U";
                        var read_parameters_vdp15 = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp15;
                        Voltage_graph_pairs = (from par in read_parameters_vdp15 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp15 = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp15;
                        Current_graph_pairs = (from par in read_parameters_vdp15 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp15 = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp15;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp15 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp15 = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp15;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp15 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp15 = this.context.vdp15.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp15;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp15 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 16:
                        parameter = "Arc_U";
                        var read_parameters_vdp16 = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp16;
                        Voltage_graph_pairs = (from par in read_parameters_vdp16 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp16 = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp16;
                        Current_graph_pairs = (from par in read_parameters_vdp16 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp16 = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp16;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp16 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp16 = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp16;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp16 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp16 = this.context.vdp16.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp16;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp16 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 17:
                        parameter = "Arc_U";
                        var read_parameters_vdp17 = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp17;
                        Voltage_graph_pairs = (from par in read_parameters_vdp17 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp17 = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp17;
                        Current_graph_pairs = (from par in read_parameters_vdp17 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp17 = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp17;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp17 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp17 = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp17;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp17 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp17 = this.context.vdp17.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp17;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp17 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 18:
                        parameter = "Arc_U";
                        var read_parameters_vdp18 = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        voltageValues.ItemsSource = read_parameters_vdp18;
                        Voltage_graph_pairs = (from par in read_parameters_vdp18 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp18 = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp18;
                        Current_graph_pairs = (from par in read_parameters_vdp18 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp18 = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp18;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp18 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp18 = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp18;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp18 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp18 = this.context.vdp18.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp18;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp18 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 19:
                        parameter = "Arc_U";
                        var read_parameters_vdp19 = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp19.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp19;
                        Voltage_graph_pairs = (from par in read_parameters_vdp19 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp19 = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp19;
                        Current_graph_pairs = (from par in read_parameters_vdp19 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp19 = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp19;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp19 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp19 = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp19;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp19 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp19 = this.context.vdp19.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp19;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp19 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 20:
                        parameter = "Arc_U";
                        var read_parameters_vdp20 = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp20.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp20;
                        Voltage_graph_pairs = (from par in read_parameters_vdp20 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp20 = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp20;
                        Current_graph_pairs =(from par in read_parameters_vdp20 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp20 = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp20;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp20 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp20 = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp20;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp20 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp20 = this.context.vdp20.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp20;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp20 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 29:
                        parameter = "Arc_U";
                        var read_parameters_vdp29 = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp29.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp29;
                        Voltage_graph_pairs = (from par in read_parameters_vdp29 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp29 = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp29;
                        Current_graph_pairs = (from par in read_parameters_vdp29 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp29 = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp29;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp29 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp29 = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp29;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp29 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp29 = this.context.vdp29.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp29;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp29 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 30:
                        parameter = "Arc_U";
                        var read_parameters_vdp30 = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp30.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp30;
                        Voltage_graph_pairs = (from par in read_parameters_vdp30 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp30 = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp30;
                        Current_graph_pairs = (from par in read_parameters_vdp30 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp30 = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp30;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp30 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp30 = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp30;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp30 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp30 = this.context.vdp30.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp30;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp30 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 31:
                        parameter = "Arc_U";
                        var read_parameters_vdp31 = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp31.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp31;
                        Voltage_graph_pairs = (from par in read_parameters_vdp31 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp31 = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp31;
                        Current_graph_pairs = (from par in read_parameters_vdp31 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp31 = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp31;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp31 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp31 = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp31;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp31 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp31 = this.context.vdp31.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp31;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp31 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 32:
                        parameter = "Arc_U";
                        var read_parameters_vdp32 = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp32.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp32;
                        Voltage_graph_pairs = (from par in read_parameters_vdp32 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp32 = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp32;
                        Current_graph_pairs = (from par in read_parameters_vdp32 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp32 = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp32;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp32 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp32 = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp32;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp32 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp32 = this.context.vdp32.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp32;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp32 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 33:
                        parameter = "Arc_U";
                        var read_parameters_vdp33 = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp33.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp33;
                        Voltage_graph_pairs = (from par in read_parameters_vdp33 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp33 = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp33;
                        Current_graph_pairs = (from par in read_parameters_vdp33 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp33 = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp33;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp33 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp33 = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp33;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp33 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp33 = this.context.vdp33.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp33;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp33 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
                        break;
                    case 44:
                        parameter = "Arc_U";
                        var read_parameters_vdp44 = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        MessageBox.Show($"We have {read_parameters_vdp44.Length} par(s).");
                        voltageValues.ItemsSource = read_parameters_vdp44;
                        Voltage_graph_pairs = (from par in read_parameters_vdp44 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        voltagePlot(Voltage_graph_pairs);
                        parameter = "Arc_I";
                        read_parameters_vdp44 = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        currentValues.ItemsSource = read_parameters_vdp44;
                        Current_graph_pairs = (from par in read_parameters_vdp44 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        currentPlot(Current_graph_pairs);
                        parameter = "Pressure";
                        read_parameters_vdp44 = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        pressureValues.ItemsSource = read_parameters_vdp44;
                        Vacuum_graph_pairs = (from par in read_parameters_vdp44 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val}).ToList();
                        vacuumPlot(Vacuum_graph_pairs);
                        pressureValues.ItemsSource = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        parameter = "Sol_U";
                        read_parameters_vdp44 = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solVoltageValues.ItemsSource = read_parameters_vdp44;
                        SolenoidU_graph_pairs = (from par in read_parameters_vdp44 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidUPlot(SolenoidU_graph_pairs);
                        parameter = "Sol_I";
                        read_parameters_vdp44 = this.context.vdp44.Where(x => x.tagname == parameter && x.dateandtime >= startTime && x.dateandtime <= finishTime).OrderBy(x => x.id).ToArray();
                        solCurrentValues.ItemsSource = read_parameters_vdp44;
                        SolenoidI_graph_pairs = (from par in read_parameters_vdp44 select new TimeParameterPair() { dt = (DateTime)par.dateandtime, parameter = (double)par.val }).ToList();
                        solenoidIPlot(SolenoidI_graph_pairs);
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
            MessageBox.Show("Копируем таблицу с сервера");
            //find the path to the psql
            //taken from http://csharptest.net/526/how-to-search-the-environments-path-for-an-exe-or-dll/index.html
            
            var path_to_postgres = FindExePath("psql.exe");
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = path_to_postgres;
            //корректируем номер печи в copy_script.i
            string current_directory_path = Directory.GetCurrentDirectory();
            string path_to_download_script = current_directory_path + "\\" + DownLoad_script_file_name;
            string[] downLoadScript = File.ReadAllLines(path_to_download_script);
            //скачиваем с удалённого сервера
            startInfo.Arguments = Download_server_credetials + " -f " + $"{ DownLoad_script_file_name}";
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                MessageBox.Show("Не удалось соединение с цеховой базой данных.");
            }
            Process.Start(startInfo);
            //далее следует закачка на локальный сервер ...
            startInfo.Arguments = Upload_server_credetials+ " -f " + $"{UpLoad_script_file_name}";
            Process.Start(startInfo);
            secondDataBase.IsChecked = true;
            //using (var context = new FurnacesModelLocalNext()) //создали контекст взаимодействия с базой данных
            //{
            //    var pars = context.vdp03.Where(x => x.tagname == "Arc_U").OrderBy(x => x.id).Take(9).ToArray<vdp03>();
            //    conn = context.Database.Connection; //извлекли объект для соединения с БД
            //    conn.Open(); //открыли соединение
            //    MessageBox.Show(String.Format("PostgreSQL version is {0}", conn.ServerVersion));
            //    MessageBox.Show($"We have {pars.Length} par(s).");
            //    voltageValues.ItemsSource = pars;
            //}
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

        private void VoltagePlot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }

        private void VoltagePlot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
        }
        private void PutTheCursor(Point clickPoint)
        {
            //Show the nearest values in ListBoxes(TrxtBoxes)! 
            VoltagePlot.VerticalCursor(clickPoint);
            CurrentPlot.VerticalCursor(clickPoint);
            VacuumPlot.VerticalCursor(clickPoint);
            SolenoidUPlot.VerticalCursor(clickPoint);
            SolenoidIPlot.VerticalCursor(clickPoint);
        }

        private void CurrentPlot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }
        private void CurrentPlot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
        }
        private void VacuumPlot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }
        private void VacuumPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
        }

 
        private void SolenoidUPlot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }

        private void SolenoidUPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
        }

        private void SolenoidIPlot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }

        private void SolenoidIPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
        }
    }


}
