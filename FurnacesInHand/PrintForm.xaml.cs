﻿    using System;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FurnacesInHand
{
    /// <summary>
    /// Interaction logic for PrintForm.xaml
    /// </summary>
    public partial class PrintForm : Window
    {
        UIElement voltageGraph = null;
        UIElement currentGraph = null;
        UIElement vacuumGraph = null;
        UIElement solenoidUGraph = null;
        UIElement solenoidIGraph = null;

        public PrintForm()
        {
            InitializeComponent();
        }
        public void voltagePlot(List<TimeParameterPair> Voltage_graph_pairs, DateTime startTime, DateTime finishTime, FurnacesInHandViewModel vm = null) 
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                if (voltageGraph != null)
                    VoltagePlot.Children?.Remove(voltageGraph);
                Rect rectangular = new Rect(0, 0, VoltagePlot.ActualWidth, VoltagePlot.ActualHeight);
                //Установить верхние и нижние границы значений, отображаеиых на графике
                voltageGraph = new VoltageGraph(Voltage_graph_pairs, rect: rectangular, startTime: startTime, finishTime: finishTime, vm: vm);
                VoltagePlot.Children.Add(voltageGraph);
                return null;
            }
             ), null);
        }
        public void currentPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void vacuumPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void solenoidUPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void solenoidIPlot(List<TimeParameterPair> timeParameterPairs) { }
    }
}
