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
        void solenoidIPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                SolenoidIPlot.Children?.Clear();
                Rect rectangular = new Rect(0, 0, SolenoidIPlot.ActualWidth, SolenoidIPlot.ActualHeight);
                _ = SolenoidIPlot.Children.Add(new SolenoidIGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime));
                return null;
            }
             ), null);
        }

    }
}
