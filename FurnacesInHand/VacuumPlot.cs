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
        void vacuumPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                VacuumPlot.Children?.Clear();
                Rect rectangular = new Rect(0, 0, VacuumPlot.ActualWidth, VacuumPlot.ActualHeight);
                _ = VacuumPlot.Children.Add(new VacuumGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime));
                return null;
            }
             ), null);
        }

    }
}
