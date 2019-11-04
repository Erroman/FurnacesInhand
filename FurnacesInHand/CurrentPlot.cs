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
        UIElement currentGraph = null;
        void currentPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                if(currentGraph!=null)
                    CurrentPlot.Children?.Remove(currentGraph);
                Rect rectangular = new Rect(0, 0, CurrentPlot.ActualWidth, CurrentPlot.ActualHeight);
                FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                //Установить верхние и нижние границы значений, отображаеиых на графике
                CurrentMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                CurrentMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                currentGraph = new CurrentGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime, vm: vm);
                CurrentPlot.Children.Add(currentGraph);
                return null;
            }
             ), null);
        }

    }
}
