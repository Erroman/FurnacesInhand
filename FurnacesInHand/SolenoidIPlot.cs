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
        UIElement solenoidIGraph = null;
        void solenoidIPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                if(solenoidIGraph!=null)
                    SolenoidIPlot.Children?.Remove(solenoidIGraph);
                Rect rectangular = new Rect(0, 0, SolenoidIPlot.ActualWidth, SolenoidIPlot.ActualHeight);
                FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                //Установить верхние и нижние границы значений, отображаеиых на графике
                SolenoidIMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                SolenoidIMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                solenoidIGraph = new SolenoidIGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime, vm: vm);
                SolenoidIPlot.Children.Add(solenoidIGraph);
                return null;
            }
             ), null);
        }

    }
}
