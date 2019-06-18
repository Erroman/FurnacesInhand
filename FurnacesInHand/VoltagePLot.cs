using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;

namespace FurnacesInHand
{
    public partial class MainWindow
    {
        void voltagePlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
             {
                 VoltagePlot.Children?.Clear();
                 Rect rectangular = new Rect(0, 0, VoltagePlot.ActualWidth, VoltagePlot.ActualHeight);
                 FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                 //Установить верхние и нижние границы значений, отображаеиых на графике
                 VoltageMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                 VoltageMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                 _ = VoltagePlot.Children.Add(new VoltageGraph(timeParameterPairs, rect: rectangular,startTime:this.startTime,finishTime:this.finishTime,vm:vm));
                 return null;
             }
             ), null);
        }

    }
}
