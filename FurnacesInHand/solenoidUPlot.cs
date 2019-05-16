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
        void solenoidUPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                SolenoidUPlot.Children?.Clear();
                Rect rectangular = new Rect(0, 0, SolenoidUPlot.ActualWidth, SolenoidUPlot.ActualHeight);
                _ = SolenoidUPlot.Children.Add(new SolenoidUGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime));
                return null;
            }
             ), null);
        }

    }
}
