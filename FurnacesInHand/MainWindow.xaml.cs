using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static FurnacesInHand.ServiceFunctions;

namespace FurnacesInHand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DownLoad_script_file_name="copy_script.i";
        const string Download_server_credetials = @"-h 10.10.48.24 -U Reader -d fttm -p 5432"; 
        //const string Download_server_credetials = @"-h localhost -U postgres -d fttm -p 5432";
        const string UpLoad_script_file_name = "restore_script.i";
        const string Upload_server_credetials =   @"-h localhost -U postgres -d fttm -p 5432";
        FurnacesModelLocal context;
        DbConnection conn;
        FurnacesInHandViewModel datacontext;
        public Int32 numberOfFurnace;
        public List<TimeParameterPair> Voltage_graph_pairs;
        public List<TimeParameterPair> Current_graph_pairs;
        public List<TimeParameterPair> Vacuum_graph_pairs;
        public List<TimeParameterPair> SolenoidU_graph_pairs;
        public List<TimeParameterPair> SolenoidI_graph_pairs;
 
        public String parameter;

        public DateTime startTime;  //начало и конец временного интервала для демонстрации параметров
        public DateTime finishTime;


        enum Parameters
        {Напряжение,Ток,Вакуум,Ток_соленоида, Напряжение_на_соленоиде}
        public MainWindow()
        {
            InitializeComponent();

            datacontext = (FurnacesInHandViewModel)this.DataContext;

            this.numberOfFurnace = Properties.Settings.Default.numberOfFurnace;
            ChooseTheItemInTheTreeForTheFurnace(this.numberOfFurnace);

            dtBegTime.Dt = Properties.Settings.Default.dtBegTime;
            dtEndTime.Dt = Properties.Settings.Default.dtEndTime;

            VoltageMin.Text = Properties.Settings.Default.lowerVoltage;
            VoltageMax.Text = Properties.Settings.Default.upperVoltage;
            CurrentMin.Text = Properties.Settings.Default.lowerCurrent;
            CurrentMax.Text = Properties.Settings.Default.upperCurrent;
            VacuumMin.Text = Properties.Settings.Default.lowerVacuum;
            VacuumMax.Text = Properties.Settings.Default.upperVacuum;
            SolenoidUMin.Text = Properties.Settings.Default.lowerUSolenoid;
            SolenoidUMax.Text = Properties.Settings.Default.upperUSolenoid;
            SolenoidIMin.Text = Properties.Settings.Default.lowerISolenoid;
            SolenoidIMax.Text = Properties.Settings.Default.upperISolenoid;


            SetDigitalStartAndFinishTimes("StartValues");

            //firstDataBase.IsChecked = Properties.Settings.Default.firstDatabase;
            //secondDataBase.IsChecked = Properties.Settings.Default.secondDatabase;

        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton li = (sender as RadioButton);
            //Base_Chosen();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.dtBegTime = dtBegTime.Dt;
            Properties.Settings.Default.dtEndTime = dtEndTime.Dt;
            //Properties.Settings.Default.secondDatabase = (bool)secondDataBase.IsChecked;
            Properties.Settings.Default.numberOfFurnace = this.numberOfFurnace;
            Properties.Settings.Default.lowerVoltage = VoltageMin.Text;
            Properties.Settings.Default.upperVoltage = VoltageMax.Text;
            Properties.Settings.Default.lowerCurrent = CurrentMin.Text;
            Properties.Settings.Default.upperCurrent = CurrentMax.Text;
            Properties.Settings.Default.lowerVacuum = VacuumMin.Text;
            Properties.Settings.Default.upperVacuum = VacuumMax.Text;
            Properties.Settings.Default.lowerUSolenoid = SolenoidUMin.Text;
            Properties.Settings.Default.upperUSolenoid = SolenoidUMax.Text;
            Properties.Settings.Default.lowerISolenoid = SolenoidIMin.Text;
            Properties.Settings.Default.upperISolenoid = SolenoidIMax.Text;

            Properties.Settings.Default.Save();
        }
        //private void Base_Chosen()
        //{
        //    txtb.Text = "Выбрана ";
        //    if ((bool)firstDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)firstDataBase.Content); MapTheRemoteBase(); }
        //    else if ((bool)secondDataBase.IsChecked) { txtb.Text += StringToLowerCase((string)secondDataBase.Content); MapTheLocalBase(); }
        //    else txtb.Text = "Произведите выбор базы данных!";
        //}
        private void MapTheLocalBase(string EdgeOrGlobalTimeBoundaries= "StartValues")
        {
           Updated.Text = "";
            using (this.context = new FurnacesModelLocal()) //создали контекст взаимодействия с базой данных
            {
                conn = this.context.Database.Connection; //извлекли объект для соединения с БД
                conn.Open(); //открыли соединение
                //MessageBox.Show(String.Format("PostgreSQL version is {0}", conn.ServerVersion));
                SetDigitalStartAndFinishTimes(EdgeOrGlobalTimeBoundaries);
                this.timeScale.BuildTimeAxis();
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
                        //voltageValues.ItemsSource = Voltage_graph_pairs;
                        //Apply  the DataTemplate here!
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
        private void SetDigitalStartAndFinishTimes(string WhatTimeInterval="StartValues")
        {

            
            if (WhatTimeInterval == "StartValues") 
            {
                datacontext.DtFixedEdgeBegTime = datacontext.DtBegTime;
                datacontext.DtFixedEdgeEndTime = datacontext.DtEndTime;
                datacontext.DtEdgeBegTime = datacontext.DtFixedEdgeBegTime;
                datacontext.DtEdgeEndTime = datacontext.DtFixedEdgeEndTime;
            }
            else 
            { 
                datacontext.DtFixedEdgeBegTime = datacontext.DtEdgeBegTime;
                datacontext.DtFixedEdgeEndTime = datacontext.DtEdgeEndTime;
            }
            timeRangeSlider.LowerValue = timeRangeSlider.Minimum;
            timeRangeSlider.UpperValue = timeRangeSlider.Maximum;
            startTime  = datacontext.DtFixedEdgeBegTime;
            finishTime = datacontext.DtFixedEdgeEndTime;
        }
        private void MapTheRemoteBase()
        {
            MessageBox.Show("Копируем таблицу с сервера");
            Updated.Text = "Обновление базы";
            //find the path to the psql
            //taken from http://csharptest.net/526/how-to-search-the-environments-path-for-an-exe-or-dll/index.html

            var path_to_postgres = FindExePath("psql.exe");
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = path_to_postgres;
            //корректируем номер печи в copy_script.i
            string current_directory_path = Directory.GetCurrentDirectory();
            string path_to_download_script = current_directory_path + "\\" + DownLoad_script_file_name;
            string downLoadScript = File.ReadAllText(path_to_download_script);
            Regex rgx = new Regex("vdp..");
            Regex rgx1 = new Regex(@"\btime..\b");
            string twoDigitsNumberOfFurnace = this.numberOfFurnace.ToString();
            twoDigitsNumberOfFurnace = this.numberOfFurnace > 9 ? twoDigitsNumberOfFurnace : "0" + twoDigitsNumberOfFurnace;
            downLoadScript = rgx.Replace(downLoadScript, "vdp" + twoDigitsNumberOfFurnace);
            File.WriteAllText(path_to_download_script, downLoadScript);
            //скачиваем с удалённого сервера
            startInfo.Arguments = Download_server_credetials + " -f " + $"{ DownLoad_script_file_name}"+" -o download.log";
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
             //Далее следует закачка на локальный сервер,
            //корректируем номер печи в скрипте для закачки
            string path_to_upload_script = current_directory_path + "\\" + UpLoad_script_file_name;
            string upLoadScript = File.ReadAllText(path_to_upload_script);
            upLoadScript = rgx1.Replace(rgx.Replace(upLoadScript, "vdp" + twoDigitsNumberOfFurnace), "time" + twoDigitsNumberOfFurnace);
            File.WriteAllText(path_to_upload_script, upLoadScript);
            startInfo.Arguments = Upload_server_credetials+ " -f " + $"{UpLoad_script_file_name}"+" -o upload.log";
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
                MessageBox.Show("Не удалась выгрузка в локальную базу данных");
            }
            Updated.Text = "База обновлена";
            //secondDataBase.IsChecked = true;
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
        private void UpLoadTheDataBaseFromTheCopy()
        {
            MessageBox.Show("Восстанавливаем базу:");
            Updated.Text = "Обновление базы";
            //find the path to the psql
            //taken from http://csharptest.net/526/how-to-search-the-environments-path-for-an-exe-or-dll/index.html

            var path_to_postgres = FindExePath("psql.exe");
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = path_to_postgres;
            string current_directory_path = Directory.GetCurrentDirectory();
            string path_to_download_script = current_directory_path + "\\" + DownLoad_script_file_name;
            Regex rgx = new Regex("vdp..");
            Regex rgx1 = new Regex(@"\btime..\b");
            string twoDigitsNumberOfFurnace = this.numberOfFurnace.ToString();
            twoDigitsNumberOfFurnace = this.numberOfFurnace > 9 ? twoDigitsNumberOfFurnace : "0" + twoDigitsNumberOfFurnace;
            //Далее следует закачка на локальный сервер,
            //корректируем номер печи в скрипте для закачки
            string path_to_upload_script = current_directory_path + "\\" + UpLoad_script_file_name;
            string upLoadScript = File.ReadAllText(path_to_upload_script);
            upLoadScript = rgx1.Replace(rgx.Replace(upLoadScript, "vdp" + twoDigitsNumberOfFurnace), "time" + twoDigitsNumberOfFurnace);
            File.WriteAllText(path_to_upload_script, upLoadScript);
            startInfo.Arguments = Upload_server_credetials + " -f " + $"{UpLoad_script_file_name}" + " -o upload.log";
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
                MessageBox.Show("Не удалась выгрузка в локальную базу данных");
            }
            Updated.Text = "База обновлена";
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
        public void PutTheCursor(Point clickPoint)
        {
            //Show the nearest values in ListBoxes(TrxtBoxes)! 
            VoltagePlot.VerticalCursor(clickPoint);
            CurrentPlot.VerticalCursor(clickPoint);
            VacuumPlot.VerticalCursor(clickPoint);
            SolenoidUPlot.VerticalCursor(clickPoint);
            SolenoidIPlot.VerticalCursor(clickPoint);
        }


        private void VoltagePlot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            datacontext.CanvasX = clickPoint.X;
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
            //datacontext.IsLeftMouseButtonPressed = true;
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
            //datacontext.IsLeftMouseButtonPressed = true;
            Point clickPoint = e.GetPosition((GraphCanvas)sender);
            PutTheCursor(clickPoint);
        }
        private void SolenoidIPlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //datacontext.IsLeftMouseButtonPressed = true;
                Point clickPoint = e.GetPosition((GraphCanvas)sender);
                PutTheCursor(clickPoint);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(MessageBoxResult.Yes == MessageBox.Show("Текущая локальная копия таблицы по печи будет заменена!\n Убедитесь в наличии связи сервером", "Закачка данных с сервера 31 цеха", MessageBoxButton.YesNo, MessageBoxImage.Question))
                            MapTheRemoteBase();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            UpLoadTheDataBaseFromTheCopy();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MapTheLocalBase("Set Edge Time Values");
        }

    }

}



