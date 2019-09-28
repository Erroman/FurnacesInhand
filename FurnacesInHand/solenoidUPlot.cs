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
        UIElement solenoidUGraph = null;
        void solenoidUPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                if (solenoidUGraph != null)
                    SolenoidUPlot.Children?.Remove(solenoidUGraph);
                Rect rectangular = new Rect(0, 0, SolenoidUPlot.ActualWidth, SolenoidUPlot.ActualHeight);
                FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                //Установить верхние и нижние границы значений, отображаеиых на графике
                SolenoidUMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                SolenoidUMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                solenoidUGraph = new SolenoidUGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime, vm: vm);
                SolenoidUPlot.Children.Add(solenoidUGraph);
                return null;
            }
             ), null);
        }

    }
}
