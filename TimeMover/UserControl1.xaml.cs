using System;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeMover
{
    public class AlarmEventArgs : EventArgs
    {
        public int MillisecondsToAlarm
        {
            get;
            set;
        }
    }
partial class UserControl1 : UserControl
    { 
    public ClockWatch clockWatch;
    public UserControl1()
    {
        InitializeComponent();
        clockWatch = (ClockWatch)Resources["newClock"];
    }
    }
}