using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FurnacesInHand
{
    public partial class MainWindow
    {
        void voltagePlot(IEnumerable<TimeParameterPair> timeParameterPair)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
             {
                 VoltagePlot.Children?.Clear();
                 Rect rectangular = new Rect(0, 0, VoltagePlot.ActualWidth, VoltagePlot.ActualHeight);
                 _ = VoltagePlot.Children.Add(new VoltageGraph(timeParameterPair, rect: rectangular,startTime:this.startTime,finishTime:this.finishTime));
                 return null;
             }
             ), null);
        }

    }
}
