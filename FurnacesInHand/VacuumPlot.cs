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
        UIElement vacuumGraph = null;
        void vacuumPlot(List<TimeParameterPair> timeParameterPairs)
        {

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (Object state)
            {
                if(vacuumGraph!=null)
                    VacuumPlot.Children?.Remove(vacuumGraph);
                Rect rectangular = new Rect(0, 0, VacuumPlot.ActualWidth, VacuumPlot.ActualHeight);
                FurnacesInHandViewModel vm = (FurnacesInHandViewModel)this.DataContext;
                //Установить верхние и нижние границы значений, отображаеиых на графике
                VacuumMax.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                VacuumMin.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                vacuumGraph = new VacuumGraph(timeParameterPairs, rect: rectangular, startTime: this.startTime, finishTime: this.finishTime, vm: vm);
                VacuumPlot.Children.Add(vacuumGraph);
                return null;
            }
             ), null);
        }

    }
}
