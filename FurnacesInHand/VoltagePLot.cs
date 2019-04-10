using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FurnacesInHand
{
    public partial class MainWindow
    {
        void voltagePlot()
        {
            VoltagePlot.Children?.Clear();
            VoltagePlot.Children.Add(new VoltageGraph());
        }
    }
}
