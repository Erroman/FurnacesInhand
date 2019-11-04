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
using System.Windows.Shapes;

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
        public void voltagePlot(List<TimeParameterPair> Voltage_graph_pairs) { }
        public void currentPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void vacuumPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void solenoidUPlot(List<TimeParameterPair> timeParameterPairs) { }
        public void solenoidIPlot(List<TimeParameterPair> timeParameterPairs) { }
    }
}
