using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                //Установить верхние и нижние границы значений, отображаеиых на графике
                SolenoidIMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                SolenoidIMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                _ = SolenoidIPlot.Children.Add(new SolenoidIGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime,vm:vm));
                return null;
            }
             ), null);
        }

    }
}
